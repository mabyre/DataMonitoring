import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { TooltipModule, BsDropdownModule } from "ngx-bootstrap";
import { NgMarqueeModule } from 'ng-marquee';
import { I18nModule } from '@app/shared/i18n/i18n.module';
import { HeaderModule } from './header/header.module';
import { FooterComponent } from './footer/footer.component';
import { NavigationModule } from './navigation/navigation.module';
import { RibbonComponent } from './ribbon/ribbon.component';
import { ShortcutComponent } from './shortcut/shortcut.component';
import { RouteBreadcrumbsComponent } from './ribbon/route-breadcrumbs.component';
import { LayoutSwitcherComponent } from './layout-switcher.component';
import { MainLayoutComponent } from './app-layouts/main-layout.component';
import { EmptyLayoutComponent } from './app-layouts/empty-layout.component';

@NgModule({
    imports: [
      CommonModule,
      HeaderModule,
      NavigationModule,
      FormsModule,
      RouterModule,
      TooltipModule,
      BsDropdownModule,
      I18nModule,
      NgMarqueeModule
    ],
    declarations: [
      FooterComponent,      
      RibbonComponent,
      ShortcutComponent,
      MainLayoutComponent,
      EmptyLayoutComponent,
      RouteBreadcrumbsComponent,
      LayoutSwitcherComponent
    ],
    exports: [
      HeaderModule,
      NavigationModule,
      FooterComponent,
      RibbonComponent,
      ShortcutComponent,
      LayoutSwitcherComponent
    ]
  })
export class DataMonitoringLayoutModule { }
