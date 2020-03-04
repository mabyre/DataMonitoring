import { Injectable, ApplicationRef } from '@angular/core';
import { Subject } from 'rxjs';
import { JsonApiService } from "@app/core/services/json.api.service";
import { Configuration } from "@app/core/configuration";
import { Language } from './language';

@Injectable({
  providedIn: 'root',
})
export class I18nService {

  public state;
  public data:{};
  public currentLanguage:any;

  constructor(private jsonApiService:JsonApiService, private ref:ApplicationRef, public configuration: Configuration) {
    this.state = new Subject();

    this.initLanguage();
  }

  private initLanguage() {
    this.initCurrentLanguage(localStorage.getItem('lang') || this.configuration.defaultLocale);
    this.fetch(this.currentLanguage.culture);
  }

  private fetch(locale: any) {
    this.jsonApiService.get(`/langs/${locale}.json`)
      .subscribe((data: any) => {
        this.data = data;
        this.state.next(data);
        this.ref.tick();
      });
  }

  private initCurrentLanguage(locale:string) {

    const language = this.configuration.languages.find((it)=> {
      return it.key === locale;
    });
    if (language) {
      this.setLanguage(language);
    } else {
      throw new Error(`Incorrect locale used for I18nService: ${locale}`);
    }
  }

  setLanguage(language:Language) {
    this.currentLanguage = language;
    localStorage.setItem('lang', language.key);
    localStorage.setItem('culture',language.culture);
    this.fetch(language.culture);
  }

  subscribe(sub:any, err:any) {
    return this.state.subscribe(sub, err);
  }

  public getTranslation(phrase:string):string {
    return this.data && this.data[phrase] ? this.data[phrase] : phrase;
  }
}
