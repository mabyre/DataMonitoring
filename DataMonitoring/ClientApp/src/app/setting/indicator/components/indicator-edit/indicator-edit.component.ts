import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormBuilder, Validators, FormArray, FormControl } from '@angular/forms';

import {Indicator, QueryConnector, CalculatedIndicator} from "../../indicator";
import {I18nService} from "@app/shared/i18n/i18n.service";
import {ConnectorService} from "@app/setting/connector/connector.service";
import {IndicatorService} from "../../indicator.service";
import {Connector} from "@app/setting/connector/connector";
import {TitleLocalization} from "@app/shared/form/title-localization/title-localization";
import {TimeManagementService} from "@app/setting/time-management/time-management-service";
import {TimeManagement} from "@app/setting/time-management/time-management";


@Component({
  selector: 'app-indicator-edit',
  templateUrl: './indicator-edit.component.html'
})
export class IndicatorEditComponent implements OnInit {

  public titleView: string;
  public indicatorForm: FormGroup;
  public indicator: Indicator;
  public errorMessage: string;
  public connectorList: Connector[];
  public indicatorTypes: any[];
  public indicators: Indicator[];
  public indicatorOperations: any[];
  public isAddMode: boolean;
  public traductionAvailableCount: number = 0;
  public tabnumber: number = 0;
  public queryConnectorsCount: number = 0;
  public timeManagements: TimeManagement[];

  constructor(private route: ActivatedRoute, private formBuilder: FormBuilder,
    private indicatorsService: IndicatorService, private connectorsService: ConnectorService,
    private router: Router, private i18nService: I18nService, private timeManagementService: TimeManagementService) {}

  ngOnInit() {
    // récupération des connecteurs enregistrés pour pouvoir en choisir un ou plusieurs
    this.connectorsService.get()
      .subscribe(result => {
        this.connectorList = result;
      }, error => {
        this.errorMessage = error;
      });

    // récupération les TimeManagement déjà existant
    this.timeManagementService.get()
      .subscribe(result => {
        this.timeManagements = result;
      }, error => {
        this.errorMessage = error;
      });

    this.indicatorsService.getIndicatorTypes()
      .subscribe(result => {
        this.indicatorTypes = result;
      }, error => {
        this.errorMessage = error;
      });

    const id = this.route.snapshot.params['id'];

    if (id != null) {
      // mode edition :
      this.isAddMode = false;
      this.titleView = this.i18nService.getTranslation('Edit');
      this.indicatorsService.getById(+id)
        .subscribe(result => {
          this.indicator = result;
          this.traductionAvailableCount = result.titleLocalizations != null ? result.titleLocalizations.length : 0;
          this.queryConnectorsCount = result.queryConnectors != null ? result.queryConnectors.length : 0;
          this.initForm();
        }, error => {
          this.errorMessage = error;
        });
    } else {
      // mode ajout :
      this.isAddMode = true;
      this.titleView = this.i18nService.getTranslation('Add');
      this.initForm();
    }
  }

  initForm() {
    this.indicatorForm = this.formBuilder.group({
      title: [this.indicator != null ? this.indicator.title : '', Validators.required],
      titleLocalizations: this.formBuilder.array([]),
      refreshTime: [this.indicator != null ? this.indicator.refreshTime : '5', Validators.required],
      delayForDelete: [this.indicator != null ? this.indicator.delayForDelete : '5', Validators.required],
      type: [this.indicator != null ? this.indicator.type : 0, Validators.required],
      indicatorCalculated: this.indicator != null && this.indicator.indicatorCalculated != null
        ? this.formBuilder.array([this.createIndicatorCalculatedGroup(this.indicator.indicatorCalculated)])
        : this.formBuilder.array([this.createEmptyIndicatorCalculatedGroup()]),
      queryConnectors: this.formBuilder.array([]),
      timeManagementId: [this.indicator != null && this.indicator.timeManagementId != null 
          ? this.indicator.timeManagementId 
          : ''],
    });
    
    if (this.indicator != null && this.indicator.queryConnectors != null) {
      this.initializeQueryConnectorGroup(this.indicator.queryConnectors);
    }
    if (this.indicator != null && this.indicator.titleLocalizations != null) {
      this.initializeTitleLocalizationGroup(this.indicator.titleLocalizations);
    }
  }

  onResetForm() {
    const queryConnectors = this.getQueryConnectorsArray();
    while (queryConnectors.length) {
      queryConnectors.removeAt(0);
    }
    const titleLocalizations = this.getTitleLocalizationsArray();
    while (titleLocalizations.length) {
      titleLocalizations.removeAt(0);
    }
    this.indicatorForm.reset();
    this.indicatorForm.patchValue({
      type: 0,
      refreshTime: 5,
      delayForDelete:5,
    });
  }

