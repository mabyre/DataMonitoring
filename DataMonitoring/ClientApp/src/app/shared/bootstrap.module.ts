import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { PopoverModule } from "ngx-popover";

import {
  ModalModule,
  ButtonsModule,
  TooltipModule,
  BsDropdownModule,
  ProgressbarModule,
  AlertModule,
  TabsModule,
} from "ngx-bootstrap";

@
NgModule({
  imports: [
    CommonModule,
    ModalModule.forRoot(),
    ButtonsModule.forRoot(),
    TooltipModule.forRoot(),
    BsDropdownModule.forRoot(),
    ProgressbarModule.forRoot(),
    AlertModule.forRoot(),
    TabsModule.forRoot()
  ],
  exports: [
    PopoverModule,
    ModalModule,
    ButtonsModule,
    TooltipModule,
    BsDropdownModule,
    ProgressbarModule,
    AlertModule,
    TabsModule
  ],
  declarations: []
})
export class BootstrapModule {}
