import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import {AccordionModule} from "ngx-bootstrap";
import {DragDropModule} from '@angular/cdk/drag-drop';

import { NgxDatatableModule } from "@swimlane/ngx-datatable";

import { SharedModule } from "@app/shared/shared.module";

import { WidgetRoutingModule } from './widget-routing.module';
import { WidgetListComponent, WidgetEditComponent, WidgetComponent } from "@app/widget/components";
import { WidgetEditTlComponent } from "@app/widget/components";
import { TableWidgetComponent } from './components/table-widget/table-widget.component';
import { IndicatorBarWidgetComponent } from './components/indicator-bar-widget/indicator-bar-widget.component';
import { IndicatorChartWidgetComponent } from './components/indicator-chart-widget/indicator-chart-widget.component';
import { IndicatorGaugeWidgetComponent } from './components/indicator-gauge-widget/indicator-gauge-widget.component';

@NgModule({
  declarations:
    [
      WidgetListComponent,
      WidgetEditComponent,
      WidgetEditTlComponent,
      WidgetComponent,
      TableWidgetComponent,
      IndicatorBarWidgetComponent,
      IndicatorChartWidgetComponent,
      IndicatorGaugeWidgetComponent
    ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    WidgetRoutingModule,
    NgxDatatableModule,
    DragDropModule,
    AccordionModule.forRoot(),
    SharedModule,
  ],
  exports:[
    WidgetComponent
  ]
})
export class WidgetModule { }
