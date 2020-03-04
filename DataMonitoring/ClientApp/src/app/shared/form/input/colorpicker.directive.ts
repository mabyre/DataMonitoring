import { Directive, Input, ElementRef, OnInit } from "@angular/core";

import "bootstrap-colorpicker/dist/js/bootstrap-colorpicker.js";
import {FormGroup} from "@angular/forms";

@Directive({
  selector: "[appColorpicker]"
})
export class ColorpickerDirective implements OnInit {

  @Input() appColorpicker: any;
  @Input() model: FormGroup;

  constructor(private el: ElementRef) {}

  ngOnInit() {
    
    this.render();

  }

  render() {

    $(this.el.nativeElement).colorpicker(this.appColorpicker || {
      colorpickerChange: () => {
        this.model.patchValue(this.el.nativeElement.value);
      }
    });
  }
}
