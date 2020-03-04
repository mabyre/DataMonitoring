import { Component } from '@angular/core';
import { LayoutService } from "@app/shared/layout/layout.service";

@Component({
  selector: 'app-min-menu',
  template: `<span class="minifyme" (click)="toggle()">
  	<i class="fa fa-arrow-circle-left hit"></i> 
  </span>`,
})
export class MinifyMenuComponent {

  constructor( private layoutService: LayoutService ) {
  }

  toggle() {
    this.layoutService.onMinifyMenu();
  }
}
