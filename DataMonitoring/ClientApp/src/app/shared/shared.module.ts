import { NgModule, ModuleWithProviders } from "@angular/core";
import { CommonModule } from "@angular/common";
import { FormsModule } from "@angular/forms";
import { RouterModule } from "@angular/router";
import { MomentModule } from 'ngx-moment';

import { I18nModule } from "./i18n/i18n.module";
import { BootstrapModule } from "./bootstrap.module";
import { DataMonitoringLayoutModule } from "./layout/layout.module";
import { InlineGraphsModule } from "./graphs/inline/inline-graphs.module";
import { DataMonitoringFormModule } from './form/DataMonitoring.form.module';
import { WidgetsModule } from './widgets/widgets.module';
import {TitleLocalizationModule} from "./form/title-localization/title-localization.module";
import {InputModule} from "./form/input/input.module";
import {DynamicFormModule} from "./form/dynamic-form/dynamic-form.module";

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    RouterModule,
    BootstrapModule,
    MomentModule
  ],
  declarations: [],
  exports: [
    CommonModule,
    FormsModule,
    RouterModule,
    BootstrapModule,
    I18nModule,
    DataMonitoringLayoutModule,
    InlineGraphsModule,
    DataMonitoringFormModule,
    WidgetsModule,
    TitleLocalizationModule,
    InputModule,
    DynamicFormModule,
  ]
})
export class SharedModule { }
