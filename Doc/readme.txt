
Script-migration

BRY_20200723 : CanDelete user isAdmin

canActivateChild: [AuthorizationGuard],
au niveau de 
\DataMonitoring\ClientApp\src\app\app-routing.module.ts
ca marche pas !

je dois le mettre là :
\DataMonitoring\ClientApp\src\app\app-views\app-views.routing.ts

errors/unauthorized
isUserAuthenticated

regarder si on ne pourrait pas mettre une fonction dans user.service !
const currentUser = this.userService.userData;

passer un paramètre à 
this.oidcSecurityService.logoff();

\DataMonitoring\ClientApp\src\app\core\auth\authorization.guard.ts

BRY_20200106 
j'utilise pas applicationScopes ApplicationAccess pour déterminer les droits !
je ne sais pas ce qui était prévu ...

SuperAdmin

<b>Roles</b>
<ul class="list-unstyled">
    <li *ngFor="let role of currentUser.roles" [value]="role">
        {{role}}<br>
    </li>
</ul>

s1.jpg -> dashboard.jpg

app\shared\models

const user = this.userService.userData;

__DATE__ 20/05/2020

je suis surpris par le manque d'info sur le User, en gros il n'y a que givenName

regarder dans l'exemple :
C:\Users\mabyre\Documents\Visual Studio 2019\Samples\Angular CSS JavaScript\Angular\angular-8-registration-login-example-master

            {
                path: 'app-views', 
                loadChildren: './app-views/app-views.module#AppViewsModule',
                data:{ pageTitle: 'App Views' }
            },

<sa-login></sa-login>

<sa-profile>

\ClientApp\src\app\shared\layout\user\login-info\login-info.component.html
          <a routerLink="/app-views/profile" class="padding-10 padding-top-0 padding-bottom-0">
            <i class="fa fa-user"></i> {{'Profile' | i18n}}
          </a>

NgxDnDModule, // BRY ngx-dnd v5.1.0
npm i @swimlane/ngx-dnd@5.1.0

run `npm audit fix` to fix them, or `npm audit` for details
npm i @swimlane/ngx-dnd@5.0.6

??defineInjectable

Refused to frame 'https://localhost:5001/' because an ancestor violates the following Content Security Policy directive: "frame-ancestors https://localhost:28001 https://localhost:28001 https://localhost:44352 https://localhost:4200".

[WDS] Warnings while compiling.
vendor.js:131528
./node_modules/@swimlane/ngx-dnd/fesm5/swimlane-ngx-dnd.js 359:59-77
"export '??defineInjectable' was not found in '@angular/core'
vendor.js:131534
ERROR
core.js:12584
Error: Uncaught (in promise): TypeError: Object(...) is not a function

StsIdentityServer
https://localhost:5001

le bouton <a routerLink="/app-views/profile" 
dirige dans le vide ... app-views est utilisé dans 
\ClientApp\src\app\shared\layout\shortcut\shortcut.component.html

une fois connecté une dropdownClass
\DataMonitoring\ClientApp\src\app\shared\layout\user\login-info\login-info.component.html
apparait avec le menu et les items suivants "settings" "profile"

silent-renew.html

------------------------

Pour la première fois je connecte aaa@bbb.com

Request Id: 80000008-0000-a300-b63f-84710c7967bb

Refused to frame 'https://localhost:44318/' because an ancestor violates the following Content Security Policy directive: "frame-ancestors 'self'".
La solution se trouve dans StsIdentity

dans :
    DataMonitoring\ClientApp\src\app\app.module.ts
on se retrouve avec 
    configuration.OpenIdConfiguration
vide

AppConfigurationService devient AppSettingsService

configuration.OpenIdConfiguration

renommer configuration en appsettings :
appsettings.ts
appsettings.service.ts

DataMonitoring\ClientApp\src\app\core\configuration.ts
public OpenIdConfiguration: OpenIDImplicitFlowConfiguration;

rename LogoutComponent en LoginCOmponent

Failed to execute 'open' on 'XMLHttpRequest': Invalid URL
"ApiServerUrl": "https://localhost:28001",
au lieu de :
"ApiServerUrl": "https://localhost:28001/",

https://localhost:28001/api/clientappsettings
{"applicationName":"SoDevLog-Monitoring","applicationScope":"DataMonitoringApp","defaultLocale":"us","defaultSkin":"sodevlog-style-0",
"apiServerUrl":"http://localhost:28000/","apiRetry":1,"apiTimeout":100,"waitIntervalMonitor":30,"waitIntervalQueryBackgroundTask":25,
"authenticationSettings":{"authorityServerActif":true,"autorityServer":"https://localhost:44318",
"redirectUrl":"https://localhost:44338","clientId":"angular_spa","responseType":"id_token token","scope":"openid profile api1 ",
"postLogoutRedirectUri":"https://localhost:44338/Home", "startCheckSession":true,"silentRenew":true, 
"startupRoute":"/Home","forbiddenRoute":"/errors/forbidden","unauthorizedRoute":"/errors/unauthorized","logConsoleWarningActive":true,
"logConsoleDebugActive":true,"maxIdTokenIatOffsetAllowedInSeconds":"60"},
"skins":[{"name":"sodevlog-style-0","logo":"/img/Sodevlog.png","label":"SoDevLog Default"},
{"name":"smart-style-1","logo":"/img/Sodevlog-DataMonitoring.png","label":"Dark Elegance"},
{"name":"smart-style-2","logo":"/img/Sodevlog-DataMonitoring.png","label":"Ultra Light"},
{"name":"smart-style-3","logo":"/img/Sodevlog.png","label":"Google Skin"},
{"name":"smart-style-4","logo":"/img/Sodevlog.png","label":"PixelSmash"},
{"name":"smart-style-5","logo":"/img/Sodevlog-DataMonitoring.png","label":"Glass"},
{"name":"smart-style-6","logo":"/img/Sodevlog.png","label":"Autre"}]}

