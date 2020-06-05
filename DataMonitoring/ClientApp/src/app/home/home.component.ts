import { Component } from '@angular/core';
import { Configuration } from '@app/core/configuration';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
})
export class HomeComponent {

    public defaultLocalValue: string;

    constructor(_configuration: Configuration) {
        console.log("HomeComponent:constructor");
        this.defaultLocalValue = _configuration.defaultLocale;
    }
}
