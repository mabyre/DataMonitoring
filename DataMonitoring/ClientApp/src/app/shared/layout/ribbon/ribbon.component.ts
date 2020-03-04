import { Component, OnInit } from '@angular/core';
import {LayoutService} from "@app/shared/layout/layout.service";

@Component({
  selector: 'app-ribbon',
  templateUrl: './ribbon.component.html'
})
export class RibbonComponent implements OnInit {

  constructor(private layoutService: LayoutService) {}

  ngOnInit() {
  }

  resetWidgets() {
    this.layoutService.factoryReset();
  }

}
