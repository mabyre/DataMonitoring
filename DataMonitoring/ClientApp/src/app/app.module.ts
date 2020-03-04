import { NgModule, APP_INITIALIZER } from '@angular/core';
import { CommonModule } from "@angular/common";
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AuthModule, OidcSecurityService } from 'angular-auth-oidc-client';

import { AuthorizationCanGuard, AuthorizationGuard } from "@app/core/auth";
import { Configuration } from '@app/core/configuration';
import { AppConfigurationService } from '@app/core/services';
import { SharedModule } from "@app/shared/shared.module";
import { LayoutService } from '@app/shared/layout/layout.service';

import { AppComponent } from './app.component';
import { AppRoutingModule } from './app-routing.module';

import { HomeComponent } from './home/home.component';

export function initApp(appConfigService: AppConfigurationService) {
  return () => appConfigService.initializeApp();
}

export function getSettings(appConfigService: AppConfigurationService) {

  let address;
  if (!window.location.origin) {
    address = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port: '');
  }
  else
  {
    address = window.location.origin;
  }

  return () => appConfigService.getSettings(`${address}/api/clientappsettings`);
}

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    CommonModule,
    FormsModule,
    SharedModule,
    AuthModule.forRoot()
  ],
  providers: [
    AppConfigurationService,
    { provide: APP_INITIALIZER, useFactory: initApp, deps: [AppConfigurationService], multi: true },
    { provide: APP_INITIALIZER, useFactory: getSettings, deps: [AppConfigurationService], multi: true },
    OidcSecurityService,
    Configuration,
    AuthorizationGuard,
    AuthorizationCanGuard
  ],
  bootstrap: [AppComponent]
})

export class AppModule {

  constructor(
    private oidcSecurityService: OidcSecurityService,
    private configurationservice: AppConfigurationService,
    configuration: Configuration) {

    this.configurationservice.onConfigurationLoaded.subscribe(() => {
      if (configuration.OpenIdConfiguration != null) {
        this.oidcSecurityService.setupModule(configuration.OpenIdConfiguration, configuration.WellKnownEndpoints);
      }
    });

    console.log("AppModule starting");

  }
}
