import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {HttpClientModule} from "@angular/common/http";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";

import {AccordionModule} from "ngx-bootstrap";

import { TimeManagementListComponent } from './components/time-management-list/time-management-list.component';
import {TimeManagementRoutingModule} from "./time-management-routing.module";
import {SharedModule} from "@app/shared/shared.module";
import { TimeManagementEditComponent } from './components/time-management-edit/time-management-edit.component';
import { TimeRangeComponent } from './components/time-range/time-range.component';


@NgModule({
  declarations: [
    TimeManagementListComponent,
    TimeManagementEditComponent,
    TimeRangeComponent
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    AccordionModule.forRoot(),
    SharedModule,
    TimeManagementRoutingModule,
  ]
})
export class TimeManagementModule { }
