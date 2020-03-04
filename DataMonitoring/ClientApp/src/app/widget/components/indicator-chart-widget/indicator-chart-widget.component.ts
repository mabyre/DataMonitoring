import { Component, OnInit, Input, OnDestroy } from '@angular/core';
import {FormGroup, FormArray, FormControl, Validators} from "@angular/forms";
import {Indicator} from "@app/setting/indicator/indicator";
import {Color} from "@app/shared/models/color";
import {TargetIndicatorChartWidget} from "@app/widget/models/indicator-chart-widget";
import {WidgetsService} from "@app/widget/widget.service";
import {IndicatorService} from "@app/setting/indicator/indicator.service";
import {TimeManagementService} from "@app/setting/time-management/time-management-service";
import {Subscription} from "rxjs";

@Component({
  selector: 'app-indicator-chart-widget',
  templateUrl: './indicator-chart-widget.component.html',
})
export class IndicatorChartWidgetComponent implements OnInit, OnDestroy {

  @Input() indicatorChartWidget: FormGroup;
  @Input() colorList: Color[];
  @Input() timeManagementId: number;

  public widgetIsOnSlipperyTime: boolean = false;
  timeManagementSubscription: Subscription;

  public indicatorList: Indicator[];
  public errorMessage: string;

  constructor(private widgetService: WidgetsService, private indicatorsService: IndicatorService, private timeManagementService: TimeManagementService) { }

  ngOnInit() {

    this.timeManagementSubscription = this.widgetService.timeManagementSubject.subscribe(
      (tmId: number) => {
        this.refreshTimeManagementState(tmId);
      }
    );

    if (this.timeManagementId != null) {
      this.refreshTimeManagementState(this.timeManagementId);
    }
    
    this.indicatorsService.get()
      .subscribe(result => {
        // Uniquement des Indicateurs de type IndicatorType.Flow OU IndicatorType.Ratio
        this.indicatorList = result.filter(x => x.type == 1 || x.type == 2);
      }, error => {
        this.errorMessage = error;
      });
  }

  private refreshTimeManagementState(timeManagementId: number) {
    this.widgetIsOnSlipperyTime = false;
    if (timeManagementId != null || timeManagementId != 0) {
      this.timeManagementService.getById(timeManagementId)
        .subscribe(result => {
          if (result.slipperyTime != null) {
            this.widgetIsOnSlipperyTime = true;
            this.resetTargetList();
          }
        }, error => {
          this.errorMessage = error;
        });
    }
  }
  
  resetTargetList(): any {
    const targetList = this.indicatorChartWidget.get('targetIndicatorChartWidgetFormList') as FormArray;
    while (targetList.length) {
      targetList.removeAt(0);
    }
  }

  ngOnDestroy() {
    this.timeManagementSubscription.unsubscribe();
  }


  getTargetIndicatorChartWidgetFormArray() {
    return this.indicatorChartWidget.get('targetIndicatorChartWidgetFormList') as FormArray;
  }

  onAddTargetIndicatorChartWidget() {
    const array = this.getTargetIndicatorChartWidgetFormArray();
    array.push(this.widgetService.getTargetIndicatorChartWidgetFormGroup(new TargetIndicatorChartWidget()));
  }

  onRemoveTargetIndicatorChartWidget(index: number) {
    const control = this.getTargetIndicatorChartWidgetFormArray();
    control.removeAt(index);
  }

  getTargetIndicatorDisplay(index: number): string {
    const formArray = this.getTargetIndicatorChartWidgetFormArray();
    return formArray.controls[index].get('startDate').value + " - " + formArray.controls[index].get('endDate').value;
  }
}
