import { NgModule, ModuleWithProviders } from "@angular/core";
import { CommonModule } from "@angular/common";
import { AvatarModule } from "ngx-avatar";

import { LoginInfoComponent } from "./login-info/login-info.component";
import { LogoutComponent } from "./logout/logout.component";
import { I18nModule } from '@app/shared/i18n/i18n.module';

@NgModule({
    imports: [CommonModule, I18nModule, AvatarModule],
    declarations: [LoginInfoComponent, LogoutComponent],
    exports: [LoginInfoComponent, LogoutComponent]
})
export class UserModule { }
