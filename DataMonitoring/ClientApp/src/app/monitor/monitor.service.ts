import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { JsonApiService } from '@app/core/services';
import { Monitor } from './models/monitor';

@Injectable({
  providedIn: 'root'
})
export class MonitorService {

  constructor(private jsonApiService: JsonApiService) { }

    getMonitor(key: string): Observable<Monitor> {
        return this.jsonApiService.getById<Monitor>('api/monitor', key);
  }
}
