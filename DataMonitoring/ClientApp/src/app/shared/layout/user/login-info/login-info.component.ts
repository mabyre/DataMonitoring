import { Component, OnInit, Input, Inject } from '@angular/core';
import { User } from '@app/shared/models/user';
import { UserService } from '../user.service';
import { LayoutService } from '../../layout.service';
import { Configuration } from '@app/core/configuration';
import { DOCUMENT } from '@angular/common';

@Component({

  selector: 'app-login-info',
  templateUrl: './login-info.component.html',
})
export class LoginInfoComponent implements OnInit {

  @Input() displayUser : boolean = true; // BRY_20200519
  @Input() dropdownClass : string = 'dropdown-menu';

  public currentUser: User;
  public givenName: string;
  public gravatarId: string;
  public googleId: string;
  public avatarSource: string;

  constructor(private userService: UserService,
      private layoutService: LayoutService,
      private configuration: Configuration,
      @Inject(DOCUMENT) private document: Document) {
  }

  ngOnInit() {
      this.userService.getUserData().subscribe(user => {
          if ( user !== undefined )
          {
              this.currentUser = user;

              this.givenName = this.currentUser.givenName;
              this.gravatarId = this.currentUser.gravatarId;
              this.googleId = this.currentUser.googleId;
              this.avatarSource = "img/avatars/male.png"
          }
      });

      console.log( 'LoginInfoComponent:ngOnInit:currentUser:' + this.currentUser.givenName );
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

  onClickGoToManageUserUrl(): void {
      const url = this.configuration.OpenIdConfiguration.stsServer + '/Manage/Index'
      this.document.location.href = url;
  }
}