import { Component } from '@angular/core';
import { Configuration } from '@app/core/configuration';
// import { UserService } from "@app/shared/layout/user/user.service";
// import { User } from '@app/shared/layout/user/user';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  public defaultLocalValue: string;
  // public userService: User;
  
  constructor(
    // _userService: UserService,
    _configuration: Configuration) {
    console.log("Home constructor");
    this.defaultLocalValue = _configuration.defaultLocale;
    // this.userService = _userService.userData;
  }
}
