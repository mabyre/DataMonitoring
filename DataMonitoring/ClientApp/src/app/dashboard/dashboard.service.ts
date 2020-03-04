import { Injectable } from '@angular/core';
import {BaseService} from "@app/setting/base.service";
import {Dashboard} from "./dashboard";
import {JsonApiService} from "@app/core/services";


@Injectable({
  providedIn: 'root'
})
export class DashboardService extends BaseService<Dashboard> {

  constructor(jsonApiService: JsonApiService) {
    super(jsonApiService, 'Dashboard');
  }

}
