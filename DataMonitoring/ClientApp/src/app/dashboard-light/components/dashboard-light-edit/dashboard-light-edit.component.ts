import { Component, OnInit } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { FormGroup, FormControl, Validators, FormArray } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { I18nService } from "@app/shared/i18n/i18n.service";
import { DashboardLight, DashboardLightLocalization } from "@app/dashboard-light/dashboard-light";
import {Color} from "@app/shared/models/color";
import { DashboardLightService } from "@app/dashboard-light/dashboard-light.service";

@Component({
  selector: 'app-dashboard-light-edit',
  templateUrl: './dashboard-light-edit.component.html',
})
export class DashboardLightEditComponent implements OnInit {

  public dashboarLightdForm: FormGroup;
  public dashboardLight: DashboardLight;
  public dashboardLocalizationCount = 0;
  public errorMessage: string;
  public colorList: Color[];
  
  constructor(private dashboardLightService: DashboardLightService,
    private route: ActivatedRoute,
    private router: Router,
    private i18nService: I18nService) { }

  ngOnInit() {
    this.dashboardLightService.getColorList()
      .subscribe(result => {
        this.colorList = result;
      }, error => {
        this.errorMessage = error;
      });
    // snapshot contient les paramètres de l'URL
    const id = this.route.snapshot.params['id'];
    if (id != null) {
      this.dashboardLightService.getById(+id)
        .subscribe(result => {
          this.dashboardLight = result;
          this.initializeForm();
        }, error => {
          this.errorMessage = error;
        });
    }
  }

  initializeForm() {
    this.dashboarLightdForm = new FormGroup({
      'title': new FormControl(this.dashboardLight.title, [Validators.required]),
      'titleDisplayed': new FormControl(this.dashboardLight.titleDisplayed, [Validators.required]),
      'titleColorName': new FormControl(this.dashboardLight.titleColorName),
      'currentTimeManagementDisplayed': new FormControl(this.dashboardLight.currentTimeManagementDisplayed),
      'dashboardLocalizations': new FormArray([]),
    });

    if (this.dashboardLight.dashboardLocalizations != null) {
      this.dashboardLight.dashboardLocalizations.forEach(x => this.onAddDashboardLocalization(x));
    }
  }

  onSubmitForm() {
    this.errorMessage = null;

    // stop here if form is invalid
    if (this.dashboarLightdForm.invalid) {
      return;
    }

    const dashboardLightFormValue = <DashboardLight>this.dashboarLightdForm.value;
    this.dashboardLight.title = dashboardLightFormValue.title;
    this.dashboardLight.titleColorName = dashboardLightFormValue.titleColorName;
    this.dashboardLight.titleDisplayed = dashboardLightFormValue.titleDisplayed;
    this.dashboardLight.currentTimeManagementDisplayed = dashboardLightFormValue.currentTimeManagementDisplayed;
    this.dashboardLight.dashboardLocalizations = dashboardLightFormValue.dashboardLocalizations;

    if (this.dashboarLightdForm.controls.title.valid == true) {
      this.dashboardLightService.put(this.dashboardLight.id, this.dashboardLight)
        .subscribe(result => {
          this.router.navigate(['/dashboard/dashboards']);
        },
          error => {
            this.errorMessage = error;
          });
    }
  }

  onAddDashboardLocalization(x: DashboardLightLocalization): void {
    if (x == null) {
      x = new DashboardLightLocalization();
    }
    const formArray = this.getDashboardLocalizationsArray();
    formArray.push(new FormGroup({
      'id': new FormControl(x.id),
      'localizationCode': new FormControl(x.localizationCode, [Validators.required]),
      'title': new FormControl(x.title, [Validators.required, Validators.maxLength(200)])
    }));
    this.dashboardLocalizationCount = formArray.length;
  }

  getDashboardLocalizationsArray(): FormArray {
    return this.dashboarLightdForm.get('dashboardLocalizations') as FormArray;
  }
}
