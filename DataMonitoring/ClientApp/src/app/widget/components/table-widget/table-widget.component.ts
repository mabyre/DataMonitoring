import { Component, OnInit, Input, OnChanges } from '@angular/core';
import { Indicator } from "@app/setting/indicator/indicator";
import { FormGroup, FormArray, FormControl, Validators } from "@angular/forms";
import { Color } from "@app/shared/models/color";
import { Style } from "@app/shared/models/style";
import { CdkDragDrop, moveItemInArray } from "@angular/cdk/drag-drop";
import { IndicatorService } from "@app/setting/indicator/indicator.service";
import { TableWidgetColumn } from "@app/widget/models/indicator-table-widget";
import { WidgetsService } from "@app/widget/widget.service";

@Component({
    selector: 'app-table-widget',
    templateUrl: './table-widget.component.html',
    styleUrls: ['../cdk-drag-drop.css'],
})
export class TableWidgetComponent implements OnInit, OnChanges {

  @Input() tableWidget: FormGroup;
  @Input() colorList: Color[];
  @Input() styleList: Style[];
  @Input() columnStyleList: any[];
  @Input() columnTypeList: any[];
  @Input() aligneStyleList: any[];
  @Input() index: number;

  public indicatorList: Indicator[];
  public selectedIndexColumn: number = 0;
  public errorMessage: string;
  public columns: string[] = [];
  public dataColumnList: string[] = [];
  public dataColumnWithTranspositionColumnList: string[] = [];
  
  constructor(private indicatorsService: IndicatorService, private widgetService: WidgetsService) { }

  ngOnInit() {

    this.indicatorsService.get()
      .subscribe(result => {
        // Uniquement des Indicateurs de type IndicatorType.Snapshot
        this.indicatorList = result.filter(x => x.type == 0);
      }, error => {
        this.errorMessage = error;
      });

    this.tableWidget.patchValue({
      sequence: this.index,
    });

    this.onIndicatorChange();
  }

  ngOnChanges(changes: any) {
    this.tableWidget.patchValue({
      sequence: this.index,
    });
  }

  // ////////////////////////////////////////////////////////////////////////////////////////////////////////
  // TABLE WIDGET COLUMN 
  // ////////////////////////////////////////////////////////////////////////////////////////////////////////
  drop(event: CdkDragDrop<string[]>) {
    moveItemInArray(this.getTableWidgetColumnList().controls, event.previousIndex, event.currentIndex);

    this.setSequencesOnRableWidgetColumnList();
  }

  private setSequencesOnRableWidgetColumnList() {
    let columnIndex = 1;
    const formArray = this.getTableWidgetColumnList();
    formArray.controls.forEach(element => {
      element.patchValue({
        sequence: columnIndex,
      });
      columnIndex++;
    });
  }

  getTableWidgetColumnList() {
    return this.tableWidget.get('tableWidgetColumnList') as FormArray;
  }

  onRemoveTableWidgetColumn(index: number) {
    const formArray = this.getTableWidgetColumnList();
    formArray.removeAt(index);

    this.setSequencesOnRableWidgetColumnList();
  }

  onAddTableWidgetColumn() {
    const tableWidgetColumn = new TableWidgetColumn();
    const formArray = this.getTableWidgetColumnList();
    this.addTableWidgetColumn(formArray, tableWidgetColumn);
  }

  setSelectedTableWidgetColumnElement(index: number) {
    this.selectedIndexColumn = index;
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
      this.dataColumnList.push("TARGET");
    }

