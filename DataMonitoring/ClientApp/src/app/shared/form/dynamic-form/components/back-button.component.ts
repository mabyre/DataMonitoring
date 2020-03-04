import { Component, OnInit } from "@angular/core";
import { FormGroup } from "@angular/forms";
import {FieldConfig} from "../field.interface";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: "app-button",
  template: `
<div [formGroup]="group">
<button type="button" class="btn btn-default" [routerLink]="field.route" ><i class="fa fa-chevron-circle-left"></i> {{field.label}}</button>
</div>
`,
  styles: []
})
export class BackButtonComponent implements OnInit {

  field: FieldConfig;
  group: FormGroup;
  constructor(private router: Router, private route: ActivatedRoute) { }
  ngOnInit() { }

}
