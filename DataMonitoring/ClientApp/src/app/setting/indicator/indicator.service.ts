import { Injectable } from '@angular/core';

import {BaseService} from "../base.service";
import {JsonApiService} from "@app/core/services";
import {Indicator, QueryConnector} from "./indicator";
import {I18nService} from "@app/shared/i18n/i18n.service";
import {Observable} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class IndicatorService extends BaseService<Indicator> {
  
  constructor(jsonApiService: JsonApiService, private i18nService: I18nService) {
    super(jsonApiService, 'IndicatorDefinition');
  }

  public getIndicatorTypes(): Observable<any[]> {
    return this.jsonApiService.getAll(this.actionUrl + '/indicatorTypes');
  }

  getQueryPreview(queryConnector: QueryConnector) {
    return this.jsonApiService.add(this.actionUrl + '/GetQueryPreview', queryConnector);
  }

  getIndicatorQueryColumns(indicatorId: number) {
    return this.jsonApiService.add(this.actionUrl + '/GetIndicatorQueryColumns', indicatorId);
  }

  IsValidQuery(value: string): string {
    if (value.toUpperCase().includes("DELETE")) {
      return this.i18nService.getTranslation('DeleteQueryNotAllowed');
    } else if (value.toUpperCase().includes("UPDATE")) {
      return this.i18nService.getTranslation('UpdateQueryNotAllowed');
    } else if (value.toUpperCase().includes("SELECT TOP")) {
      return null;
    } else if (!(value.includes("%%localDate%%") || value.includes("%%utcDate%%"))) {
      return this.i18nService.getTranslation('DateQueryMissing');
    } else {
      return null;
    }
  }

  IsFlowValidColumns(columns: any[]): any {

    let group1Exist = false;
    let valueExist = false;
    let anyColumnInError = false;
    columns.forEach(element => {
      if (element.prop == "GROUP1") {
        group1Exist = true;
      } else if (element.prop == "VALUE") {
        valueExist = true;
      } else if (element.prop != "GROUP2" && element.prop != "GROUP3" && element.prop != "GROUP4" && element.prop != "GROUP5") {
        anyColumnInError = true;
      }
    });

    if (!group1Exist) {
      return this.i18nService.getTranslation('Group1Missing');
    } else if (!valueExist) {
      return this.i18nService.getTranslation('ValueMissing');
    } else if (anyColumnInError) {
      return this.i18nService.getTranslation('WrongGroupColumnName');
    } else {
      return null;
    }
  }

  initializeColumns(columns: any[], data: any) {
    if (data.length > 0) {
      const columnsIn = data[0];
      for (const key in columnsIn) {
        if (columnsIn.hasOwnProperty(key)) {
          columns.push(new Object({"prop": key}));
        }
      }
    }
  }
}
