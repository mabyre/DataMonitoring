import { Injectable } from '@angular/core';
import { Subscription, Observable, Subject } from "rxjs";
import { map } from 'rxjs/operators';
import { OidcSecurityService } from "angular-auth-oidc-client";

import { NotificationService } from '@app/shared/layout/notification.service';
import { I18nService } from '@app/shared/i18n/i18n.service';
import { Configuration } from '@app/core/configuration';
import { User } from './user';

@Injectable({
  providedIn: 'root',
})
export class UserService {

  isUserLogged = new Subject<boolean>();

  userDataSubscription: Subscription | undefined;
  userData: User;
  userLogged : boolean = false;

  constructor(private oidcSecurityService: OidcSecurityService,
    private notificationService: NotificationService,
    private configuration: Configuration,
    private i18nService: I18nService) {

    this.userDataSubscription = this.oidcSecurityService.getUserData().subscribe(
      (userData: any) => {

        if (userData !== '') {
          this.userData = this.getUser(userData);
        }

        console.log('userData getting data');
      });

  }

  emitUserLoggedSubject(logged : boolean) {
    this.isUserLogged.next(logged);
  }

  getUserData(): Observable<any> {

    return this.oidcSecurityService.getUserData().pipe(
      map(
        result => {
          return this.getUser(result);
        }
      ));
  }

  private getUser(userData): User {

    const user = new User();

    if (userData !== '') {

      user.givenName = userData.given_name;
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

      if (user.roles.find((e) => { return e === "admin" }))
        user.isAdmin = true;
      else
        user.isAdmin = false;

      if (userData.ApplicationAccess instanceof Array) {
        user.applicationScopes = userData.ApplicationAccess;
      } else {
        user.applicationScopes = new Array();
        user.applicationScopes[0] = userData.ApplicationAccess;
      }

      const permission = userData[this.configuration.applicationScope + '/permission'];
      if (permission instanceof Array) {
        user.permissions = permission;
      } else {
        user.permissions = new Array();
        user.permissions[0] = permission;
      }
    }
    return user;
  }

  showPopupLogout() {

    var button = '[' + this.i18nService.getTranslation("No") + '] [' + this.i18nService.getTranslation("Yes") + ']';

    this.notificationService.smartMessageBox(
      {
        title:
          "<i class='fa fa-sign-out txt-color-orangeDark'></i> " +
          this.i18nService.getTranslation("Logout") +
          " <span class='txt-color-orangeDark'><strong>" + this.userData.givenName + "</strong></span> ?",
        content: "",
        buttons: button
      },
      ButtonPressed => {
        if (ButtonPressed == this.i18nService.getTranslation('Yes')) {
          this.logout();
        }
      }
    );
  }

  login() {
    console.log('start login');

    if (localStorage.getItem('culture') !== null) {
      this.oidcSecurityService.setCustomRequestParameters({ 'ui_locales': localStorage.getItem('culture') });
    }
    this.oidcSecurityService.authorize();
  }

  private logout() {
    console.log('start logoff');
    this.oidcSecurityService.logoff();
  }
}
