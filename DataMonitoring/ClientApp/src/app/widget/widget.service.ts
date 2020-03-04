import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';

import { JsonApiService } from '@app/core/services';
import { Widget } from './models/widget';
import { BaseService } from "@app/setting/base.service";
import { TargetIndicatorChartWidget } from "./models/indicator-chart-widget";
import { FormGroup, FormControl, Validators, FormArray } from "@angular/forms";
import { TimeManagementService } from "@app/setting/time-management/time-management-service";
import { IndicatorBarWidgetColumn } from "./models/indicator-bar-widget";
import { TableWidgetColumn } from "./models/indicator-table-widget";

@Injectable({
    providedIn: 'root'
})
export class WidgetsService extends BaseService<Widget> {

    timeManagementSubject = new Subject<number>();
    changeTimeManagement(id: number) {
        this.timeManagementSubject.next(id);
    }

    constructor(jsonApiService: JsonApiService, private timeManagementService: TimeManagementService) {
        super(jsonApiService, 'widget');
    }

    getWidgetTypeList(): Observable<any[]> {
        return this.jsonApiService.getAll(this.actionUrl + '/type');
    }

    getColumnStyleList(): Observable<any[]> {
        return this.jsonApiService.getAll(this.actionUrl + '/columnStyle');
    }

    getColumnTypeList(): Observable<any[]> {
        return this.jsonApiService.getAll(this.actionUrl + '/columnType');
    }

    getTimeManagementList(): Observable<any[]> {
        return this.jsonApiService.getAll('api/TimeManagement');
    }

    duplicateWidget(id: number): Observable<any[]> {
        return this.jsonApiService.duplicate(this.actionUrl + '/duplicateWidget', id);
    }

    getAlignStyleList(): Observable<any[]> {
        return this.jsonApiService.getAll(this.actionUrl + '/alignStyle');
    }


    // Shared FormGroup
    getTargetIndicatorChartWidgetFormGroup(element: TargetIndicatorChartWidget) {
        return new FormGroup({
            'id': new FormControl(element.id, [Validators.required]),
            'startDate': new FormControl(element.startDateUtc != null ? this.timeManagementService.getFormatedHour(element.startDateUtc) : '08:00', [Validators.required]),
            'startTargetValue': new FormControl(element.startTargetValue, [Validators.required]),
            'endDate': new FormControl(element.endDateUtc != null ? this.timeManagementService.getFormatedHour(element.endDateUtc) : '17:00', [Validators.required]),
            'endTargetValue': new FormControl(element.endTargetValue, [Validators.required]),
        });
    }

    getIndicatorBarWidgetColumnFormGroup(element: IndicatorBarWidgetColumn) {
        return new FormGroup({
            'id': new FormControl(element.id, [Validators.required]),
            'code': new FormControl(element.code, [Validators.required]),
            'filtered': new FormControl(element.filtered, [Validators.required]),
            'filteredValue': new FormControl(element.filteredValue),
            'isNumericFormat': new FormControl(element.isNumericFormat, [Validators.required]),
        });
    }

    getTableWidgetColumnFormGroup(element: TableWidgetColumn, indexOfLast: number = null, columnNameLocalizationsFormArray: FormArray = null) {
        return new FormGroup({
            'id': new FormControl(element.id, [Validators.required]),
            'type': new FormControl(element.type, [Validators.required]),
            'sequence': new FormControl(indexOfLast != null ? indexOfLast + 1 : element.sequence, [Validators.required]),
            'code': new FormControl(element.code, [Validators.required]),
            'displayed': new FormControl(element.displayed, [Validators.required]),
            'name': new FormControl(element.name),
            'nameDisplayed': new FormControl(element.nameDisplayed, [Validators.required]),
            'columnStyle': new FormControl(element.columnStyle, [Validators.required]),
            'textBodyColor': new FormControl(element.textBodyColor, [Validators.required]),
            'textHeaderColor': new FormControl(element.textHeaderColor, [Validators.required]),
            'decimalMask': new FormControl(element.decimalMask),
            'boldHeader': new FormControl(element.boldHeader),
            'boldBody': new FormControl(element.boldBody),
            'alignStyle': new FormControl(element.alignStyle),
            // Style LowerValue
            'cellStyleWhenLowerValue': new FormControl(element.cellStyleWhenLowerValue),
            'lowerValue': new FormControl(element.lowerValue),
            'lowerColumnCode': new FormControl(element.lowerColumnCode),
            // Style HigherValue
            'cellStyleWhenHigherValue': new FormControl(element.cellStyleWhenHigherValue),
            'higherValue': new FormControl(element.higherValue),
            'higherColumnCode': new FormControl(element.higherColumnCode),
            // Style EqualValue 1
            'cellStyleWhenEqualValue1': new FormControl(element.cellStyleWhenEqualValue1),
            'equalsValue1': new FormControl(element.equalsValue1),
            'equalsColumnCode1': new FormControl(element.equalsColumnCode1),
            // Style EqualValue 2
            'cellStyleWhenEqualValue2': new FormControl(element.cellStyleWhenEqualValue2),
            'equalsValue2': new FormControl(element.equalsValue2),
            'equalsColumnCode2': new FormControl(element.equalsColumnCode2),
            // Style EqualValue 3
            'cellStyleWhenEqualValue3': new FormControl(element.cellStyleWhenEqualValue3),
            'equalsValue3': new FormControl(element.equalsValue3),
            'equalsColumnCode3': new FormControl(element.equalsColumnCode3),

            // IndicatorTableWidgetColumn :
            'filtered': new FormControl(element.filtered),
            'filteredValue': new FormControl(element.filteredValue),
            'isNumericFormat': new FormControl(element.isNumericFormat),
            'transpositionColumn': new FormControl(element.transpositionColumn),
            'transpositionValue': new FormControl(element.transpositionValue),
            'transpositionRow': new FormControl(element.transpositionRow),

            // MaskTableWidgetColumn :
            'displayModel': new FormControl(element.displayModel),

            // CalculatedTableWidgetColumn :
            'partialValueColumn': new FormControl(element.partialValueColumn),
            'totalValueColumn': new FormControl(element.totalValueColumn),

            // Translations
            'columnNameLocalizations': columnNameLocalizationsFormArray != null ? columnNameLocalizationsFormArray : new FormArray([]),
        });
    }
}