  onSubmitForm() {
    this.errorMessage = null;
    const formValue = this.indicatorForm.value;

    // console.log('-----Form in JSON Format-----');
    // console.log(JSON.stringify(formValue));
    // console.log('-----------------------------');

    const indicatorForm: Indicator = formValue;

    if (this.indicator == null) {
      this.indicator = new Indicator();
    }
    this.indicator.title = indicatorForm.title;
    this.indicator.refreshTime = indicatorForm.refreshTime;
    this.indicator.delayForDelete = indicatorForm.delayForDelete;
    this.indicator.type = indicatorForm.type;
    this.indicator.queryConnectors = indicatorForm.queryConnectors;
    this.indicator.titleLocalizations = indicatorForm.titleLocalizations;
    this.indicator.timeManagementId = indicatorForm.timeManagementId;
    if (this.indicator.type == 2) { // Type calculé
      this.indicator.indicatorCalculated = indicatorForm.indicatorCalculated[0];
    }

    if (this.indicator.queryConnectors != null) {
      this.indicator.queryConnectors.forEach(element => {
        const error = this.indicatorsService.IsValidQuery(element.query);
        if (error != null) {
          this.errorMessage = error;
          return;
        }
      });
    }

    // Pour les types Table et Value : 
    if (this.indicator.type != 2) {
      // Gestion du temps obligatoire 
      if (this.indicator.timeManagementId == null || this.indicator.timeManagementId == 0) {
        this.errorMessage = this.i18nService.getTranslation('TimeManagementRequired');
        return;
      }
      // Connecteur obligatoire
      if (this.indicator.queryConnectors == null || this.indicator.queryConnectors.length == 0 || 
          this.indicator.queryConnectors[0].connectorId == 0 || this.indicator.queryConnectors[0].query == "") {
        this.errorMessage = this.i18nService.getTranslation('ConnectorRequired');
        return;
      }
    }

    // Pour le type Calculé : 
    if (this.indicator.type == 2) {
      // Il faut obligatoirement choisir au moins 1 indicateur et un opérateur
      if (this.indicator.indicatorCalculated == null || this.indicator.indicatorCalculated.indicatorOneId == 0 || this.indicator.indicatorCalculated.indicatorTwoId == 0) {
        this.errorMessage = this.i18nService.getTranslation('TwoIndicatorsRequired');
        return;
      }
    }

    if (this.errorMessage == null) {
      this.updateOrAddIndicator(this.indicator);
    }
  }

  private updateOrAddIndicator(indicator: Indicator) {
    console.log(indicator);
    this.indicatorsService.post(indicator)
      .subscribe(result => {
        this.router.navigate(['/indicator/indicators']);
      }, error => {
        this.errorMessage = error;
      });
  }

  //////////////////////////////////////////////////////////////////////
  // TITLE LOCALIZATION :
  //////////////////////////////////////////////////////////////////////
  initializeTitleLocalizationGroup(titleLocalizations: TitleLocalization[]) {
    const array = this.getTitleLocalizationsArray();
    titleLocalizations.forEach(element => {
      array.push(this.createTitleLocalizationGroup(element));
    });
  }

  createTitleLocalizationGroup(element: TitleLocalization) {
    return this.formBuilder.group({
      localizationCode: [element.localizationCode],
      title: [element.title]
    });
  }

  getTitleLocalizationsArray(): FormArray {
    return this.indicatorForm.get('titleLocalizations') as FormArray;
  }
  //////////////////////////////////////////////////////////////////////

  //////////////////////////////////////////////////////////////////////
  // QUERY CONNECTOR :
  //////////////////////////////////////////////////////////////////////
  createEmptyQueryConnectorGroup() {
    return this.formBuilder.group({
        connectorId: [''],
        query: ['']
    });
  }

  initializeQueryConnectorGroup(queryConnectors: QueryConnector[]) {
    const array = this.getQueryConnectorsArray();
    queryConnectors.forEach(element => {
      array.push(this.createQueryConnectorGroup(element));
    });
  }

  createQueryConnectorGroup(element: QueryConnector) {
    return this.formBuilder.group({
        connectorId: [element.connectorId],
        query: [element.query]
    });
  }

  getQueryConnectorsArray(): FormArray {
    return this.indicatorForm.get('queryConnectors') as FormArray;
  }

  onAddQueryConnector() {
    const array = this.getQueryConnectorsArray();
    array.push(this.createEmptyQueryConnectorGroup());
  }

  onRemoveQueryConnector(i: number) {
    const control = this.getQueryConnectorsArray();
    control.removeAt(i);
  }
  //////////////////////////////////////////////////////////////////////


  //////////////////////////////////////////////////////////////////////
  // CALCULATED INDICATOR :
  //////////////////////////////////////////////////////////////////////
  createEmptyIndicatorCalculatedGroup() {
    return this.formBuilder.group({
      indicatorOneId: [''],
      indicatorTwoId: [''],
    });
  }

  createIndicatorCalculatedGroup(indicatorCalculated: CalculatedIndicator) {
    return this.formBuilder.group({
      indicatorOneId: [indicatorCalculated.indicatorOneId],
      indicatorTwoId: [indicatorCalculated.indicatorTwoId],
    });
  }

  getIndicatorCalculatedArray(): FormArray {
    return this.indicatorForm.get('indicatorCalculated') as FormArray;
  }
  //////////////////////////////////////////////////////////////////////

}
