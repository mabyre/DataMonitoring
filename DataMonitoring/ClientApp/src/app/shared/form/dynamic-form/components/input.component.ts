import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import {FieldConfig} from "../field.interface";


@Component({
  selector: "app-input",
  template: `
<div [formGroup]="group" class="form-group">
  <label [for]="field.name">{{field.label}}</label>
  <input [type]="field.inputType" [formControlName]="field.name" class="form-control"/>
  <ng-container *ngFor="let validation of field.validations;">
    <div *ngIf="group.get(field.name).hasError(validation.name)" class="text-danger">
      {{validation.message}}
    </div>
  </ng-container>
</div>
`,
  styles: []
})
export class InputComponent implements OnInit {
  field: FieldConfig;
  group: FormGroup;
  constructor() {}
  ngOnInit() {}
}
