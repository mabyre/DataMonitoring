//
// IndicatorBarWidgetColumn
// BarLabelWidget
//
import { Component, OnInit, Input } from '@angular/core';
import { FormGroup, FormArray, Validators, FormControl } from "@angular/forms";
import { Indicator } from "@app/setting/indicator/indicator";
import { IndicatorService } from "@app/setting/indicator/indicator.service";
import { Color } from "@app/shared/models/color";
import { CdkDragDrop, moveItemInArray } from "@angular/cdk/drag-drop";
import { IndicatorBarWidgetColumn, BarLabelWidget } from "@app/widget/models/indicator-bar-widget";
import { I18nService } from "@app/shared/i18n/i18n.service";
import { WidgetsService } from "@app/widget/widget.service";

@Component({
    selector: 'app-indicator-bar-widget',
    templateUrl: './indicator-bar-widget.component.html',
    styleUrls: ['../cdk-drag-drop.css'],
})
export class IndicatorBarWidgetComponent implements OnInit {

    @Input() indicatorBarWidget: FormGroup;
    @Input() colorList: Color[];

    public indicatorList: Indicator[];
    public errorMessage: string;
    public dataColumnList: string[] = [];
    public selectedIndexColumn = 0;

    constructor(private indicatorsService: IndicatorService, private i18nService: I18nService, private widgetService: WidgetsService) { }

    ngOnInit() {
        this.indicatorsService.get()
            .subscribe(result => {
                // Uniquement des Indicateurs de type IndicatorType.Snapshot
                this.indicatorList = result.filter(x => x.type == 0);
            }, error => {
                this.errorMessage = error;
            });

        this.onIndicatorChange();
    }

    onIndicatorChange() {
        const indicatorId = this.indicatorBarWidget.get('indicatorId').value;
        if (indicatorId != null && +indicatorId != 0) {
            this.indicatorsService.getIndicatorQueryColumns(+indicatorId)
                .subscribe(result => {
                    this.initializeCodeColumns(result);
                }, error => {
                    this.errorMessage = error;
                });
        }
    }

    initializeCodeColumns(result: any) {
        this.dataColumnList = [];
        this.dataColumnList.push("");
        if (result.length > 0) {
            const columnsIn = result[0];
            for (const key in columnsIn) {
                if (columnsIn.hasOwnProperty(key)) {
                    this.dataColumnList.push(key);
                }
            }
        }
    }

    //-----------------------------
    // IndicatorBarWidgetColumn
    //-----------------------------
    getIndicatorBarWidgetColumnFormArray() {
        return this.indicatorBarWidget.get('indicatorBarWidgetColumnList') as FormArray;
    }

    onRefreshIndicatorBarWidgetColumns() {
        this.errorMessage = null;

        const indicatorId = this.indicatorBarWidget.get('indicatorId').value;
        if (indicatorId == null || indicatorId == 0) {
            this.errorMessage = this.i18nService.getTranslation('IndicatorRequired');
            return;
        }

        if (this.errorMessage == null) {
            this.indicatorsService.getIndicatorQueryColumns(+indicatorId)
                .subscribe(result => {
                    this.initializeIndicatorBarWidgetColumns(result);
                }, error => {
                    this.errorMessage = error;
                });
        }
    }

    initializeIndicatorBarWidgetColumns(result: any): any {
        const columns = [];
        if (result.length > 0) {
            const columnsIn = result[0];
            for (const key in columnsIn) {
                if (columnsIn.hasOwnProperty(key)) {
                    columns.push(key);
                }
            }
        }

        const columnCodeListToRemove: string[] = [];
        const columnCodeListExisting: string[] = [];

        const formArray = this.getIndicatorBarWidgetColumnFormArray();
        formArray.controls.forEach(control => {
            const value = control.get('code').value;
            if (!columns.find(x => x == value)) {
                // la clonne du formulaire n'existe plus 
                columnCodeListToRemove.push(value);
            } else {
                // la clolonne existe
                columnCodeListExisting.push(value);
            }
        });

        // DELETE :
        columnCodeListToRemove.forEach(element => {
            const index = formArray.controls.findIndex(x => x.value.code == element);
            if (index != -1) {
                formArray.removeAt(index);
            }
        });

        // ADD :
        columns.forEach(element => {
            if (!columnCodeListExisting.find(x => x == element)) {
                const indicatorBarWidgetColumn = new IndicatorBarWidgetColumn();
                indicatorBarWidgetColumn.code = element;
                this.addIndicatorBarWidgetColumn(formArray, indicatorBarWidgetColumn);
            }
        });

        this.initializeCodeColumns(result);
    }

    private addIndicatorBarWidgetColumn(formArray: FormArray, element: IndicatorBarWidgetColumn) {
        formArray.push(this.widgetService.getIndicatorBarWidgetColumnFormGroup(element));
    }

    //----------------
    // BarLabelWidget 
    //----------------

    getBarLabelWidgetFormArray() {
        return this.indicatorBarWidget.get('barLabelWidgetList') as FormArray;
    }

    onAddBarLabelWidget() {
        const barLabelWidget = new BarLabelWidget();
        const formArray = this.getBarLabelWidgetFormArray();
        this.addBarLabelWidget(formArray, barLabelWidget);
    }

    private addBarLabelWidget(formArray: FormArray, element: BarLabelWidget) {
        let indexOfLast = 0;
        if (formArray.length > 0) {
            indexOfLast = formArray.at(formArray.length - 1).value.sequence;
        }
        formArray.push(new FormGroup({
            'id': new FormControl(element.id, [Validators.required]),
            'name': new FormControl(element.name),
            'sequence': new FormControl(indexOfLast + 1, [Validators.required]),
            'labelTextColor': new FormControl(element.labelTextColor, [Validators.required]),
            'useLabelColorForBar': new FormControl(element.useLabelColorForBar, [Validators.required]),
            'barLabelWidgetLocalizationList': new FormArray([]),
        }));

        this.selectedIndexColumn = formArray.length - 1;
    }

    onRemoveBarLabelWidget(index: number) {
        const controls = this.getBarLabelWidgetFormArray();
        controls.removeAt(index);
    }

    drop(event: CdkDragDrop<string[]>) {
        moveItemInArray(this.getBarLabelWidgetFormArray().controls, event.previousIndex, event.currentIndex);

        this.setSequencesOnBarLabelWidgetList();
    }

    private setSequencesOnBarLabelWidgetList() {
        let columnIndex = 1;
        const formArray = this.getBarLabelWidgetFormArray();
        formArray.controls.forEach(element => {
            element.patchValue({
                sequence: columnIndex,
            });
            columnIndex++;
        });
    }

    setSelectedBarLabelWidgetElement(index: number) {
        this.selectedIndexColumn = index;
    }

    getBarLabelWidgetLocalizationArray(index: number) {
        const formArray = this.getBarLabelWidgetFormArray();
        const columnElement = formArray.controls[index];
        return columnElement.get('barLabelWidgetLocalizationList') as FormArray;
    }

    getBarLabelWidgetLocalizationCount(index: number) {
        return this.getBarLabelWidgetLocalizationArray(index).length;
    }    
}
