import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {HttpClientModule} from "@angular/common/http";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";

import { NgxDatatableModule } from "@swimlane/ngx-datatable";
import { AutosizeModule } from 'ngx-autosize';
import {AccordionModule} from "ngx-bootstrap";

import { IndicatorsListComponent } from './components/indicators-list/indicators-list.component';
import { IndicatorEditComponent } from './components/indicator-edit/indicator-edit.component';
import { IndicatorCalculatedComponent } from './components/indicator-calculated/indicator-calculated.component';
import {IndicatorRoutingModule} from "./indicator-routing.module";
import {SharedModule} from "@app/shared/shared.module";
import {QueryConnectorComponent} from "./components/query-connector/query-connector.component";

@NgModule({
  declarations: [
    IndicatorsListComponent,
    IndicatorEditComponent,
    IndicatorCalculatedComponent,
    QueryConnectorComponent,
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AccordionModule.forRoot(),
    SharedModule,
    NgxDatatableModule,
    AutosizeModule,
    IndicatorRoutingModule,
  ]
})
export class IndicatorModule { }
