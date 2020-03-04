import { Component, OnInit, Input } from '@angular/core';
import {FormGroup} from "@angular/forms";
import {Color} from "@app/shared/models/color";
import {Indicator} from "@app/setting/indicator/indicator";
import {IndicatorService} from "@app/setting/indicator/indicator.service";

@Component({
  selector: 'app-indicator-gauge-widget',
  templateUrl: './indicator-gauge-widget.component.html',
})
export class IndicatorGaugeWidgetComponent implements OnInit {

  @Input() indicatorGaugeWidget: FormGroup;
  @Input() colorList: Color[];

  public indicatorList: Indicator[];
  public errorMessage: string;

  constructor(private indicatorsService: IndicatorService) { }

  ngOnInit() {

    this.indicatorsService.get()
      .subscribe(result => {
        // Uniquement des Indicateurs de type IndicatorType.Flow OU IndicatorType.Ratio
        this.indicatorList = result.filter(x => x.type == 1 || x.type == 2);
      }, error => {
        this.errorMessage = error;
      });
  }

}
