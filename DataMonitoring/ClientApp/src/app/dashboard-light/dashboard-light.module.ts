import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from "@app/shared/shared.module";
import { HttpClientModule } from "@angular/common/http";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { DashboardLightRoutingModule } from './dashboard-light-routing.module';
import { DashboardLightEditComponent } from './components/dashboard-light-edit/dashboard-light-edit.component';

@NgModule({
  declarations: [
    DashboardLightEditComponent
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    SharedModule,
    FormsModule,
    ReactiveFormsModule,
    DashboardLightRoutingModule,
  ]
})
export class DashboardLightModule { }
