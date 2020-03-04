import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

import { UserService } from '../user/user.service';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html'
})
export class NavigationComponent implements OnInit, OnDestroy {

  isLogged : boolean = false;
  isUserLogged: Subscription;

  constructor(private userService: UserService) {
  }

  ngOnInit() {

      this.isUserLogged = this.userService.isUserLogged.subscribe(
      (isLogged:boolean) => {
        this.isLogged = isLogged;
      }
    );
  }

  ngOnDestroy() {
      this.isUserLogged.unsubscribe();
  }
}
