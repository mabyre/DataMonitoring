import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '@app/shared/shared.module';
import { Error500RoutingModule } from './error500-routing.module';
import { Error500Component } from './error500.component';

@NgModule({
  imports: [
    CommonModule,
    Error500RoutingModule,
    SharedModule
  ],

  declarations: [Error500Component]
})
export class Error500Module { }
