import {CommonModule} from "@angular/common";
import {FormsModule} from "@angular/forms";
import {NgModule} from "@angular/core";
import {PopoverModule} from "ngx-popover";
import {BsDropdownModule} from "ngx-bootstrap";
import {AvatarModule} from "ngx-avatar";

import {CollapseMenuComponent} from "./collapse-menu/collapse-menu.component";
import {FullScreenComponent} from "./full-screen/full-screen.component";
import { HeaderComponent } from "./header.component";
import {I18nModule} from "@app/shared/i18n/i18n.module";
import {UserModule} from "@app/shared/layout/user/user.module";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    BsDropdownModule,
    PopoverModule,
    I18nModule,
    UserModule,
    AvatarModule
  ],
  declarations: [
    FullScreenComponent,
    CollapseMenuComponent,
    HeaderComponent,
  ],
  exports: [
    HeaderComponent,
    FullScreenComponent,
  ]
})
export class HeaderModule{}
