import {Directive,Input,ElementRef,OnInit,HostListener} from "@angular/core";

import "script-loader!smartadmin-plugins/bower_components/bootstrapvalidator/dist/js/bootstrapValidator.min.js";

declare var $: any;

@Directive({
  selector: "[appBootstrapValidator]"
})
export class BootstrapValidatorDirective implements OnInit {
  @Input() appBootstrapValidator: any;

  @HostListener("submit")
  s = () => {
    const bootstrapValidator = this.$form.data("bootstrapValidator");
    bootstrapValidator.validate();
    if (bootstrapValidator.isValid()) this.$form.submit();
    else return;
  };

  private $form: any;

  constructor(private el: ElementRef) { }

  ngOnInit() {
    this.$form = $(this.el.nativeElement);
    this.$form.bootstrapValidator(this.appBootstrapValidator || {});

    this.$form.submit(function (ev) {
      ev.preventDefault();
    });
  }
}
