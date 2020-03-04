// slice
// https://www.tutorialspoint.com/typescript/typescript_array_slice.htm
//
import { Injectable } from '@angular/core';
import { BaseService } from "../base.service";
import { JsonApiService } from "@app/core/services";
import { TimeManagement } from "./time-management";
import { I18nService } from "@app/shared/i18n/i18n.service";
import { Observable } from "rxjs";

@Injectable({
    providedIn: 'root'
})
export class TimeManagementService extends BaseService<TimeManagement> {

    constructor(jsonApiService: JsonApiService, private i18nService: I18nService) {
        super(jsonApiService, 'TimeManagement');
    }

    getTimeManagementTypes(): Observable<any[]> {
        return this.jsonApiService.getAll(this.actionUrl + '/timeManagementTypes');
    }

    getUnitOfTimes(): Observable<any[]> {
        return this.jsonApiService.getAll(this.actionUrl + '/unitOfTimes');
    }

    getFormatedHour(date: Date): string {
        const hours = new Date(date).getHours();
        const minutes = new Date(date).getMinutes();

        const localTime = hours + ':' + minutes;

        console.log(JSON.stringify("localTime : " + localTime));
        return localTime;
    }

    getUtcDate(date: string): Date {
        const dateStr2Array = date.split(":");
        const hours = +dateStr2Array.slice(0, 1); // + : convert in int
        const minutes = +dateStr2Array.slice(1, 2);

        const dateNew = new Date();
        dateNew.setHours(hours);
        dateNew.setMinutes(minutes);
        dateNew.setSeconds(0);
        dateNew.setMilliseconds(0);

        console.log(JSON.stringify("localTime : " + dateNew));
        return dateNew;
    }
}

