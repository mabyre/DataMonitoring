import { NgModule, ModuleWithProviders } from "@angular/core";
import { CommonModule } from "@angular/common";
import { AvatarModule } from "ngx-avatar";

import { LoginComponent } from "./login/login.component";
import { LoginInfoComponent } from "./login-info/login-info.component";
import { I18nModule } from '@app/shared/i18n/i18n.module';

@NgModule({
    imports: [CommonModule, I18nModule, AvatarModule],
  declarations: [LoginInfoComponent, LoginComponent],
  exports: [LoginInfoComponent, LoginComponent]
})
export class UserModule { }
