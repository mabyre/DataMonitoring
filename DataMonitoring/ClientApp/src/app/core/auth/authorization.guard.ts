import { Injectable } from '@angular/core';
import {
    Router,
    CanActivate,
    CanActivateChild,
    CanLoad,
    Route,
    ActivatedRouteSnapshot,
    RouterStateSnapshot,
} from '@angular/router';

import { Observable, forkJoin } from 'rxjs';
import { map } from 'rxjs/operators';

import { OidcSecurityService } from "angular-auth-oidc-client";

import { UserService } from "@app/shared/layout/user/user.service"
import { Configuration } from '@app/core/configuration';

@Injectable()
export class AuthorizationGuard implements CanActivate, CanActivateChild, CanLoad {

    constructor(
        private router: Router,
        private oidcSecurityService: OidcSecurityService,
        private userService: UserService,
        private configuration: Configuration ) { }

    public canActivate( route: ActivatedRouteSnapshot, state: RouterStateSnapshot ): Observable<boolean> | boolean {
        console.log( 'AuthorizationGuard: canActivate' );

        if ( this.configuration.autorityServerActif === false ) {
            console.log( 'AuthorizationGuard: module security desactivated' );
            return true;
        }

        return this.oidcSecurityService.getIsAuthorized().pipe(
            map( ( isAuthorized: boolean ) => {
                console.log( 'AuthorizationGuard: getIsAuthorized' );

                if ( isAuthorized ) {
                    const currentUser = this.userService.userData;

                    // SuperAdmin got all permissions
                    if ( currentUser.isAdmin ) { return true; }

                    // BRY_20200106 don't use scope
                    // if (user.applicationScopes.find(x => x === this.configuration.applicationScope) === null) {
                    //   this.router.navigate([this.configuration.OpenIdConfiguration.forbidden_route]);
                    //   return false;
                    // }

                    // If route is restricted by roles
                    if ( route.data.roles ) {
                        const roles = route.data.roles;

                        if ( currentUser.role ) {
                            const userRoles = currentUser.role;

                            for ( let i = 0; i < roles.length; i++ ) {
                                for ( let j = 0; j < userRoles.length; j++ ) {
                                    if ( userRoles[i] == roles[j] ) {
                                        return true;
                                    }
                                }
                            }
                        }

                        this.router.navigate( [this.configuration.OpenIdConfiguration.forbidden_route] );
                        return false;
                    }

                    // Don't use permission
                    // if (route.data.permission !== null) {
                    //   if (currentUser.permissions !== null && currentUser.permissions.find(x => x === route.data.permission)) {
                    //     return true;
                    //   }

                    this.router.navigate( [this.configuration.OpenIdConfiguration.forbidden_route] );
                    return false;
                }

                this.router.navigate( [this.configuration.OpenIdConfiguration.unauthorized_route] );
                return false;
            } )
        );
    }

    canActivateChild( route: ActivatedRouteSnapshot, state: RouterStateSnapshot ): Observable<boolean> | boolean {
        return this.canActivate( route, state );
    }

    checkLogin( url: string ): boolean {
        if ( this.userService.isUserAuthenticated() ) {
            return true;
        }

        // Store the attempted URL for redirecting
        this.userService.redirectUrl = url;

        // Navigate to the login page with extras
        this.router.navigate( ['/login'] );

        return false;
    }

    canLoad( route: Route ): boolean {
        let url = `/${route.path}`;

        if ( this.userService.isUserAuthenticated() ) {
            return true;
        }

        this.router.navigate( [this.configuration.OpenIdConfiguration.unauthorized_route] );

        return false;

        //console.log('AuthorizationGuard, canLoad:' + this.oidcSecurityService.moduleSetup);
        //return this.oidcSecurityService.moduleSetup;
    }
}
