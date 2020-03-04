import {Component, OnInit, OnDestroy, ElementRef, Input, AfterViewInit, HostBinding} from '@angular/core';
import {Router} from "@angular/router";

declare var $: any;

@Component({
  selector: 'app-widget,[app-widget]',
  template: `<ng-content></ng-content>`
})
export class WidgetComponent implements OnInit, OnDestroy, AfterViewInit {

  @HostBinding('attr.id') public widgetId: string;
  

  @Input() public name: string;
  @Input() public colorbutton: boolean = true;
  @Input() public editbutton: boolean = true;
  @Input() public togglebutton: boolean = true;
  @Input() public deletebutton: boolean = true;
  @Input() public fullscreenbutton: boolean = true;
  @Input() public custombutton: boolean = false;
  @Input() public collapsed: boolean = false;
  @Input() public sortable: boolean = true;
  @Input() public hidden: boolean = false;
  @Input() public color: string;
  @Input() public load: string;
  @Input() public refresh: boolean = false;
  @Input() public refreshTime: number = 0;


  static counter: number = 0;

  constructor(public el: ElementRef, private router: Router) {

  }

  ngOnInit() {
    this.widgetId = this.genId();


    let widget = this.el.nativeElement;
    widget.className += ' jarviswidget';
    if (this.sortable) {
      widget.className += ' jarviswidget-sortable';
    }

    if (this.color) {
      widget.className += (' jarviswidget-color-' + this.color);
    }

    ['colorbutton',
      'editbutton',
      'togglebutton',
      'deletebutton',
      'fullscreenbutton',
      'custombutton',
      'sortable'
    ].forEach((option) => {
      if (!this[option]) {
        widget.setAttribute('data-widget-' + option, 'false')
      }
    });

    [
      'hidden',
      'collapsed'
    ].forEach((option) => {
      if (this[option]) {
        widget.setAttribute('data-widget-' + option, 'true')
      }
      });

    if (this.load != null) {
      widget.setAttribute('data-widget-load', this.load);
    }

    if (this.refreshTime != 0) {
      widget.setAttribute('data-widget-refresh', this.refreshTime);
    }
  }

  private genId() {
    if (this.name) {
      return this.name
    } else {
      let heading = this.el.nativeElement.querySelector('header h2');
      let id = heading ? heading.textContent.trim() : 'jarviswidget-' + WidgetComponent.counter++;
      id = id.toLowerCase().replace(/\W+/gm, '-');

      let url = this.router.url.substr(1).replace(/\//g, '-');
      id = url + '--' + id;

      return id
    }
  }

  ngAfterViewInit(): any {
    const $widget = $(this.el.nativeElement);

    if (this.editbutton) {
      $widget.find('.widget-body').prepend('<div class="jarviswidget-editbox"><input class="form-control" type="text"></div>');
    }
  }

  ngOnDestroy(){

  }

}
