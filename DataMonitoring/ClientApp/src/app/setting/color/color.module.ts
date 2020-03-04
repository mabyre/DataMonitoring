import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ColorEditComponent } from './components/color-edit/color-edit.component';
import { ColorListComponent } from './components/color-list/color-list.component';
import {HttpClientModule} from "@angular/common/http";
import {FormsModule, ReactiveFormsModule} from "@angular/forms";
import {SharedModule} from "@app/shared/shared.module";
import {ColorRoutingModule} from "./color-routing.module";

@NgModule({
  declarations: [
    ColorEditComponent, 
    ColorListComponent
  ],
  imports: [
    CommonModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    ColorRoutingModule
  ]
})
export class ColorModule { }
