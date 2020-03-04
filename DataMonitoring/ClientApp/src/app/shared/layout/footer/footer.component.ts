import { Component, OnInit, OnDestroy } from '@angular/core';
import {Configuration} from '@app/core/configuration';
import { Subscription } from 'rxjs';

import {LayoutService} from '../layout.service';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html'
})
export class FooterComponent implements OnInit {

  public applicationName: string;

  isLogged : boolean = false;
  messageSubscription: Subscription;
  message : string;

  constructor(configuration: Configuration, private layoutService: LayoutService) {
    this.applicationName = configuration.applicationName;
  }

  ngOnInit() {
    this.messageSubscription = this.layoutService.footerMessage.subscribe(
      (message:string) => {
        this.message = message;
      }
    );
  }

  ngOnDestroy() {
    this.messageSubscription.unsubscribe();
  }

}
