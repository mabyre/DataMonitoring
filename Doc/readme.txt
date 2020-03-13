
*** 13/03/2020
- j'ajoute les graphique de type "line" issus de Chart.js (c'est facile à faire)
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
j'ajoute de possibilité de connexion
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
