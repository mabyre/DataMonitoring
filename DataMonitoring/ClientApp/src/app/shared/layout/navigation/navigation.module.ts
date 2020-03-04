import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";

import { UserModule } from "../user/user.module";
import { I18nModule } from "../../i18n/i18n.module";

import { BigBreadcrumbsComponent } from "./big-breadcrumbs.component";
import { MinifyMenuComponent } from "./minify-menu.component";
import { NavigationComponent } from "./navigation.component";
import { AppMenuDirective } from "./app-menu.directive";

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    I18nModule,
    UserModule
  ],
  declarations: [
    BigBreadcrumbsComponent,
    MinifyMenuComponent,
    NavigationComponent,
    AppMenuDirective
  ],
  exports: [
    BigBreadcrumbsComponent,
    MinifyMenuComponent,
    NavigationComponent,
    AppMenuDirective
  ]
})
export class NavigationModule { }
