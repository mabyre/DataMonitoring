import { Injectable } from '@angular/core';
import { Router, CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot } from '@angular/router';
import { Observable, forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';

import { OidcSecurityService } from "angular-auth-oidc-client";

import { UserService } from "@app/shared/layout/user/user.service"
import { Configuration } from '@app/core/configuration';

@Injectable()
export class AuthorizationGuard implements CanActivate {

  constructor(
    private router: Router,
    private oidcSecurityService: OidcSecurityService,
    private userService: UserService,
    private configuration: Configuration) { }

  public canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Observable<boolean> | boolean {
    console.log(route + '' + state);
    console.log('AuthorizationGuard, canActivate');

    if (this.configuration.autorityServerActif === false) {
      console.log("module security desactivated");
      return true;
    }

    return this.oidcSecurityService.getIsAuthorized().pipe(
      map((isAuthorized: boolean) => {
        console.log('AuthorizationGuard, canActivate isAuthorized: ' + isAuthorized);

        if (isAuthorized) {
          const user = this.userService.userData;

          if (user.isAdmin) { return true; }

          if (user.applicationScopes.find(x => x === this.configuration.applicationScope) === null) {
            this.router.navigate([this.configuration.OpenIdConfiguration.forbidden_route]);
            return false;
          }

          if (route.data.role != null) {

            if (route.data.role.indexOf(';') !== -1) {

              const roles = route.data.role.split(';');

              for (let i = 0; i < roles.length; i++) {
                if (user.roles.find(x => x === roles[i])) {
                  return true;
                }
              }
            } else {
              if (user.roles.find(x => x === route.data.role))
              {
                return true;
              }
            }

            if (route.data.permission === null) {
              this.router.navigate([this.configuration.OpenIdConfiguration.forbidden_route]);
              return false;
            }
          }

          if (route.data.permission !== null) {
            if (user.permissions !== null && user.permissions.find(x => x === route.data.permission)) {
              return true;
            }

            this.router.navigate([this.configuration.OpenIdConfiguration.forbidden_route]);
            return false;
          }

          return true;
        }

        this.router.navigate([this.configuration.OpenIdConfiguration.unauthorized_route]);
        return false;
      })
    );

  }
}

