import { Component, Input, AfterViewInit } from '@angular/core';

import jarvisWidgetsDefaults from '../widget.defaults';
import { ElementRef } from "@angular/core";

import './jarvis.widget.ng2.js'

declare var $: any;

@Component({

  selector: 'app-widgets-grid',
  template: `
     <section id="widgets-grid" class="sortable-grid">
       <ng-content></ng-content>
     </section>
  `,
  styles: []
})
export class WidgetsGridComponent implements AfterViewInit {

  @Input() public timestampFormat: string;

  constructor(public el: ElementRef) { }

  ngAfterViewInit() {
    if (this.timestampFormat != null) {
      jarvisWidgetsDefaults.timestampFormat = this.timestampFormat;

    }
    $('#widgets-grid', this.el.nativeElement).jarvisWidgets(jarvisWidgetsDefaults);
  }

}
