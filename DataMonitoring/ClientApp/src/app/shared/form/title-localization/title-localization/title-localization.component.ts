import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from "@angular/forms";

import { I18nService } from "@app/shared/i18n/i18n.service";
import { Language } from "@app/shared/i18n/language";
import {TitleLocalization} from "../title-localization";
import {Configuration} from "@app/core/configuration";

@Component({
  selector: 'app-title-localization',
  templateUrl: './title-localization.component.html'
})
export class TitleLocalizationComponent implements OnInit {

  @Input() titleLocalization: FormGroup;

  public languages: Language[];
  public selectedLanguage: Language;

  constructor(private i18nService: I18nService, private configuration: Configuration) { }

  ngOnInit() {
    // récupération de la liste des langues supportées
    this.languages = this.configuration.languages;
    this.initializeSelectedLanguage(<TitleLocalization> this.titleLocalization.value);
  }

  setSelectedLanguage(culture) {
    const language = this.languages.find(x => x.culture == culture);
    this.selectedLanguage = language;
  }

  initializeSelectedLanguage(titleLocalizationForm: TitleLocalization) {
    if (titleLocalizationForm.localizationCode == "") {
      this.setSelectedLanguage("en");
      this.titleLocalization.patchValue({
        localizationCode: this.selectedLanguage.culture
      });
    } else {
      this.setSelectedLanguage(titleLocalizationForm.localizationCode);
    }
  }

}
