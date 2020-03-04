import { Component, OnInit } from '@angular/core';

import { LayoutService } from '@app/shared/layout/layout.service';
import { UserService } from '@app/shared/layout/user/user.service'

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
})
export class HeaderComponent implements OnInit {

  constructor(private layoutService: LayoutService, private userService: UserService) {
  }

  ngOnInit() {
  }

  showPopupLogout() {
    this.userService.showPopupLogout();
  }

  onFullScreenToggle() {
    this.layoutService.onFullScreenToggle();
  }

}
