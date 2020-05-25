import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxDatatableModule } from "@swimlane/ngx-datatable";
import { SharedModule } from "@app/shared/shared.module";
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DashboardListComponent } from './components/dashboard-list/dashboard-list.component';
import { DashboardEditComponent } from './components/dashboard-edit/dashboard-edit.component';
import { DashboardPublishComponent } from './components/dashboard-publish/dashboard-publish.component';
import { HttpClientModule } from "@angular/common/http";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { AccordionModule } from "ngx-bootstrap";
import { WidgetModule } from "@app/widget/widget.module";
import { NgxDnDModule } from '@swimlane/ngx-dnd';

@NgModule({
  declarations:
    [
      DashboardListComponent,
      DashboardEditComponent,
      DashboardPublishComponent
    ],
  imports: [
    CommonModule,
    DashboardRoutingModule,
    SharedModule,
    NgxDatatableModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    WidgetModule,
    NgxDnDModule, // BRY ngx-dnd v5.1.0
    AccordionModule.forRoot(),
  ]
})
export class DashboardModule { }
