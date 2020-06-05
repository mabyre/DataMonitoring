import { Injectable } from '@angular/core';
import { Subscription, Observable, Subject, BehaviorSubject } from "rxjs";
import { map } from 'rxjs/operators';
import { OidcSecurityService } from "angular-auth-oidc-client";

import { NotificationService } from '@app/shared/layout/notification.service';
import { I18nService } from '@app/shared/i18n/i18n.service';
import { Configuration } from '@app/core/configuration';
import { User } from '@app/shared/models/user';

@Injectable({
    providedIn: 'root',
})
export class UserService {

    isUserLogged = new Subject<boolean>();

    // Store the URL so we can redirect after logging in
    redirectUrl: string;

    userDataSubscription: Subscription | undefined;
    userData: any = '';
    userDataEmailValid: false;

    constructor(
        private oidcSecurityService: OidcSecurityService,
        private notificationService: NotificationService,
        private configuration: Configuration,
        private i18nService: I18nService) {

        this.userDataSubscription = this.oidcSecurityService.getUserData().subscribe(
            (data: any) => {
                if (data !== '') {
                    this.userData = data;
                }

                console.log('UserService: constructor:getUserData');
            });
    }

    emitUserLoggedSubject(logged: boolean) {
        this.isUserLogged.next(logged);
    }

    getUserData(): Observable<any> {
        return this.oidcSecurityService.getUserData().pipe(
            map( result => {
                    return this.getUser(result);
                 }
            )
        );
    }

    private getUser(userData): any {
        const user = new User();

        if ( userData !== '' ) {

            //
            // User's informations 
            // see Claims in StsIdentityServer
            //
            user.email = userData.email;
            user.emailVerified = userData.email_verified;
            user.givenName = userData.given_name;
            user.firstName= userData.firstName;
            user.lastName = userData.lastName;
            user.address = userData.address;
            user.society = userData.society;
            user.phone = userData.phone;

            user.googleId = userData.googleId;
            user.gravatarId = userData.gravatarId;

            if (userData.gender != null) {
                user.gender = userData.gender;
            } else {
                user.gender = "male";
            }

            if (userData.role instanceof Array) {
                user.roles = userData.role;
            } else {
                user.roles = new Array();
                user.roles[0] = userData.role;
            }

            // SuperAdmin == IsAdmin + Role == "Admin" cf. StsIdentityServer
            if (user.roles.find((e) => { return e === "SuperAdmin" }))
                user.isAdmin = true;
            else
                user.isAdmin = false;

            // BRY_20200106 don't use this
            // if (userData.ApplicationAccess instanceof Array) {
            //     user.applicationScopes = userData.ApplicationAccess;
            // } else {
            //     user.applicationScopes = new Array();
            //     user.applicationScopes[0] = userData.ApplicationAccess;
            // }

            // don't use this 
            //const permission = userData[this.configuration.applicationScope + '/permission'];
            //if (permission instanceof Array) {
            //    user.permissions = permission;
            //} else {
            //    user.permissions = new Array();
            //    user.permissions[0] = permission;
            //}

            const mettre_un_point_darret = 3;
        }
        return user;
    }

    isUserAuthenticated(): boolean {
        const currentUser = this.userData;
        const log = (currentUser !== "");
        return log;
    }

    isUserInRole(): boolean {
        const role = ( this.userData.role !== undefined );
        return role;
    }

    showPopupLogout() {

        const button = '[' + this.i18nService.getTranslation("No") + '] [' + this.i18nService.getTranslation("Yes") + ']';

        this.notificationService.smartMessageBox(
            {
                title:
                    "<i class='fa fa-sign-out txt-color-orangeDark'></i> " +
                    this.i18nService.getTranslation("Logout") +
                    " <span class='txt-color-orangeDark'><strong>" + this.userData.given_name + "</strong></span> ?",
                content: "",
                buttons: button
            },
            ( ButtonPressed: string) => {
                if (ButtonPressed == this.i18nService.getTranslation('Yes')) {
                    this.logout();
                }
            }
        );
    }

    login() {
        console.log('UserService: start login');
        
        if (localStorage.getItem('culture') !== null) {
            this.oidcSecurityService.setCustomRequestParameters({ 'ui_locales': localStorage.getItem('culture') });
        }
        this.oidcSecurityService.authorize();
    }

    private logout() {
        console.log('UserService: start logoff');
        
        this.oidcSecurityService.logoff();
    }
}
