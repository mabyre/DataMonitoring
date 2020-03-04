import { Component, OnInit } from '@angular/core';
import {I18nService} from "@app/shared/i18n/i18n.service";
import {NotificationService} from "@app/shared/layout/notification.service";
import {DashboardService} from "@app/dashboard/dashboard.service";

@Component({
  selector: 'app-dashboard-list',
  templateUrl: './dashboard-list.component.html',
})
export class DashboardListComponent implements OnInit {

  public errorMessage: string;
  public successMessage:string;

  rows = [];
  temp = [];

  loadingIndicator: boolean = true;

  controls: any = {
    pageSize: 10,
    tableOffset: 0,
    filter: '',
  };

  constructor(private i18nservice: I18nService, private notificationService: NotificationService, private dashboardService: DashboardService) { }

  ngOnInit() {
    this.showDashboards();
  }

  onPageChange(event: any): void {
    this.controls.tableOffset = event.offset;
  }

  showDashboards() {
    this.dashboardService.get()
      .subscribe(data => {
        this.temp = [...data];
        this.rows = data;
        this.loadingIndicator = false;
      }, error => {
        this.errorMessage = error;
      });
  }

  onDelete(id: number) {

    this.errorMessage = null;
    const button = '[' + this.i18nservice.getTranslation("No") + '] [' + this.i18nservice.getTranslation("Yes") + ']';

    this.notificationService.smartMessageBox(
      {
        title:
          "<i class='fa fa-sign-out txt-color-orangeDark'></i> " +
          this.i18nservice.getTranslation("Delete Request"),
        content: "",
        buttons: button
      },
      ButtonPressed => {
        if (ButtonPressed == this.i18nservice.getTranslation('Yes')) {
          this.dashboardService.delete(id)
            .subscribe(result => {
              this.successMessage = this.i18nservice.getTranslation("Line Deleted");
              this.showDashboards();
              setTimeout(() => this.successMessage = '', 5000);

            }, error => { this.errorMessage = error; });
        }
      }
    );
  }
}
