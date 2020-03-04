import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import {FieldConfig} from "../field.interface";


@Component({
  selector: "app-checkbox",
  template: `
<div [formGroup]="group" class="form-group" >
  <div class="checkbox">
    <label class="checkbox">
      <input type="checkbox" [formControlName]="field.name">
      {{field.label}}
    </label>
  </div>
</div>
`,
  styles: []
})
export class CheckboxComponent implements OnInit {
  field: FieldConfig;
  group: FormGroup;
  constructor() {}
  ngOnInit() {}
}
