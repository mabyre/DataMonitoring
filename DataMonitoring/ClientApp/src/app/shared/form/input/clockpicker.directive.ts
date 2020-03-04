import {Directive, ElementRef, OnInit, Input} from '@angular/core';
import {FormGroup} from "@angular/forms";

@Directive({
  // tslint:disable-next-line:directive-selector
  selector: '[clockpicker]'
})
export class ClockpickerDirective implements OnInit {

  @Input() clockpicker: any;
  @Input() model: FormGroup;

  constructor(private el:ElementRef) {
  }

  ngOnInit() {
    import('clockpicker/dist/bootstrap-clockpicker.min.js').then(()=> {
      this.render();
    });
  }

  render() {
    $(this.el.nativeElement).clockpicker(this.clockpicker || {
      placement: 'top',
      donetext: 'Done',
      afterDone: () => {
        this.model.patchValue(this.el.nativeElement.value);
      }
    });
  }

}
