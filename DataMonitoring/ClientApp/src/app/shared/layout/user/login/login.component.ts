import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { OidcSecurityService, AuthorizationResult, AuthorizationState } from "angular-auth-oidc-client";

import { UserService } from '@app/shared/layout/user/user.service';
import { Configuration } from '@app/core/configuration';

@Component({
    selector: 'sa-login',
    templateUrl: './login.component.html',
    styles: []
})
export class LoginComponent implements OnInit {

    isAuthorizedSubscription: Subscription | undefined;
    isAuthorized = false;

    constructor(
        private userService: UserService,
        private oidcSecurityService: OidcSecurityService,
        private configuration: Configuration )
    {
        if (this.oidcSecurityService.moduleSetup) {
            this.doCallbackLogicIfRequired();
        } else {
            this.oidcSecurityService.onModuleSetup.subscribe(() => {
                this.doCallbackLogicIfRequired();
            });
        }

        this.oidcSecurityService.onAuthorizationResult.subscribe(
            (authorizationResult: AuthorizationResult) => {
                this.onAuthorizationResultComplete(authorizationResult);
            });
    }

    ngOnInit() {

        this.isAuthorizedSubscription = this.oidcSecurityService.getIsAuthorized().subscribe(
            (isAuthorized: boolean) => {
                this.isAuthorized = isAuthorized;
                this.userService.emitUserLoggedSubject(isAuthorized);
            });
    }

    ngOnDestroy(): void {

        if (this.isAuthorizedSubscription) {
            this.isAuthorizedSubscription.unsubscribe();
        }
    }

    private doCallbackLogicIfRequired() {

        if (window.location.hash) {
            console.log('authorizedCallback hash:' + window.location.hash);
            this.oidcSecurityService.authorizedCallback();
        }
    }

    private onAuthorizationResultComplete(authorizationResult: AuthorizationResult) {

        console.log('Auth result received:' + authorizationResult);

        if (authorizationResult.authorizationState === AuthorizationState.unauthorized) {
            if (window.parent) {
                // sent from the child iframe, for example the silent renew
                window.parent.location.href = this.configuration.OpenIdConfiguration.unauthorized_route;
            } else {
                // sent from the main window
                window.location.href = this.configuration.OpenIdConfiguration.unauthorized_route;
            }
        }
    }

    login() {
        this.userService.login();
    }

    logout() {
        this.userService.showPopupLogout();
    }
}
