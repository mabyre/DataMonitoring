import { Component, OnInit } from '@angular/core';

import {I18nService} from "@app/shared/i18n/i18n.service";
import {TimeManagementService} from "../../time-management-service";
import {TimeManagement} from "../../time-management";
import {NotificationService} from "@app/shared/layout/notification.service";

@Component({
  selector: 'app-time-management-list',
  templateUrl: './time-management-list.component.html'
})
export class TimeManagementListComponent implements OnInit {

  public errorMessage: string;
  public timeManagements: TimeManagement[];

  constructor(private timeManagementService: TimeManagementService, private i18nService: I18nService, private notificationService: NotificationService) { }

  ngOnInit() {
    this.showTimes();
  }

  showTimes() {
    this.timeManagementService.get()
      .subscribe(result => {
        this.initializeTimes(result);
      }, error => {
        this.errorMessage = error;
      });
  }

  initializeTimes(result: TimeManagement[]) {
    this.timeManagements = result;
    this.timeManagements.forEach(element => {
      element.typeToDisplay = element.slipperyTime != null 
        ? this.i18nService.getTranslation("Slippery Time")
        : this.i18nService.getTranslation("Time Range");
    });
  }

  onDelete(id: number) {
    this.errorMessage = null;
    const button = '[' + this.i18nService.getTranslation("No") + '] [' + this.i18nService.getTranslation("Yes") + ']';

    this.notificationService.smartMessageBox(
      {
        title:
          "<i class='fa fa-sign-out txt-color-orangeDark'></i> " +
          this.i18nService.getTranslation('Delete Request'),
        content: "",
        buttons: button
      },
      ButtonPressed => {
        if (ButtonPressed == this.i18nService.getTranslation('Yes')) {
          this.timeManagementService.delete(id)
            .subscribe(result => {
              this.showTimes();
            }, error => {
              this.errorMessage = error;
            });
        } else {
          return null;
        }
      }
    );
  }

}
