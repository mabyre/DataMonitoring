import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormsModule, ReactiveFormsModule} from "@angular/forms";

import { TitleLocalizationComponent } from "./title-localization/title-localization.component";
import { TitleLocalizationListComponent } from './title-localization-list/title-localization-list.component';
import {I18nModule} from "@app/shared/i18n/i18n.module";
import {AccordionModule} from "ngx-bootstrap";


@NgModule({
  declarations: [
    TitleLocalizationComponent,
    TitleLocalizationListComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    I18nModule,
    AccordionModule.forRoot(),
  ],
  exports: [
    TitleLocalizationListComponent
  ],
})
export class TitleLocalizationModule { }
