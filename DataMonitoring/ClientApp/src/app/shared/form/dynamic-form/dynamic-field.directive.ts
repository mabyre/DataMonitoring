import {
    ComponentFactoryResolver,
    Directive,
    Input,
    OnInit,
    ViewContainerRef
  } from "@angular/core";
  import { FormGroup } from "@angular/forms";

import {FieldConfig} from "./field.interface";
import {InputComponent} from "./components/input.component";
import {SubmitButtonComponent} from "./components/submit-button.component";
import {SelectComponent} from "./components/select.component";
import {CheckboxComponent} from "./components/checkbox.component";
import {TextareaComponent} from "./components/textarea.component";
import {BackButtonComponent} from "./components/back-button.component";
  
  const componentMapper = {
    input: InputComponent,
    submitButton: SubmitButtonComponent,
    backButton: BackButtonComponent,
    select: SelectComponent,
    checkbox: CheckboxComponent,
    textarea: TextareaComponent
  };
  @Directive({
    // tslint:disable-next-line:directive-selector
    selector: "[dynamicField]"
  })
  export class DynamicFieldDirective implements OnInit {
    @Input() field: FieldConfig;
    @Input() group: FormGroup;
    componentRef: any;
    constructor(
      private resolver: ComponentFactoryResolver,
      private container: ViewContainerRef
    ) {}
    ngOnInit() {
      const factory = this.resolver.resolveComponentFactory(
        componentMapper[this.field.type]
      );
      this.componentRef = this.container.createComponent(factory);
      this.componentRef.instance.field = this.field;
      this.componentRef.instance.group = this.group;
    }
  }

