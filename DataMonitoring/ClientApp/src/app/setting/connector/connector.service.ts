import { Injectable } from '@angular/core';

import { BaseService } from "../base.service";
import { JsonApiService } from "@app/core/services";
import { Connector } from "./connector";
import { Observable } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class ConnectorService extends BaseService<Connector> {

  constructor(jsonApiService: JsonApiService) {
    super(jsonApiService, 'Connector');
  }

  testConnection(connector: Connector) {
    return this.jsonApiService.add(this.actionUrl + '/TestConnection', connector);
  }

  public getConnectorTypes(): Observable<any[]> {
    return this.jsonApiService.getAll(this.actionUrl + '/connectorTypes');
  }

  public getApiAutorisationTypes(): Observable<any[]> {
    return this.jsonApiService.getAll(this.actionUrl + '/autorisationTypes');
  }

  public getApiGrantTypes(): Observable<any[]> {
    return this.jsonApiService.getAll(this.actionUrl + '/grantTypes');
  }

  public getHttpMethods(): any[] {
    const grantTypes: any[] = [
      { 'value': 'GET' },
      { 'value': 'POST' },
      { 'value': 'PUT' },
      { 'value': 'DELETE' },
    ];
    return grantTypes;
  }
}

