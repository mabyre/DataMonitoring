import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '@app/shared/shared.module';
import { Error404RoutingModule } from './error404-routing.module';
import { Error404Component } from './error404.component';

@NgModule({
  imports: [
    CommonModule,
    Error404RoutingModule,
    SharedModule
  ],
  declarations: [Error404Component]
})
export class Error404Module { }
