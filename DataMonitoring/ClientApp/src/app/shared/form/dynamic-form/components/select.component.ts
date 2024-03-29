import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import {FieldConfig} from "../field.interface";


@Component({
  selector: "app-select",
  template: `
<div class="form-group" [formGroup]="group">
<label [for]="field.name">{{field.label}}</label>
  <select [formControlName]="field.name" class="form-control">
    <option *ngFor="let item of field.options" [value]="item">{{item}}</option>
  </select>
</div>
`,
  styles: []
})
export class SelectComponent implements OnInit {
  field: FieldConfig;
  group: FormGroup;
  constructor() {}
  ngOnInit() {}
}
