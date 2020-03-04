import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import {FieldConfig} from "../field.interface";


@Component({
  selector: "app-button",
  template: `
<div [formGroup]="group">
<button type="submit" class="btn btn-default" [disabled]="group.invalid"><i class="fa fa-save"></i> {{field.label}}</button>
</div>
`,
  styles: []
})
export class SubmitButtonComponent implements OnInit {
  field: FieldConfig;
  group: FormGroup;
  constructor() { }
  ngOnInit() { }
}
