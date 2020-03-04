import { Component, OnInit } from '@angular/core';
import {Style} from "@app/shared/models/style";
import {I18nService} from "@app/shared/i18n/i18n.service";
import {NotificationService} from "@app/shared/layout/notification.service";
import {StyleService} from "../../style.service";

@Component({
  selector: 'app-style-list',
  templateUrl: './style-list.component.html'
})
export class StyleListComponent implements OnInit {

  public styles: Style[];
  public errorMessage: string;

  constructor(private styleService: StyleService, private i18nService: I18nService, private notificationService: NotificationService) { }

  ngOnInit() {
    this.showStyles();
  }

  showStyles() {
    this.styleService.get()
      .subscribe(result => {
        this.styles = result;
      }, error => {
        this.errorMessage = error;
      });
  }

  onDelete(id: number) {

    this.errorMessage = null;
    const button = '[' + this.i18nService.getTranslation("No") + '] [' + this.i18nService.getTranslation("Yes") + ']';

    this.notificationService.smartMessageBox(
      {
        title:
          "<i class='fa fa-sign-out txt-color-orangeDark'></i> " +
          this.i18nService.getTranslation("Delete Request"),
        content: "",
        buttons: button
      },
      ButtonPressed => {
        if (ButtonPressed == this.i18nService.getTranslation('Yes')) {
          this.styleService.delete(id)
            .subscribe(result => {
              this.showStyles();
            }, error => { this.errorMessage = error; });
        }
      }
    );
  }

}
