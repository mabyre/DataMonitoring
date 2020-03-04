import { Component, OnInit, Input } from '@angular/core';
import {FormGroup} from "@angular/forms";

@Component({
  selector: 'app-time-range',
  templateUrl: './time-range.component.html'
})
export class TimeRangeComponent implements OnInit {

  @Input() timeRangeControl: FormGroup;
  @Input() index: number;

  constructor() { }

  ngOnInit() {
  }

}
