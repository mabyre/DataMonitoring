import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormsModule } from "@angular/forms";
import { HttpClientModule } from "@angular/common/http";


import { SharedModule } from "@app/shared/shared.module";
import { StyleListComponent } from './components/style-list/style-list.component';
import { StyleEditComponent } from './components/style-edit/style-edit.component';
import {StyleRoutingModule} from "./style-routing.module";

@NgModule({
  declarations: [
  StyleListComponent,
  StyleEditComponent
],
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    StyleRoutingModule
  ]
})
export class StyleModule { }
