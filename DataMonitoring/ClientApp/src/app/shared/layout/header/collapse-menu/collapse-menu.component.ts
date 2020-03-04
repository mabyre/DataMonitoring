import {Component, OnInit} from '@angular/core';
import {LayoutService} from "@app/shared/layout/layout.service";

@Component({
  selector: 'app-collapse-menu',
  templateUrl: './collapse-menu.component.html'
})
export class CollapseMenuComponent {

    constructor(private layoutService: LayoutService) {}

  onToggle() {
    this.layoutService.onCollapseMenu();
  }
}
