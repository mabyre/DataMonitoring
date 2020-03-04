import { Injectable } from '@angular/core';
import { OpenIDImplicitFlowConfiguration, AuthWellKnownEndpoints } from 'angular-auth-oidc-client';
import { Language } from "@app/shared/i18n/language";

@Injectable({
    providedIn: 'root',
})
export class Configuration {

    public applicationName: string;
    public applicationScope: string;
    public defaultLocale: string = 'us';
    public apiServerUrl: string = null;
    public apiTimeout: number = 100;
    public apiRetry: number = 1;
    public headerContentType: string = 'application/json';
    public headerAccept: string = 'application/json';
    public autorityServerActif: boolean = true;
    public waitIntervalMonitor: number = 30;

    public OpenIdConfiguration: OpenIDImplicitFlowConfiguration;
    public WellKnownEndpoints: AuthWellKnownEndpoints;

    public defaultSkin: string = "sodevlog-style-0";
    public skins: SkinSetting[];

    public languages: Language[];
}

export class SkinSetting {

    name: string;
    logo: string;
    label: string;
}