    this.addCodeColumnsWhenFormExist();
  }

  addCodeColumnsWhenFormExist() {
    this.dataColumnWithTranspositionColumnList = [];
    this.dataColumnWithTranspositionColumnList.push("");
    const formArray = this.getTableWidgetColumnList();
    formArray.controls.forEach(control => {
      if (control.get('type').value == 3 || control.get('type').value == 0) { // colonnes de type Transposition OU Indicator
        this.dataColumnWithTranspositionColumnList.push(control.get('code').value);
      }
    });
  }

  onRefreshColumns() {
    this.errorMessage = null;
    const indicatorId = this.tableWidget.get('indicatorId').value;
    this.indicatorsService.getIndicatorQueryColumns(+indicatorId)
      .subscribe(result => {
        this.initializeIndicatorDataColumns(result);
      }, error => {
        this.errorMessage = error;
      });
  }

  initializeIndicatorDataColumns(result: any): any {
    this.columns = [];
    if (result.length > 0) {
      const columnsIn = result[0];
      for (const key in columnsIn) {
        if (columnsIn.hasOwnProperty(key)) {
          this.columns.push(key);
        }
      }
    }

    const columnSequenceListToRemove: number[] = [];
    const columnCodeListExisting: string[] = [];

    const formArray = this.getTableWidgetColumnList();
    formArray.controls.forEach(control => {
      if (control.get('type').value == 0) { // colonnes de type IndicatorData
        if (!this.columns.find(x => x == control.get('code').value)) {
          // la clonne de type IndicatoData du formulaire n'existe plus 
          columnSequenceListToRemove.push(control.get('sequence').value);
        } else {
          // la clolonne existe
          columnCodeListExisting.push(control.get('code').value);
        }
      }
    });

    // DELETE :
    columnSequenceListToRemove.forEach(element => {
      formArray.removeAt(element - 1);
    });
    
    // ADD :
    this.columns.forEach(element => {
      if (!columnCodeListExisting.find(x => x == element)) {
        const tableWidgetColumn = new TableWidgetColumn();
        tableWidgetColumn.code = element;
        tableWidgetColumn.type = 0;
        this.addTableWidgetColumn(formArray, tableWidgetColumn);
      }
    });

    this.initializeCodeColumns(result);
  }

  private addTableWidgetColumn(formArray: FormArray, tableWidgetColumn: TableWidgetColumn) {
    let indexOfLast = 0;
    if (formArray.length > 0) {
      indexOfLast = formArray.at(formArray.length - 1).value.sequence;
    }
    formArray.push(this.widgetService.getTableWidgetColumnFormGroup(tableWidgetColumn, indexOfLast, null));

    this.selectedIndexColumn = formArray.length - 1;
  }

  onChange(index: number) {
    const formArray = this.getTableWidgetColumnList();
    const columnElement = formArray.controls[index];
    if (columnElement.get('type').value == 2) {
      // Colonne de type TARGET
      columnElement.patchValue({
        code: "TARGET",
      });
    } else if (columnElement.get('type').value == 3) {
      // Colonne de type TRANSPOSITION
      columnElement.patchValue({
        code: "",
      });
    } else {
      const columnTypeName = this.columnTypeList.find(x => x.value == columnElement.get('type').value).name;
      columnElement.patchValue({
        code: columnTypeName + '_' + Math.floor(Math.random() * 100) + 1,
      });
    }
  }

  onIndicatorChange() {
    const indicatorId = this.tableWidget.get('indicatorId').value;
    if (indicatorId != null && +indicatorId != 0) {
      this.indicatorsService.getIndicatorQueryColumns(+indicatorId)
        .subscribe(result => {
          this.initializeCodeColumns(result);
        }, error => {
          this.errorMessage = error;
        });
    }
  }

  onTranspositionColumnChecked(index: number) {
    const formArray = this.getTableWidgetColumnList();
    const columnElement = formArray.controls[index];
    columnElement.patchValue({
      transpositionValue: false,
      transpositionRow: false
    });
  }

  onTranspositionValueChecked(index: number) {
    const formArray = this.getTableWidgetColumnList();
    const columnElement = formArray.controls[index];
    columnElement.patchValue({
      transpositionColumn: false,
      transpositionRow: false
    });
  }

  onTranspositionRowChecked(index: number) {
    const formArray = this.getTableWidgetColumnList();
    const columnElement = formArray.controls[index];
    columnElement.patchValue({
      transpositionColumn: false,
      transpositionValue: false
    });
  }

  onTranspositionCodeColumnChange() {
    this.addCodeColumnsWhenFormExist();
  }

  getColumnNameLocalizationsArray(index: number) {
    const formArray = this.getTableWidgetColumnList();
    const columnElement = formArray.controls[index];
    return columnElement.get('columnNameLocalizations') as FormArray;
  }

  getColumnLocalizationCount(index: number) {
    return this.getColumnNameLocalizationsArray(index).length;
  }
  // ////////////////////////////////////////////////////////////////////////////////////////////////////////
}
