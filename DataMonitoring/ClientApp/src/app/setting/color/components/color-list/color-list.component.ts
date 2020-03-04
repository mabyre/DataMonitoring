import { Component, OnInit } from '@angular/core';
import {Color} from "@app/shared/models/color";
import {ColorService} from "../../color.service";
import {I18nService} from "@app/shared/i18n/i18n.service";
import {NotificationService} from "@app/shared/layout/notification.service";

@Component({
  selector: 'app-color-list',
  templateUrl: './color-list.component.html',
})
export class ColorListComponent implements OnInit {

  public colors: Color[];
  public errorMessage: string;

  constructor(private colorService: ColorService, private i18nService: I18nService, private notificationService: NotificationService) { }

  ngOnInit() {
    this.showColors();
  }

  showColors() {
    this.colorService.get()
      .subscribe(result => {
        this.colors = result;
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
          this.colorService.delete(id)
            .subscribe(result => {
              this.showColors();
            }, error => { this.errorMessage = error; });
        }
      }
    );
  }

}
