import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {ReactiveFormsModule, FormsModule} from "@angular/forms";
import {InputComponent} from "./components/input.component";
import {SubmitButtonComponent} from "./components/submit-button.component";
import {SelectComponent} from "./components/select.component";
import {CheckboxComponent} from "./components/checkbox.component";
import {DynamicFieldDirective} from "./dynamic-field.directive";
import {DynamicFormComponent} from "./dynamic-form.component";
import {TextareaComponent} from "./components/textarea.component";
import {AutosizeModule} from "ngx-autosize";
import { BackButtonComponent } from './components/back-button.component';
import {RouterModule} from "@angular/router";

@NgModule({
  imports: [
    CommonModule,
    ReactiveFormsModule,
    FormsModule,
    AutosizeModule,
    RouterModule,
  ],
  declarations: [
    InputComponent,
    SubmitButtonComponent,
    SelectComponent,
    CheckboxComponent,
    TextareaComponent,
    DynamicFieldDirective,
    DynamicFormComponent,
    BackButtonComponent
  ],
  entryComponents: [
    InputComponent,
    SubmitButtonComponent,
    SelectComponent,
    CheckboxComponent,
    TextareaComponent,
    BackButtonComponent,
  ],
  exports: [
    DynamicFormComponent
  ],
})
export class DynamicFormModule { }
