import { Component, OnInit, Input } from '@angular/core';
import { FormGroup } from '@angular/forms';

import {Indicator} from "../../indicator";
import {IndicatorService} from "../../indicator.service";

@Component({
  selector: 'app-indicator-calculated',
  templateUrl: './indicator-calculated.component.html'
})
export class IndicatorCalculatedComponent implements OnInit {

  @Input() indicatorCalculated: FormGroup;
  @Input() errorMessage: string;

  public indicators: Indicator[];
  
  constructor(private indicatorsService: IndicatorService) { }

  ngOnInit() {

    // récupération les indicateurs déjà existant
    this.indicatorsService.get()
      .subscribe(result => {
        // Uniquement des Indicateurs de type IndicatorType.Flow
        this.indicators = result.filter(x => x.type == 1);
      }, error => {
        this.errorMessage = error;
      });
  }

}
