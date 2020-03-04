import { Component, OnInit, Input } from '@angular/core';
import {FormArray, FormGroup, FormBuilder, Validators} from "@angular/forms";

import {I18nService} from "@app/shared/i18n/i18n.service";


@Component({
  selector: 'app-title-localization-list',
  templateUrl: './title-localization-list.component.html'
})
export class TitleLocalizationListComponent implements OnInit {

  @Input() titleLocalizations: any;
  @Input() count: number = 0;

  constructor(private i18nService: I18nService, private formBuilder: FormBuilder) { }

  ngOnInit() {
  }

  onAddTitleLocalization() {
    this.titleLocalizations.push(this.createEmptyTitleLocalizationGroup());
    this.count = this.titleLocalizations.length;
  }

  createEmptyTitleLocalizationGroup() {
    return this.formBuilder.group({
      id: [0],
      localizationCode: ['', [Validators.required]],
      title: ['', [Validators.required, Validators.maxLength(200)]]
    });
  }

  onRemoveTitleLocalization(i: number) {
    this.titleLocalizations.removeAt(i);
    this.count = this.titleLocalizations.length;
  }
}
