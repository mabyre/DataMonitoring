import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { JsonApiService } from './json.api.service';
import { TimeZone } from '../timeZone';
import * as moment from 'moment-timezone';


@Injectable({
  providedIn: 'root',
})
export class TimeZoneService {
 
  constructor(private jsonApiService:JsonApiService) { }

  public getCurrentTimeZone():Observable<TimeZone> {

      return this.jsonApiService.get('api/timeZone/current');
  }

  public getTimeZones(): Observable<TimeZone[]> {

    return this.jsonApiService.getAll<TimeZone>('api/timeZone');
  }
}