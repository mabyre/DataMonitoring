import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";

import { NgxSpinnerModule } from 'ngx-spinner';

import { ConnectorsListComponent } from './components/connectors-list/connectors-list.component';
import { ConnectorEditComponent } from './components/connector-edit/connector-edit.component';
import { ConnectorRoutingModule } from "./connector-routing.module";
import { SharedModule } from "@app/shared/shared.module";

@NgModule({
  declarations: [
    ConnectorsListComponent,
    ConnectorEditComponent,
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    ConnectorRoutingModule,
    NgxSpinnerModule,
  ]
})
export class ConnectorModule { }
