import { Component, OnInit } from '@angular/core';

import { ConnectorService } from '../../connector.service';
import { I18nService } from '@app/shared/i18n/i18n.service';
import { NotificationService } from '@app/shared/layout/notification.service';
import { Connector } from "../../connector";
import { UserService } from '@app/shared/layout/user/user.service';
import { Router } from '@angular/router';

@Component( {
    selector: 'app-connectors-list',
    templateUrl: './connectors-list.component.html'
} )
export class ConnectorsListComponent implements OnInit {

    connectors: Connector[];
    errorMessage: string;
    isUserHasRole: boolean;
    canUserDelete: boolean;

    constructor(
        private connectorsService: ConnectorService,
        private i18nService: I18nService,
        private notificationService: NotificationService,
        private userService: UserService,
        private router: Router,
    ) { }

    ngOnInit() {
        this.showConnectors();
        this.isUserHasRole = this.userService.isUserInRole();
        this.canUserDelete = this.userService.canUserDelete();
    }

    showConnectors() {
        this.connectorsService.get()
            .subscribe( result => {
                this.initializeConnectors( result );
            }, error => {
                this.errorMessage = error;
            } );
    }

    initializeConnectors( result: Connector[] ) {
        this.connectors = result;
        this.connectors.forEach( element => {
            if ( element.apiConnector != null ) {
                element.connectorType = 'API';
                element.connection = element.apiConnector.baseUrl;
            }
            else if ( element.sqlServerConnector != null ) {
                element.connectorType = 'SQL Server';
                element.connection = element.sqlServerConnector.hostName + ' - ' + element.sqlServerConnector.databaseName;
            }
            else if ( element.sqLiteConnector != null ) {
                element.connectorType = 'SQLite';
                element.connection = element.sqLiteConnector.hostName + ' - ' + element.sqLiteConnector.databaseName;
            }
        } );
    }

    onDelete( id: number ) {

        if ( this.isUserHasRole == false )
        {
            this.router.navigate( ['./errors/unauthorized'] );  
            return;      
        }

        if ( this.canUserDelete !== true )
        {
            this.router.navigate( ['./errors/unauthorized'] );  
            return;      
        }

        this.errorMessage = null;
        const button = '[' + this.i18nService.getTranslation( "No" ) + '] [' + this.i18nService.getTranslation( "Yes" ) + ']';

        this.notificationService.smartMessageBox(
            {
                title:
                    "<i class='fa fa-sign-out txt-color-orangeDark'></i> " +
                    this.i18nService.getTranslation( "Delete Request" ),
                content: "",
                buttons: button
            },
            ButtonPressed => {
                if ( ButtonPressed == this.i18nService.getTranslation( 'Yes' ) ) {
                    this.connectorsService.delete( id )
                        .subscribe( result => {
                            this.showConnectors();
                        }, error => { this.errorMessage = error; } );
                }
            }
        );
    }

}
