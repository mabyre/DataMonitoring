import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import {ClockpickerDirective} from './clockpicker.directive';
import {DisableControlDirective} from "./disabled-control-directive";
import {ColorpickerDirective} from './colorpicker.directive';

@NgModule({
  imports: [
    CommonModule
  ],
  declarations: [
    ClockpickerDirective,
    DisableControlDirective,
    ColorpickerDirective
  ],
  exports: [
    ClockpickerDirective,
    DisableControlDirective,
    ColorpickerDirective
  ]
})
export class InputModule { }
