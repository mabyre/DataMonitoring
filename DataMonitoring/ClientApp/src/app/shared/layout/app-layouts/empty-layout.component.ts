import { Component, OnInit } from '@angular/core';

import { LayoutService } from '@app/shared/layout/layout.service';

@Component({
  selector: 'app-empty-layout',
  templateUrl: './empty-layout.component.html',
  styles: []
})
export class EmptyLayoutComponent implements OnInit {

  constructor(private layoutService: LayoutService) { }

  ngOnInit() {
    this.layoutService.onCollapseMenu();
  }
}
