import { Injectable } from '@angular/core';
import { BaseService } from "@app/setting/base.service";
import { DashboardLight } from "./dashboard-light";
import { JsonApiService } from "@app/core/services";

@Injectable({
  providedIn: 'root'
})
export class DashboardLightService extends BaseService<DashboardLight> {
  constructor(jsonApiService: JsonApiService) {
    super(jsonApiService, 'DashboardLight');
  }
}
