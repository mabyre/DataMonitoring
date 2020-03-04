import { Component, OnInit } from '@angular/core';

import { NotificationService } from '@app/shared/layout/notification.service';

import { I18nService } from '@app/shared/i18n/i18n.service';
import { WidgetsService } from '../widget.service';
import {Widget} from "../models/widget";


@Component({
  selector: 'app-widget-list',
  templateUrl: './widget-list.component.html'
})
export class WidgetListComponent implements OnInit {

  public errorMessage: string;
  public successMessage:string;

  public widgets: Widget[];
  public widgetTypes: any[];

  rows = [];
  temp = [];

  loadingIndicator: boolean = true;
  reorderable: boolean = true;

  controls: any = {
    pageSize: 10,
    tableOffset: 0,
    filter: '',
  };

  constructor(private widgetService: WidgetsService, private i18nservice: I18nService, private notificationService: NotificationService) { }

  ngOnInit() {

    this.showWidget();
  }

  showWidget() {

    this.widgetService.get()
      .subscribe(data => {
        this.widgets = data;
        this.widgetService.getWidgetTypeList()
          .subscribe(typesResult => {
            this.widgetTypes = typesResult;
            if (this.widgets != null) {
              this.widgets.forEach(element => {
                element.typeDisplay = this.widgetTypes.find(x => x.value == element.type).name;
              });
            }
            this.temp = [...this.widgets];
            this.rows = this.widgets;
            this.loadingIndicator = false;
          }, error => {
            this.errorMessage = error;
          });
      }, error => {
        this.errorMessage = error;
      });
  }

  onPageChange(event: any): void {
    this.controls.tableOffset = event.offset;
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
          this.widgetService.delete(id)
            .subscribe(result => {
              this.successMessage = this.i18nservice.getTranslation("Line Deleted");
              this.showWidget();
              setTimeout(() => this.successMessage = '', 5000);

            }, error => { this.errorMessage = error; });
        }
      }
    );
  }

  onDuplicate(id: number) {
    this.errorMessage = null;

    this.widgetService.duplicateWidget(id)
      .subscribe(result => {
        this.successMessage = this.i18nservice.getTranslation("ObjectDuplicated");
        this.showWidget();
        setTimeout(() => this.successMessage = '', 5000);
      }, error => {
        this.errorMessage = error;
      });
  }

}

