import { Component, OnInit, Input } from '@angular/core';
import { Subscription } from "rxjs";
import { UserService } from '@app/shared/layout/user/user.service';
import { LayoutService } from '@app/shared/layout/layout.service';
import { User } from '../user';

@Component({

  selector: 'app-login-info',
  templateUrl: './login-info.component.html',
})
export class LoginInfoComponent implements OnInit {

  @Input() displayUser : boolean = false;
  @Input() dropdownClass : string = 'dropdown-menu';


  userDataSubscription: Subscription | undefined;

  public givenName: string;
  public gravatarId: string;
  public googleId: string;
  public avatarSource: string;

  constructor(private userService: UserService, private layoutService: LayoutService) {
  }

  ngOnInit() {
    this.userDataSubscription = this.userService.getUserData().subscribe(
      (userData: User) => {

        if (userData !== null) {
    
          this.givenName = userData.givenName;
          this.gravatarId = userData.gravatarId;
          this.googleId = userData.googleId;
       
          if (userData.gender === "male") {
            this.avatarSource = "img/avatars/male.png";
          }
          if (userData.gender === "female") {
            this.avatarSource = "img/avatars/female.png";
          }
        }

        console.log('given_name getting data');
      });
  }

  ngOnDestroy(): void {
    if (this.userDataSubscription) {
      this.userDataSubscription.unsubscribe();
    }
  }

  toggleShortcut() {
    this.layoutService.onShortcutToggle();
  }

  onFullScreenToggle() {
    this.layoutService.onFullScreenToggle();
  }

  showPopupLogout() {
    this.userService.showPopupLogout();
  }

}
