import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';

import {IndicatorService} from "@app/setting/indicator/indicator.service";
import {Connector} from "@app/setting/connector/connector";
import {QueryConnector} from "../../indicator";

@Component({
  moduleId: module.id,
  selector: 'app-query-connector',
  templateUrl: './query-connector.component.html'
})
export class QueryConnectorComponent implements OnInit {

  @Input() queryConnector: FormGroup;
  @Input() connectorList: Connector[];
  @Input() index: number;
  @Input() indicatorType: number;

  public errorMessage: string;

  rows = [];
  columns = [];
  loadingIndicator: boolean = true;

  constructor(private indicatorsService: IndicatorService) { }

  ngOnInit() {
  }

  onPreview() {
    this.loadingIndicator = true;
    this.errorMessage = null;
    const formValue = this.queryConnector.value;
    const queryConnectorForm: QueryConnector = formValue;

    console.log(queryConnectorForm);

    this.errorMessage = this.indicatorsService.IsValidQuery(queryConnectorForm.query);
    if (this.errorMessage == null) {
      this.indicatorsService.getQueryPreview(queryConnectorForm)
      .subscribe(result => {
        this.initialize(result);
      }, error => {
        this.errorMessage = error;
      });
    }
  }

  private initialize(data: any) {
    
    this.columns = [];
    this.rows = [];

    if (data == null) {
      return;
    }

    this.indicatorsService.initializeColumns(this.columns, data);
    this.rows = data;
    this.loadingIndicator = false;

    if (this.indicatorType == 1) { // si de type Flow
      this.errorMessage = this.indicatorsService.IsFlowValidColumns(this.columns);
    }
  }

  
}
