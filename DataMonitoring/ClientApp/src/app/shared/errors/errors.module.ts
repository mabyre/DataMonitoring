import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SharedModule } from '@app/shared/shared.module';
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { ForbiddenComponent } from "@app/shared/errors/forbidden/forbidden.component"

import { routing } from "./errors.routing";

@
NgModule({
  imports: [
    CommonModule,
    SharedModule,
    routing
  ],
  declarations: [
    UnauthorizedComponent,
    ForbiddenComponent
  ]
})
export class ErrorsModule { }
