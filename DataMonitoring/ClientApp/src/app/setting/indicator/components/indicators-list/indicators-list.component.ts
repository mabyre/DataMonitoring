import { Component, OnInit } from '@angular/core';

import { IndicatorService } from '../../indicator.service';
import { Indicator } from "../../indicator";
import { I18nService } from "@app/shared/i18n/i18n.service";
import { NotificationService } from "@app/shared/layout/notification.service";
import { UserService } from '@app/shared/layout/user/user.service';

@Component( {
    selector: 'app-indicators-list',
    templateUrl: './indicators-list.component.html'
} )
export class IndicatorsListComponent implements OnInit {

    public indicators: Indicator[];
    public errorMessage: string;
    public successMessage: string;
    public indicatorTypes: any[];
    isUserHasRole: boolean;

    rows = [];
    temp = [];

    loadingIndicator: boolean = true;

    controls: any = {
        pageSize: 10,
        tableOffset: 0,
        filter: '',
    };

    constructor(
        private indicatorsService: IndicatorService,
        private i18nService: I18nService,
        private notificationService: NotificationService,
        private userService: UserService ) { }

    ngOnInit() {
        this.showIndicators();
        this.isUserHasRole = this.userService.isUserInRole();
    }

    onPageChange( event: any ): void {
        this.controls.tableOffset = event.offset;
    }

    showIndicators() {
        this.indicatorsService.get()
            .subscribe( result => {
                this.indicators = result;
                this.indicatorsService.getIndicatorTypes()
                    .subscribe( typesResult => {
                        this.indicatorTypes = typesResult;
                        if ( this.indicators != null ) {
                            this.indicators.forEach( element => {
                                element.typeDisplay = this.indicatorTypes.find( x => x.value == element.type ).name;
                            } );
                        }
                        this.temp = [...this.indicators];
                        this.rows = this.indicators;
                        this.loadingIndicator = false;
                    }, error => {
                        this.errorMessage = error;
                    } );
            }, error => {
                this.errorMessage = error;
            } );
    }

    onDelete( id: number ) {
        this.errorMessage = null;
        const button = '[' + this.i18nService.getTranslation( "No" ) + '] [' + this.i18nService.getTranslation( "Yes" ) + ']';

        this.notificationService.smartMessageBox(
            {
                title:
                    "<i class='fa fa-sign-out txt-color-orangeDark'></i> " +
                    this.i18nService.getTranslation( 'Delete Request' ),
                content: "",
                buttons: button
            },
            ButtonPressed => {
                if ( ButtonPressed == this.i18nService.getTranslation( 'Yes' ) ) {
                    this.indicatorsService.delete( id )
                        .subscribe( result => {
                            this.successMessage = this.i18nService.getTranslation( "Line Deleted" );
                            this.showIndicators();
                            setTimeout( () => this.successMessage = '', 5000 );
                        }, error => {
                            this.errorMessage = error;
                        } );
                } else {
                    return null;
                }
            }
        );
    }
}
