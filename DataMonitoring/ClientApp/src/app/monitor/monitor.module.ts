import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from "@app/shared/shared.module"

import { MonitorRoutingModule } from './monitor-routing.module'
import { MonitorComponent } from "./components/monitor.component"
import { WidgetModule } from '../widget/widget.module'


@NgModule({
  declarations:
    [
      MonitorComponent
    ],
  imports: [
    CommonModule,
    MonitorRoutingModule,
    SharedModule,
    WidgetModule
  ]
})
export class MonitorModule { }
