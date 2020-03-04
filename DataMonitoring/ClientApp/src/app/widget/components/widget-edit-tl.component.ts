import { Component, OnInit, AfterViewInit } from '@angular/core';
import { FormGroup, FormControl, FormArray, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { CdkDragDrop, moveItemInArray } from "@angular/cdk/drag-drop";

import { I18nService } from '@app/shared/i18n/i18n.service';
import { TimeZone } from '@app/core/timeZone';
import { TimeZoneService } from '@app/core/services';

import { WidgetsService } from '../widget.service';
import { Widget, TitleWidgetTranlation } from '../models/widget';
import { Color } from "@app/shared/models/color";
import { StyleService } from "@app/setting/style/style.service";
import { Style } from "@app/shared/models/style";
import { IndicatorService } from "@app/setting/indicator/indicator.service";
import { Indicator } from "@app/setting/indicator/indicator";
import { IndicatorTableWidget } from "../models/indicator-table-widget";
import { IndicatorBarWidget } from "../models/indicator-bar-widget";
import { IndicatorChartWidget, TargetIndicatorChartWidget } from "../models/indicator-chart-widget";
import { TimeManagementService } from "@app/setting/time-management/time-management-service";
import { Configuration } from "@app/core/configuration";
import { IndicatorGaugeWidget } from "../models/indicator-gauge-widget";

@Component({
  selector: 'app-widget-edit-tl',
  templateUrl: './widget-edit-tl.component.html',
  styleUrls: ['cdk-drag-drop.css'],
})
export class WidgetEditTlComponent implements OnInit {

  KeyPreviewToProd: string = "9AB3B0C9-6E01-43C8-A7B2-0EF548E2B5DF";

  public widgetForm: FormGroup;

  errorMessage: string;
  infoMessage: string;
  successMessage: string;

  widgetTypes: any[];
  columnStyleList: any[];
  alignStyleList: any[];
  columnTypeList: any[] = [];
  timeManagements: any[];
  colorList: Color[];
  styleList: Style[] = [];
  indicatorList: Indicator[];
  languageList: any[];
  timeZones: TimeZone[];
  currentServerTimezone: string;
  selectedColor: any;
  traductionAvailable: number = 0;
  tabnumber: number = 0;
  indicatorWidgetCount: number = 0;
  collapseAllGroup: boolean = true;

  isTableWidget = false;
  isBarWidget = false;
  isChartWidget = false;
  isGaugeWidget = false;

  isSubmited = false;

  widget: Widget;

  constructor(private widgetService: WidgetsService, private i18nservice: I18nService, private route: ActivatedRoute,
    private styleService: StyleService, private indicatorService: IndicatorService,
    private timeManagementService: TimeManagementService, private timeZoneService: TimeZoneService,
    private configuration: Configuration) { }

  ngOnInit() {

    this.timeZoneService.getTimeZones()
    .subscribe(result => {
      this.timeZones = result;
    }, error => {
      this.errorMessage = error;
    });

    this.timeZoneService.getCurrentTimeZone()
    .subscribe(result => {
      this.currentServerTimezone = result.id;
    }, error => {
      this.errorMessage = error;
    });

    this.widgetService.getWidgetTypeList()
      .subscribe(result => {
        this.widgetTypes = result;
      }, error => {
        this.errorMessage = error;
      });

    this.widgetService.getTimeManagementList()
      .subscribe(result => {
        this.timeManagements = result;
      }, error => {
        this.errorMessage = error;
      });

    this.widgetService.getColorList()
      .subscribe(result => {
        this.colorList = result;
      }, error => {
        this.errorMessage = error;
      });

    this.styleService.get()
      .subscribe(result => {
        const emptyStyle = new Style();
        this.styleList.push(emptyStyle);
        result.forEach(element => {
          this.styleList.push(element);
        });
      }, error => {
        this.errorMessage = error;
      });

    this.indicatorService.get()
      .subscribe(result => {
        this.indicatorList = result;
      }, error => {
        this.errorMessage = error;
      });

    this.widgetService.getColumnStyleList()
      .subscribe(result => {
        this.columnStyleList = result;
      }, error => {
        this.errorMessage = error;
      });

    this.widgetService.getAlignStyleList()
      .subscribe(result => {
        this.alignStyleList = result;
      }, error => {
        this.errorMessage = error;
      });

    this.widgetService.getColumnTypeList()
      .subscribe(result => {
        result.forEach(element => {
          if (element.value != 0) {
            this.columnTypeList.push(element);
          }
        });
      }, error => {
        this.errorMessage = error;
      });

    this.languageList = this.configuration.languages;

    const id = +this.route.snapshot.paramMap.get('id');

    if (id != 0) {
      this.isSubmited = true;
      this.widgetService.getById(id)
        .subscribe(result => {
          this.widget = result;
          if (this.widget != null && this.colorList != null) {
            this.selectedColor = this.colorList.find(x => x.name === this.widget.titleColorName).txtClassName;
          }
          this.refreshWidget();
          this.initWidgetForm();
        },
          error => {
            this.errorMessage = error;
          });
    } else {
      this.widget = new Widget();
      this.initWidgetForm();
    }
  }

  private initWidgetForm() {

    if (this.colorList != null) {
      const color = this.colorList.find(x => x.name === this.widget.titleColorName);
      this.selectedColor = color != null ? color.txtClassName: '';
    }

    if (this.widget.titleTranslate != null) {
      this.traductionAvailable = this.widget.titleTranslate.length;
    }

    this.widget.isTestModePreview = true;

    this.widgetForm = new FormGroup({
      'type': new FormControl(this.widget.type, [Validators.required]),
      'title': new FormControl(this.widget.title, [Validators.required, Validators.maxLength(200)]),
      'titleFontSize': new FormControl(this.widget.titleFontSize, [Validators.required, Validators.min(15), Validators.max(30)]),
      'titleTranslate': new FormArray([]),
      'titleColorName': new FormControl(this.widget.titleColorName, [Validators.required]),
      'refreshTime': new FormControl(this.widget.refreshTime, [Validators.required, Validators.min(0)]),
      'titleDisplayed': new FormControl(this.widget.titleDisplayed),
      'currentTimeManagementDisplayed': new FormControl(this.widget.currentTimeManagementDisplayed),
      'lastRefreshTimeIndicatorDisplayed': new FormControl(this.widget.lastRefreshTimeIndicatorDisplayed),
      'timeManagementId': new FormControl(this.widget.timeManagementId, [Validators.required, Validators.pattern('^(?!0).*')]),
      'isTestModePreview': new FormControl(this.widget.isTestModePreview),
      'timeZone': new FormControl(this.currentServerTimezone),

      'indicatorTableWidgetList': new FormArray([]),
      'indicatorBarWidget': new FormArray([]),
      'indicatorChartWidget': new FormArray([]),
      'indicatorGaugeWidget': new FormArray([]),
    });

    if (this.widget.indicatorTableWidgetList != null) {
      this.widget.indicatorTableWidgetList = this.widget.indicatorTableWidgetList.sort(function(a, b) {
        return a.sequence - b.sequence;
      });
      this.widget.indicatorTableWidgetList.forEach(x => this.onAddIndicatorTableWidget(x));
    }

    if (this.widget.titleTranslate != null) {
      this.widget.titleTranslate.forEach(x => this.onAddTitleTranslate(x));
    }

    this.InitializeIndicatorBarWidget(this.widget.indicatorBarWidget);
    this.InitializeIndicatorChartWidget(this.widget.indicatorChartWidget);
    this.InitializeIndicatorGaugeWidget(this.widget.indicatorGaugeWidget);
  }

  get form() { return this.widgetForm.controls; }

  get formTitleArray() {
    return (this.widgetForm.get('titleTranslate') as FormArray).controls;
  }

  get widgetTypeSelected(): any { return this.widgetForm.get('type'); }

  get colorSelected(): any { return this.widgetForm.get('titleColor'); }

  get widgetAddress(): any { return 'api/monitor/widget/' + this.widget.id; }

  get widgetName(): any { return 'wid' + this.widget.id; }

  onChangeType() {
    this.errorMessage = '';
    this.infoMessage = '';
    this.successMessage = '';
  }

  onChangeTitleColor(value) {
    this.selectedColor = this.colorList.find(x => x.name === value).txtClassName;
  }

  getLocalizationsArray() {
    return this.widgetForm.get('titleTranslate') as FormArray;
  }

  onAddTitleTranslate(widget: TitleWidgetTranlation) {
    const formArray = this.getLocalizationsArray();
    formArray.push(new FormGroup({
      'id': new FormControl(widget != null ? widget.id : 0),
      'localizationCode': new FormControl(widget != null ? widget.localizationCode : '', [Validators.required]),
      'title': new FormControl(widget != null ? widget.title : '', [Validators.required, Validators.maxLength(200)])
    }));
    this.traductionAvailable = formArray.length;
  }



  onRemoveTitleTranslate(index: number) {
    const formArray = this.widgetForm.get('titleTranslate') as FormArray;
    formArray.removeAt(index);
    this.traductionAvailable = formArray.length;
  }

  onSubmit() {
    this.errorMessage = null;
    this.infoMessage = '';
    this.successMessage = '';

    // stop here if form is invalid
    if (this.widgetForm.invalid) {
      return;
    }

    const widgetFormValue = <Widget>this.widgetForm.value;

    this.widget.titleColorName = widgetFormValue.titleColorName;
    this.widget.title = widgetFormValue.title;
    this.widget.titleFontSize = widgetFormValue.titleFontSize;
    this.widget.titleTranslate = widgetFormValue.titleTranslate;
    this.widget.type = widgetFormValue.type;
    this.widget.refreshTime = widgetFormValue.refreshTime;
    this.widget.currentTimeManagementDisplayed = widgetFormValue.currentTimeManagementDisplayed;
    this.widget.titleDisplayed = widgetFormValue.titleDisplayed;
    this.widget.lastRefreshTimeIndicatorDisplayed = widgetFormValue.lastRefreshTimeIndicatorDisplayed;
    this.widget.timeManagementId = widgetFormValue.timeManagementId;

    // Type IndicatorTable :
    this.widget.indicatorTableWidgetList = this.getTableWidget(widgetFormValue.indicatorTableWidgetList);

    // Type IndicatorBar :
    if (this.widget.type == 4 && this.isSubmited) {
      this.widget.indicatorBarWidget = widgetFormValue.indicatorBarWidget[0];
      if (this.checkAnyErrorsOnIndicatorBarWidget()) {
        return;
      }
    }

    // Type IndicatorChart :
    if (this.widget.type == 3 && this.isSubmited) {
      this.widget.indicatorChartWidget = <IndicatorChartWidget>widgetFormValue.indicatorChartWidget[0];

      if (this.checkAnyErrorsOnIndicatorChartWidget()) {
        return;
      }

      this.widget.indicatorChartWidget.targetIndicatorChartWidgetList = [];
      const selectedTimeManagement = this.timeManagements.find(t => t.id == this.widget.timeManagementId);
      if (selectedTimeManagement.slipperyTime == null) {
        // on récupère la liste de TargetIndicator si on est pas en temps glissant
        this.widget.indicatorChartWidget.targetIndicatorChartWidgetFormList.forEach(element => {

          const targetIndicatorChartWidget = new TargetIndicatorChartWidget();
          targetIndicatorChartWidget.startDateUtc = this.timeManagementService.getUtcDate(element.startDate.toLocaleString());
          targetIndicatorChartWidget.id = element.id;
          targetIndicatorChartWidget.startTargetValue = element.startTargetValue;
          targetIndicatorChartWidget.endDateUtc = this.timeManagementService.getUtcDate(element.endDate.toLocaleString());
          targetIndicatorChartWidget.endTargetValue = element.endTargetValue;
          this.widget.indicatorChartWidget.targetIndicatorChartWidgetList.push(targetIndicatorChartWidget);
        });
      }
    }

    // Type IndicatorGauge :
    if (this.widget.type == 5 && this.isSubmited) {
      this.widget.indicatorGaugeWidget = widgetFormValue.indicatorGaugeWidget[0];
    }

    if (this.errorMessage == null) {
      if (this.widget.id === 0) {
        this.widgetService.post(this.widget)
          .subscribe(result => {
            this.widget.id = +result;
            this.successMessage = this.i18nservice.getTranslation('Création effectuée...');
            this.refreshWidget();
            this.isSubmited = true;
            setTimeout(() => this.successMessage = '', 5000);
          },
            error => {
              this.errorMessage = error;
            });
      } else {
        this.widgetService.put(this.widget.id, this.widget)
          .subscribe(result => {
            this.successMessage = this.i18nservice.getTranslation('Mise à jour effectuée...');
            setTimeout(() => this.successMessage = '', 5000);
          },
            error => {
              this.errorMessage = error;
            });
      }

      console.log('submitted');
    }
  }

  checkAnyErrorsOnIndicatorBarWidget(): boolean {
      if (this.widget.indicatorBarWidget.indicatorId == null || this.widget.indicatorBarWidget.indicatorId == 0) {
        this.errorMessage = this.i18nservice.getTranslation('IndicatorRequired');
        return true;
      }
      if (this.widget.indicatorBarWidget.indicatorBarWidgetColumnList == null || this.widget.indicatorBarWidget.indicatorBarWidgetColumnList.length == 0) {
        this.errorMessage = this.i18nservice.getTranslation('IndicatorBarWidgetColumnRequired');
        return true;
      }
      if (this.widget.indicatorBarWidget.indicatorBarWidgetColumnList != null && this.widget.indicatorBarWidget.indicatorBarWidgetColumnList.length > 0) {
        const result = this.widget.indicatorBarWidget.indicatorBarWidgetColumnList.find(x => x.isNumericFormat);
        if (result == null) {
          this.errorMessage = this.i18nservice.getTranslation('IndicatorBarWidgetColumnWithNumericFormatRequired');
          return true;
        }
      }
      if (this.widget.indicatorBarWidget.labelColumnCode == null || this.widget.indicatorBarWidget.labelColumnCode == "") {
        this.errorMessage = this.i18nservice.getTranslation('labelColumnCodeRequired');
        return true;
      }
      if (this.widget.indicatorBarWidget.dataColumnCode == null || this.widget.indicatorBarWidget.dataColumnCode == "") {
        this.errorMessage = this.i18nservice.getTranslation('dataColumnCodeRequired');
        return true;
      }

      return false;
  }

  checkAnyErrorsOnIndicatorChartWidget(): boolean {
    if (this.widget.indicatorChartWidget.indicatorId == null || this.widget.indicatorChartWidget.indicatorId == 0) {
      this.errorMessage = this.i18nservice.getTranslation('IndicatorRequired');
      return true;
    }

    return false;
}

  onTimeManagementChange() {
    const selectedTimeManagementId = this.widgetForm.get('timeManagementId').value;
    this.widgetService.changeTimeManagement(selectedTimeManagementId);
  }


  /////////////////////////////////////////////////////////
  // IndicatorTable Widget
  /////////////////////////////////////////////////////////
  getTableWidget(indicatorTableWidgetList: IndicatorTableWidget[]): IndicatorTableWidget[] {
    const newTableWidget: IndicatorTableWidget[] = [];
    indicatorTableWidgetList.forEach(element => {
      if (!element.titleIndicatorDisplayed) {
        element.titleIndicatorColor = null;
      }
      if (!element.rowStyleDisplayed) {
        element.rowStyleWhenEqualValue = null;
        element.equalsValue = null;
        element.columnCode = null;
      }
      newTableWidget.push(element);
    });

    return newTableWidget;
  }

  onCollapseAll() {
    this.collapseAllGroup = !this.collapseAllGroup;
  }

  refreshWidget() {
    if (this.widgetTypes != null) {
      const selectedType = this.widgetTypes.find(x => x.value == this.widget.type);
      if (selectedType.value ==  0 || selectedType.value ==  1 || selectedType.value ==  2) {
        this.isTableWidget = true;
      } else if (selectedType.value ==  3) {
        this.isChartWidget = true;
      } else if (selectedType.value ==  4) {
        this.isBarWidget = true;
      } else if (selectedType.value == 5) {
        this.isGaugeWidget = true;
      }
    }
  }

  onAddIndicatorTableWidget(indicatorTableWidget: IndicatorTableWidget) {
    if (indicatorTableWidget == null) {
      indicatorTableWidget = new IndicatorTableWidget();
    }

    const tableWidgetColumnFormArray = new FormArray([]);
    if (indicatorTableWidget.tableWidgetColumnList != null) {
      indicatorTableWidget.tableWidgetColumnList = indicatorTableWidget.tableWidgetColumnList.sort(function(a, b) {
        return a.sequence - b.sequence;
      });

      indicatorTableWidget.tableWidgetColumnList.forEach(element => {

        const columnNameLocalizationsFormArray = new FormArray([]);
        if (element.columnNameLocalizations != null) {
          element.columnNameLocalizations.forEach(loc =>
            columnNameLocalizationsFormArray.push(new FormGroup({
              'id': new FormControl(loc.id, [Validators.required]),
              'localizationCode': new FormControl(loc.localizationCode, [Validators.required]),
              'title': new FormControl(loc.title, [Validators.required, Validators.maxLength(60)]),
            })));
          }

        tableWidgetColumnFormArray.push(this.widgetService.getTableWidgetColumnFormGroup(element, null, columnNameLocalizationsFormArray));
      });
    }

    const formArray = this.getIndicatorTableWidgetArray();
    formArray.push(new FormGroup({
      'id': new FormControl(indicatorTableWidget.id, [Validators.required]),
      'indicatorId': new FormControl(indicatorTableWidget.indicatorId, [Validators.required, Validators.pattern('^(?!0).*')]),
      'sequence': new FormControl(indicatorTableWidget.sequence, [Validators.required]),
      'headerDisplayed': new FormControl(indicatorTableWidget.headerDisplayed, [Validators.required]),
      'targetValue': new FormControl(indicatorTableWidget.targetValue),
      'titleIndicatorDisplayed': new FormControl(indicatorTableWidget.titleIndicatorDisplayed, [Validators.required]),
      'titleIndicatorColor': new FormControl(indicatorTableWidget.titleIndicatorColor),
      'rowStyleDisplayed': new FormControl(indicatorTableWidget.rowStyleWhenEqualValue != null && indicatorTableWidget.rowStyleWhenEqualValue != ''
        ? true
        : false),
      'rowStyleWhenEqualValue': new FormControl(indicatorTableWidget.rowStyleWhenEqualValue),
      'equalsValue': new FormControl(indicatorTableWidget.equalsValue, [Validators.maxLength(60)]),
      'columnCode': new FormControl(indicatorTableWidget.columnCode, [Validators.maxLength(60)]),
      'tableWidgetColumnList' : tableWidgetColumnFormArray,
    }));

    this.indicatorWidgetCount = formArray.length;
  }

  getIndicatorTableWidgetArray() {
    return this.widgetForm.get('indicatorTableWidgetList') as FormArray;
  }

  onRemoveIndicatorTableWidget(index: number) {
    const formArray = this.getIndicatorTableWidgetArray();
    formArray.removeAt(index);
    this.indicatorWidgetCount = formArray.length;
  }

  onDuplicateIndicatorTableWidget(index: number) {
    const formArray = this.getIndicatorTableWidgetArray();
    const indicatorTableWidgetForm = formArray.at(index);
    const indicatorTableWidget = <IndicatorTableWidget>indicatorTableWidgetForm.value;
    indicatorTableWidget.id = 0;
    indicatorTableWidget.tableWidgetColumnList.forEach(element => {
      element.id = 0;
      element.columnNameLocalizations.forEach(loc => {
        loc.id = 0;
      });
    });

    this.onAddIndicatorTableWidget(indicatorTableWidget);
  }

  getIndicatorTitle(id: number): string {
    if (id != 0 && this.indicatorList != null) {
      return this.indicatorList.find(x => x.id == id).title;
    } else {
      return '';
    }
  }

  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(this.getIndicatorTableWidgetArray().controls, event.previousIndex, event.currentIndex);
  }
  /////////////////////////////////////////////////////////




  /////////////////////////////////////////////////////////
  // IndicatorBar Widget
  /////////////////////////////////////////////////////////

  InitializeIndicatorBarWidget(indicatorBarWidget: IndicatorBarWidget): any {
    if (indicatorBarWidget == null) {
      indicatorBarWidget = new IndicatorBarWidget();
    }

    // IndicatorBarWidgetColumn :
    const indicatorBarWidgetColumnFormArray = new FormArray([]);
    if (indicatorBarWidget.indicatorBarWidgetColumnList != null) {
      indicatorBarWidget.indicatorBarWidgetColumnList.forEach(element => {
        indicatorBarWidgetColumnFormArray.push(this.widgetService.getIndicatorBarWidgetColumnFormGroup(element));
      });
    }

    // BarLabelWidget :
    const barLabelWidgetFormArray = new FormArray([]);
    if (indicatorBarWidget.barLabelWidgetList != null) {
      indicatorBarWidget.barLabelWidgetList = indicatorBarWidget.barLabelWidgetList.sort(function(a, b) {
        return a.sequence - b.sequence;
      });

      indicatorBarWidget.barLabelWidgetList.forEach(element => {

        const barLabelWidgetLocalizationFormArray = new FormArray([]);
        if (element.barLabelWidgetLocalizationList != null) {
          element.barLabelWidgetLocalizationList.forEach(loc =>
            barLabelWidgetLocalizationFormArray.push(new FormGroup({
              'id': new FormControl(loc.id, [Validators.required]),
              'localizationCode': new FormControl(loc.localizationCode, [Validators.required]),
              'title': new FormControl(loc.title, [Validators.required, Validators.maxLength(60)]),
            })));
          }

          barLabelWidgetFormArray.push(new FormGroup({
          'id': new FormControl(element.id, [Validators.required]),
          'name': new FormControl(element.name),
          'sequence': new FormControl(element.sequence, [Validators.required]),
          'labelTextColor': new FormControl(element.labelTextColor, [Validators.required]),
          'useLabelColorForBar': new FormControl(element.useLabelColorForBar, [Validators.required]),
          'barLabelWidgetLocalizationList': barLabelWidgetLocalizationFormArray,
        }));
      });
    }

    // IndicatorBarWidget :
    const indicatorBarWidgetForm = this.getIndicatorBarWidgetFormArray();
      indicatorBarWidgetForm.push(new FormGroup({
        'id': new FormControl(indicatorBarWidget.id),
        'indicatorId': new FormControl(indicatorBarWidget.indicatorId),
        'targetValue': new FormControl(indicatorBarWidget.targetValue),
        'titleIndicatorDisplayed': new FormControl(indicatorBarWidget.titleIndicatorDisplayed),
        'titleIndicatorColor': new FormControl(indicatorBarWidget.titleIndicatorColor),

        // Axe X
        'displayAxeX': new FormControl(indicatorBarWidget.displayAxeX),
        'labelColumnCode': new FormControl(indicatorBarWidget.labelColumnCode),
        'labelFontSize': new FormControl(indicatorBarWidget.labelFontSize),
        'labelColorText': new FormControl(indicatorBarWidget.labelColorText),

        // Axe Y
        'displayAxeY': new FormControl(indicatorBarWidget.displayAxeY),
        'displayDataAxeY': new FormControl(indicatorBarWidget.displayDataAxeY),
        'textDataAxeYColor': new FormControl(indicatorBarWidget.textDataAxeYColor),
        'displayGridLinesAxeY': new FormControl(indicatorBarWidget.displayGridLinesAxeY),

        // DataLabel
        'dataColumnCode': new FormControl(indicatorBarWidget.dataColumnCode),
        'displayDataLabelInBar': new FormControl(indicatorBarWidget.displayDataLabelInBar),
        'dataLabelInBarColor': new FormControl(indicatorBarWidget.dataLabelInBarColor),
        'fontSizeDataLabel': new FormControl(indicatorBarWidget.fontSizeDataLabel),
        'decimalMask': new FormControl(indicatorBarWidget.decimalMask),
        'barColor': new FormControl(indicatorBarWidget.barColor),

        // Stacked bar
        'addBarStackedToTarget': new FormControl(indicatorBarWidget.addBarStackedToTarget),
        'barColorStackedToTarget': new FormControl(indicatorBarWidget.barColorStackedToTarget),

        // Target
        'addTargetBar': new FormControl(indicatorBarWidget.addTargetBar),
        'targetBarColor': new FormControl(indicatorBarWidget.targetBarColor),
        'setSumDataToTarget': new FormControl(indicatorBarWidget.setSumDataToTarget),

        'indicatorBarWidgetColumnList': indicatorBarWidgetColumnFormArray,
        'barLabelWidgetList': barLabelWidgetFormArray,
      }));
  }

  getIndicatorBarWidgetFormArray() {
    return this.widgetForm.get('indicatorBarWidget') as FormArray;
  }
  /////////////////////////////////////////////////////////



  /////////////////////////////////////////////////////////
  // IndicatorChart Widget
  /////////////////////////////////////////////////////////

  InitializeIndicatorChartWidget(indicatorChartWidget: IndicatorChartWidget): any {
    if (indicatorChartWidget == null) {
      indicatorChartWidget = new IndicatorChartWidget();
    }

    // TargetIndicatorChartWidget :
    const targetIndicatorChartWidgetFormArray = new FormArray([]);
    if (indicatorChartWidget.targetIndicatorChartWidgetList != null) {
      indicatorChartWidget.targetIndicatorChartWidgetList.forEach(element => {
        targetIndicatorChartWidgetFormArray.push(this.widgetService.getTargetIndicatorChartWidgetFormGroup(element));
      });
    }

    // IndicatorChartWidget :
    const indicatorChartWidgetForm = this.getIndicatorChartWidgetFormArray();
      indicatorChartWidgetForm.push(new FormGroup({
        'id': new FormControl(indicatorChartWidget.id),
        'indicatorId': new FormControl(indicatorChartWidget.indicatorId),
        'targetValue': new FormControl(indicatorChartWidget.targetValue),
        'titleIndicatorDisplayed': new FormControl(indicatorChartWidget.titleIndicatorDisplayed),
        'titleIndicatorColor': new FormControl(indicatorChartWidget.titleIndicatorColor),

        'axeFontSize': new FormControl(indicatorChartWidget.axeFontSize),
        'decimalMask': new FormControl(indicatorChartWidget.decimalMask),
        // Axe X
        'axeXDisplayed': new FormControl(indicatorChartWidget.axeXDisplayed),
        'axeXColor': new FormControl(indicatorChartWidget.axeXColor),
        // Axe Y
        'axeYDisplayed': new FormControl(indicatorChartWidget.axeYDisplayed),
        'axeYDataDisplayed': new FormControl(indicatorChartWidget.axeYDataDisplayed),
        'axeYColor': new FormControl(indicatorChartWidget.axeYColor),
        // Target
        'chartTargetDisplayed': new FormControl(indicatorChartWidget.chartTargetDisplayed),
        'chartTargetColor': new FormControl(indicatorChartWidget.chartTargetColor),
        // Average
        'chartAverageDisplayed': new FormControl(indicatorChartWidget.chartAverageDisplayed),
        'chartAverageColor': new FormControl(indicatorChartWidget.chartAverageColor),
        // Chart
        'chartDataColor': new FormControl(indicatorChartWidget.chartDataColor),
        'chartDataFill': new FormControl(indicatorChartWidget.chartDataFill),
        // Groups
        'group1': new FormControl(indicatorChartWidget.group1),
        'group2': new FormControl(indicatorChartWidget.group2),
        'group3': new FormControl(indicatorChartWidget.group3),
        'group4': new FormControl(indicatorChartWidget.group4),
        'group5': new FormControl(indicatorChartWidget.group5),

        'axeYIsAutoAdjustableAccordingMinValue': new FormControl(indicatorChartWidget.axeYIsAutoAdjustableAccordingMinValue),
        'axeYOffsetFromMinValue': new FormControl(indicatorChartWidget.axeYOffsetFromMinValue),

        'targetIndicatorChartWidgetFormList': targetIndicatorChartWidgetFormArray,
      }));
  }

  getIndicatorChartWidgetFormArray() {
    return this.widgetForm.get('indicatorChartWidget') as FormArray;
  }
  /////////////////////////////////////////////////////////


  /////////////////////////////////////////////////////////
  // IndicatorGauge Widget
  /////////////////////////////////////////////////////////

  InitializeIndicatorGaugeWidget(indicatorGaugeWidget: IndicatorGaugeWidget): any {
    if (indicatorGaugeWidget == null) {
      indicatorGaugeWidget = new IndicatorGaugeWidget();
    }

    const indicatorGaugeWidgetForm = this.getIndicatorGaugeWidgetFormArray();
    indicatorGaugeWidgetForm.push(new FormGroup({
        'id': new FormControl(indicatorGaugeWidget.id),
        'indicatorId': new FormControl(indicatorGaugeWidget.indicatorId),
        'targetValue': new FormControl(indicatorGaugeWidget.targetValue),
        'titleIndicatorDisplayed': new FormControl(indicatorGaugeWidget.titleIndicatorDisplayed),
        'titleIndicatorColor': new FormControl(indicatorGaugeWidget.titleIndicatorColor),

        // Target
        'targetDisplayed': new FormControl(indicatorGaugeWidget.targetDisplayed),
        'gaugeTargetColor': new FormControl(indicatorGaugeWidget.gaugeTargetColor),

        // Groups
        'group1': new FormControl(indicatorGaugeWidget.group1),
        'group2': new FormControl(indicatorGaugeWidget.group2),
        'group3': new FormControl(indicatorGaugeWidget.group3),
        'group4': new FormControl(indicatorGaugeWidget.group4),
        'group5': new FormControl(indicatorGaugeWidget.group5),

        // Range 1
        'gaugeRange1Color': new FormControl(indicatorGaugeWidget.gaugeRange1Color),
        'gaugeRange1MinValue': new FormControl(indicatorGaugeWidget.gaugeRange1MinValue),
        'gaugeRange1MaxValue': new FormControl(indicatorGaugeWidget.gaugeRange1MaxValue),

        // Range 2
        'gaugeRange2Displayed': new FormControl(indicatorGaugeWidget.gaugeRange2Displayed),
        'gaugeRange2Color': new FormControl(indicatorGaugeWidget.gaugeRange2Color),
        'gaugeRange2MinValue': new FormControl(indicatorGaugeWidget.gaugeRange2MinValue),
        'gaugeRange2MaxValue': new FormControl(indicatorGaugeWidget.gaugeRange2MaxValue),

        // Range 3
        'gaugeRange3Displayed': new FormControl(indicatorGaugeWidget.gaugeRange3Displayed),
        'gaugeRange3Color': new FormControl(indicatorGaugeWidget.gaugeRange3Color),
        'gaugeRange3MinValue': new FormControl(indicatorGaugeWidget.gaugeRange3MinValue),
        'gaugeRange3MaxValue': new FormControl(indicatorGaugeWidget.gaugeRange3MaxValue),
      }));
  }

  getIndicatorGaugeWidgetFormArray() {
    return this.widgetForm.get('indicatorGaugeWidget') as FormArray;
  }
  /////////////////////////////////////////////////////////
}

