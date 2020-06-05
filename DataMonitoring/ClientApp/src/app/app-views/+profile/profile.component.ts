import { Component, OnInit } from '@angular/core';
//import {FadeInTop} from "../../shared/animations/fade-in-top.decorator";
import { User } from '@app/shared/models/user';
import { UserService } from '@app/shared/layout/user/user.service';

//@FadeInTop()
@Component( {
    selector: 'sa-profile',
    templateUrl: './profile.component.html',
} )
export class ProfileComponent implements OnInit {

    public currentUser: User;

    constructor( private userService: UserService ) { }

    ngOnInit() {
        this.userService.getUserData().subscribe( user => {
            this.currentUser = user
        } )

        console.log( 'ProfileComponent:ngOnInit:' + this.currentUser.givenName );
    }
}