\DataMonitoring\Controllers\SecurityHeadersAttribute.cs
context.HttpContext.Response.Headers.ContainsKey("Content-Security-Policy")

"ApiServerUrl": "http://localhost:28000/",

ClientUrl
https://localhost:44338/

AuthorityServerUrl
https://localhost:44318/

StsIdentityServer
https://localhost:44318/

BRY_Refuse_To_Frame
Refused to frame 'https://localhost:44318/' because an ancestor violates the following Content Security Policy directive: "frame-ancestors 'self'".
Sorry, there was an error : unauthorized_client

npm uninstall angular-auth-oidc-client --save

getClientSettings

StsIdentityServer
https://localhost:44318/

\DataMonitoringApp-20200511_1204\DataMonitoring\appsettings.Development.json
"AuthorityServerUrl": "https://sodevlog-001:5001",
"AuthorityServerUrl": "https://localhost:44337",
"AuthorityServerUrl": "https://localhost:44300/",

StsIdentityServer\Properties\launchSettings.json
iisSettings
"applicationUrl": "https://localhost:44300/"


SSL \DataMonitoring\ClientApp\package.json
    //"start": "ng serve --ssl --ssl-key .\\ssl\\server.key  --ssl-cert .\\ssl\\server.crt",

C:\Users\mabyre\Documents\Visual Studio 2019\Samples\DataMonitoringApp\DataMonitoringApp\DataMonitoring\ClientApp\protractor.conf.js

https://localhost:44338/

http://localhost:28000;https://localhost:28001
;https://localhost:28001

https://localhost:44318/

\package.json
"start": "ng serve --ssl --ssl-key .\\ssl\\server.key  --ssl-cert .\\ssl\\server.crt",

        {
            "type": "chrome",
            "request": "launch",
            "name": "Chrome:HTTPS",
            "url": "https://localhost:4200",
            "webRoot": "${workspaceFolder}"
        },
		
    get authApiURI() {
        return 'http://localhost:5000/api';
    } 

ng serve --ssl --ssl-key .\ssl\server.key  --ssl-cert .\ssl\server.crt
	
le module "user" qui est de base dans "shared/user" a été déplacé dans Layout ...
à la place j'utilise "app/account/login" avec le module oidc-client

OidcSecurityService

LogoutComponent 

// BRY_20200512 intégration de l'authentification

AuthorizationGuard

je cherche le bouton Login, je trouve :
<span> <a title="{{'Sign in' | i18n}}"><i class="fa fa-sign-in"></i></a> </span>
dans \DataMonitoring\ClientApp\src\app\shared\layout\user\logout\logout.component.html

*** 08/05/2020
après le dev de l'IdentityServer j'ai un peu perdu le contact.
la doc se trouve dans :
D:\SoDevLog\Projets\DataMonitoringApp


*** 04/03/2020
- j'ajoute les graphiques de type "line" issus de Chart.js (c'est facile à faire)
- j'ajoute la possibilité d'écrire des reuqêtes de type "SELECT TOP"
- pour un graphique de type Chart j'accepte la mesure de Type Snapshot
je constate que la mesure de type flow pour un "Chart" est une mesure de type Temps 
il y dans Chart.js une config possible pour que l'axe des x soit le temps et ces ce qui est utlisé à outrance 

DataMonitoringApp-20200309_1255
Je sauve avant d'ajouter le typede chart 'line'

*** DataMonitoringApp-git
Pour mettre en gestion de conf avec git il faut cloner !
c'est à dire récupérer sur mon disque un repo git créé dans le cloud
du coup j'ai créé DataMonitoringApp-git puis j'ai copiée la version DataMonitoringApp-20200304_1037
comme ça je n'ai pas le répertoire node_modules
mais comme j'ai ouvert le solution avec Visual Studio il m'a recréé tout un tas de daubes mais qui ne se retrouve pas dans le git.

donc je ne souhaite pas exécuter l'application à partir de DataMonitoringApp-git donc je travaille danse DataMonitoringApp 
et puis je ferai un beyong compare

*** DataMonitoringApp-20200210_1107
je souhaite ajouter un nouveau type de Query
je comprends pourquoi c'est crétins oblige à ajouter localDate ou UtcDate dans la Query ...

Ajouter %%value%% dans la progressbar

*** 27/01/2020
je n'arrive pas à mettre à jour ColorClass j'ai renomé dans le modèle par ColorHtml
je pourrais tenter de renommer la table à la main ...

*** 16/01/2020
j'ajoute la possibilité de connexion
je supprime l'authentification requise sur le create d'un connector
BRY_20200116

*** 15/01/2020
j'ajoute 
<li routerLinkActive="active">
dans 
\DataMonitoring\ClientApp\src\app\shared\layout\navigation\navigation.component.html
pour faire refonctionner la sélection automatique du menu
comme c'est indiqué dans :
https://angular.io/api/router/RouterLinkActive#description

// TODO : Ajouter la couleur en hexa dans la liste des couleurs

Authentification je trouve l'existence de IdentityServer4

https://github.com/IdentityServer/IdentityServer4

using IdentityServer4.AccessTokenValidation;
