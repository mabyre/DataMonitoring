import { Component, OnInit } from '@angular/core';
import { UserService } from '@app/shared/layout/user/user.service'

@Component({
  selector: 'app-unauthorized',
  templateUrl: './unauthorized.component.html',
})
export class UnauthorizedComponent implements OnInit {

  constructor(private userService: UserService) { }

  ngOnInit() {
  }

  login() {
    this.userService.login();
  }

}
