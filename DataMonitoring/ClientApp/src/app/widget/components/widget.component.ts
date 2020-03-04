import { Component, OnInit, OnDestroy, ElementRef, Input, SimpleChanges, SimpleChange } from '@angular/core';
import { Observable, timer, Subscription } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

import * as moment from 'moment';
import 'chart.js';
import 'moment';

declare var $: any;

@Component({
  selector: 'app-monitorWidget',
  templateUrl: './widget.component.html'
})
export class WidgetComponent implements OnInit, OnDestroy {

  @Input() public id: number;
  @Input() public codeLangue: string;
  @Input() public timeZone: string;
  @Input() public refreshButton: boolean = false;
  @Input() public refreshAuto: boolean = false;
  @Input() public testMode: boolean;
  @Input() public key: string;
  @Input() public widgetIdList: string;
  @Input() public position: string;

  counter: number = 0;
  widgetList: number[];

  errorMessage: string;

  refreshTime: number = 0;
  title: string;
  titleFontSize: number;
  titleColorClass: string;
  lastRefreshTime: string;
  refreshContent: boolean = true;

  isRefresh: boolean = false;
  isRefreshContent: boolean = false;

  private timer;
  private timeSubscription: Subscription;

  constructor(public el: ElementRef, private http: HttpClient) {
  }

  ngOnInit() {

    if (this.refreshAuto) {
      this.onRefreshContent();
    }
  }

  ngOnChanges(changes: SimpleChanges) {

    const value: SimpleChange = changes.widgetIdList;

    if (value != null && value.currentValue != value.previousValue) {

      const list = value.currentValue.split(';');
      let index = 0;

      if (list.length > 0 && this.widgetList == null) {
        this.widgetList = new Array<number>();
      }

      list.forEach(element => {
        if (index < this.widgetList.length) {
          this.widgetList[index] = +element;
        } else {
          this.widgetList.push(+element);
        }
        index++;
      });

      if (this.widgetList.length > index) {
        const total = this.widgetList.length;
        for (let i = index; i < total; i++) {
          this.widgetList.pop();
        }
      }
    }
  }

  setTitleStyle() {
    const style = {
      'font-size': this.titleFontSize + 'px',
    };
    return style;
  }

  onRefresh() {
    this.isRefresh = true;
    this.errorMessage = '';

    this.loadingButton('loading');

    this.getWidgetContent(this.id)
      .subscribe(result => {

        this.initWidgetInformation(result);
        this.isRefresh = false;
        this.loadingButton('reset');
      },
        error => {
          this.errorMessage = error.statusText;
          this.isRefresh = false;
          this.loadingButton('reset');
        });
  }

  onRefreshContent() {
    this.isRefreshContent = true;
    this.errorMessage = '';

    if (this.timer != null) {
      this.timeSubscription.unsubscribe();
      this.timeSubscription = null;
      this.timer = null;
    }

    let id = this.id;

    if (this.widgetList != null) {
      if (this.counter < this.widgetList.length) {
        id = this.widgetList[this.counter];
      } else {
        this.counter = 0;
        id = this.widgetList[this.counter];
      }
    }

    this.getWidgetContent(id)
      .subscribe(result => {
        this.initWidgetInformation(result);
        this.isRefreshContent = false;

        const time = this.refreshTime * 60 * 1000;
        this.refreshContent = true;

        if (this.refreshTime == 0) {
          this.refreshContent = false;
        } else {
          if (this.widgetList != null) {
            this.counter++;
          }

          this.timer = timer(time);
          this.timeSubscription = this.timer.subscribe(val => this.onRefreshContent());
        }
      },
        error => {
          this.errorMessage = error.message;
          this.isRefreshContent = false;

          this.timer = timer(300000);
          this.timeSubscription = this.timer.subscribe(val => this.onRefreshContent());
        });
  }


  getWidgetContent(id: number): Observable<any> {

    let address = 'api/monitor/widget/' + id;

    if (this.testMode == true) {
      address = 'api/monitor/widget/test/' + id;
    } else {
      if (this.key) {
        address += '/' + this.key;
      }
    }

    if (this.codeLangue) {
      address += '?ui_locales=' + this.codeLangue;
      if (this.timeZone) {
        address += '&tz=' + this.timeZone;
      }
    } else {
      if (this.timeZone) {
        address += '?tz=' + this.timeZone;
      }
    }
    if (this.codeLangue || this.timeZone) {
      if (this.position) {
        address += '&position=' + this.position;
      }
    } else {
      if (this.position) {
        address += '?position=' + this.position;
      }
    }

    return this.http.get(address, { responseType: 'json' })
      .pipe(map((data: any) => (data.data || data)));
  }

  initWidgetInformation(data: any) {
    this.title = data.title;
    this.titleFontSize = data.titleFontSize;

    if (data.lastUpdateToDisplay != null) {
      this.lastRefreshTime = data.lastUpdateToDisplay;
    } else {
      this.lastRefreshTime = '';
    }

    this.refreshTime = data.refreshTime;
    this.titleColorClass = data.titleClassColor;

    if (this.refreshContent) {
      const $widget = $(this.el.nativeElement);

      if (data.widgetType < 3) {
        $widget.find('.widget-body').fadeOut("slow", function () {
          $widget.find('.widget-body').html(data.content);
        });

        $widget.find('.widget-body').fadeIn("slow");
      } else {
        $widget.find('.widget-body').html(data.content);
      }
    }
  }

  loadingButton(state: any) {
    const $widget = $(this.el.nativeElement);
    $widget.find('.jarviswidget-refresh-btn').button(state);
  }

  ngOnDestroy() {
    this.refreshTime = 0;
    if (this.timer != null) {
      this.timeSubscription.unsubscribe();
    }
  }
}
