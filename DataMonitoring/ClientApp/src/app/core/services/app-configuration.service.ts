import { EventEmitter, Injectable, Output } from "@angular/core";
import { HttpClient } from '@angular/common/http';
import { Configuration, SkinSetting } from '../configuration';
import { OpenIDImplicitFlowConfiguration, AuthWellKnownEndpoints } from "angular-auth-oidc-client";
import { Language } from "@app/shared/i18n/language";

@Injectable()
export class AppConfigurationService {

    @Output() onConfigurationLoaded = new EventEmitter<boolean>();

    constructor(private readonly httpClient: HttpClient, public configuration: Configuration) { }

    initializeApp(): Promise<any> {
        return new Promise((resolve, reject) => {
            setTimeout(() => {

                console.log('AppConfigurationService: inside setTimeout');

                resolve();
            }, 2000);
        });
    }

    async getSettings(configUrl: string): Promise<any> {

        console.log(`AppConfigurationService: getSettings from ${configUrl}`);

        try {
            const response = await fetch(configUrl);
            if (!response.ok) {
                throw new Error(response.statusText);
            }

            const clientConfiguration = await response.json();

            this.configuration.defaultLocale = clientConfiguration.defaultLocale;
            this.configuration.apiServerUrl = clientConfiguration.apiServerUrl;
            this.configuration.applicationName = clientConfiguration.applicationName;
            this.configuration.applicationScope = clientConfiguration.applicationScope;
            this.configuration.apiRetry = clientConfiguration.apiRetry;
            this.configuration.apiTimeout = clientConfiguration.apiTimeout;

            if (clientConfiguration.waitIntervalMonitor != 0) {
                this.configuration.waitIntervalMonitor = clientConfiguration.waitIntervalMonitor;
            }
            this.configuration.autorityServerActif = clientConfiguration.authenticationSettings.authorityServerActif;
            this.configuration.OpenIdConfiguration = this.getOpenIdConfiguration(clientConfiguration.authenticationSettings);
            this.configuration.WellKnownEndpoints = this.getAuthWellKnownEndpoints(this.configuration.OpenIdConfiguration.stsServer);

            this.configuration.defaultSkin = clientConfiguration.defaultSkin;
            this.configuration.skins = this.getSkinSettings(clientConfiguration.skins);

            this.configuration.languages = await this.getLanguages();

            this.onConfigurationLoaded.emit(true);

            console.log(`AppConfigurationService: Settings from API: `, clientConfiguration);

        } catch (err) {
            console.error(`AppConfigurationService: 'load' threw an error on calling ${configUrl}`, err);
            this.onConfigurationLoaded.emit(false);
        }
    }

    async getLanguages(): Promise<Language[]> {
        const responseLang = await fetch(`api/LanguageSettings`);
        if (!responseLang.ok) {
            throw new Error(responseLang.statusText);
        }
        const languages = await responseLang.json();
        return languages;
    }

    private getOpenIdConfiguration(settings: any) {
        const redirUrl = window.location.origin;
        const openIdConfiguration = new OpenIDImplicitFlowConfiguration();

        openIdConfiguration.stsServer = settings.autorityServer;
        openIdConfiguration.redirect_url = settings.redirectUrl;
        openIdConfiguration.client_id = settings.clientId;
        openIdConfiguration.response_type = settings.responseType;
        openIdConfiguration.scope = settings.scope;
        openIdConfiguration.post_logout_redirect_uri = settings.postLogoutRedirectUri;
        openIdConfiguration.post_login_route = settings.startupRoute;
        openIdConfiguration.forbidden_route = settings.forbiddenRoute;
        openIdConfiguration.unauthorized_route = settings.unauthorizedRoute;
        openIdConfiguration.auto_userinfo = true;
        openIdConfiguration.log_console_warning_active = settings.logConsoleWarningActive;
        openIdConfiguration.log_console_debug_active = settings.logConsoleDebugActive;
        openIdConfiguration.max_id_token_iat_offset_allowed_in_seconds = settings.maxIdTokenIatOffsetAllowedInSeconds;
        openIdConfiguration.start_checksession = settings.startCheckSession;
        openIdConfiguration.silent_renew = settings.silentRenew;
        openIdConfiguration.silent_renew_url = settings.redirectUrl + '/silent-renew.html';
        openIdConfiguration.silent_renew_offset_in_seconds = 10;

        return openIdConfiguration;
    }

    private getAuthWellKnownEndpoints(authUrl: string) {
        const authWellKnownEndpoints = new AuthWellKnownEndpoints();

        authWellKnownEndpoints.issuer = authUrl;

        authWellKnownEndpoints.jwks_uri = authUrl + '/.well-known/openid-configuration/jwks';
        authWellKnownEndpoints.authorization_endpoint = authUrl + '/connect/authorize';
        authWellKnownEndpoints.token_endpoint = authUrl + '/connect/token';
        authWellKnownEndpoints.userinfo_endpoint = authUrl + '/connect/userinfo';
        authWellKnownEndpoints.end_session_endpoint = authUrl + '/connect/endsession';
        authWellKnownEndpoints.check_session_iframe = authUrl + '/connect/checksession';
        authWellKnownEndpoints.revocation_endpoint = authUrl + '/connect/revocation';
        authWellKnownEndpoints.introspection_endpoint = authUrl + '/connect/introspect';
        authWellKnownEndpoints.introspection_endpoint = authUrl + '/connect/introspect';

        return authWellKnownEndpoints;
    }

    private getSkinSettings(settings: any[]) {
        const skins = new Array<SkinSetting>();

        settings.forEach(element => {
            const style = new SkinSetting();
            style.label = element.label;
            style.logo = element.logo;
            style.name = element.name;

            skins.push(style);
        });

        return skins;
    }
}


