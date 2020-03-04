import { Component, OnInit } from '@angular/core';
import {Dashboard, SharedDashboard} from "@app/dashboard/dashboard";
import {FormGroup, FormArray, FormControl, Validators} from "@angular/forms";
import {ActivatedRoute, Router} from "@angular/router";

import * as uuid from 'uuid';

import {DashboardService} from "@app/dashboard/dashboard.service";
import {Configuration, SkinSetting} from "@app/core/configuration";
import {TimeZoneService} from "@app/core/services";
import {Language} from "@app/shared/i18n/language";
import {I18nService} from "@app/shared/i18n/i18n.service";
import { TimeZone } from '@app/core/timeZone';

@Component({
  selector: 'app-dashboard-publish',
  templateUrl: './dashboard-publish.component.html',
})
export class DashboardPublishComponent implements OnInit {

  public dashboardForm: FormGroup;
  public dashboard: Dashboard;
  public errorMessage: string;
  public sharedDashboardCount = 0;
  public skinList: SkinSetting[];
  public languages: Language[];
  public timeZones: TimeZone[];
  public currentServerTimezone: string;

  constructor(private dashboardService: DashboardService, private route: ActivatedRoute, private router: Router,
    private configuration: Configuration, private i18nService: I18nService, private timeZoneService:TimeZoneService) { }

  ngOnInit() {

    this.skinList = this.configuration.skins;
    this.languages = this.configuration.languages;
    
    this.timeZoneService.getTimeZones()
      .subscribe(result => {
        this.timeZones = result;
      }, error => {
        this.errorMessage = error;
      });
  
    const id = this.route.snapshot.params['id'];

    if (id != null) {
      this.dashboardService.getById(+id)
        .subscribe(result => {
          this.dashboard = result;
          this.initCurrentTimeZone();
        }, error => {
          this.errorMessage = error;
        });
    }
  }

  initCurrentTimeZone() {

      this.timeZoneService.getCurrentTimeZone()
        .subscribe(result => {
          this.currentServerTimezone = result.id;
          this.initializeForm();
        }, error => {
          this.errorMessage = error;
        });
  }

  initializeForm() {

    this.dashboardForm = new FormGroup({
      'title': new FormControl(this.dashboard.title, [Validators.required]),
      'sharedDashboards': new FormArray([]),
    });

    if (this.dashboard.sharedDashboards != null) {
      this.dashboard.sharedDashboards.forEach(x => this.onAddSharedDashboard(x));
    }
  }

  onSubmitForm() {
    this.errorMessage = null;

    // stop here if form is invalid
    if (this.dashboardForm.invalid) {
      return;
    }

    const dashboardFormValue = <Dashboard>this.dashboardForm.value;
    this.dashboard.sharedDashboards = dashboardFormValue.sharedDashboards;

    this.dashboardService.put(this.dashboard.id, this.dashboard)
    .subscribe(result => {
      this.router.navigate(['/dashboard/dashboards']);
    },
    error => {
      this.errorMessage = error;
    });
  }

  onAddSharedDashboard(x: SharedDashboard): void {
    if (x == null) {
      x = new SharedDashboard();
      x.key = uuid.v4();
    }

    if (x.timeZone == null || x.timeZone == "") {
      x.timeZone = this.currentServerTimezone;
    }

    const formArray = this.getSharedDashboardsArray();
    formArray.push(new FormGroup({
      'id': new FormControl(x.id, [Validators.required]),
      'key': new FormControl(x.key, [Validators.required]),
      'codeLangue': new FormControl(x.codeLangue, [Validators.required]),
      'timeZone' : new FormControl(x.timeZone, [Validators.required]),
      'skin': new FormControl(x.skin, [Validators.required]),
      'isTestMode': new FormControl(x.isTestMode, [Validators.required]),
      'message': new FormControl(x.message), 
    }));
    this.sharedDashboardCount = formArray.length;
  }

  getSharedDashboardsArray(): FormArray {
    return this.dashboardForm.get('sharedDashboards') as FormArray;
  }

  onOpenDashboard(i: number) {
    const control = this.getSharedDashboardsArray();
    var win = window.open("/monitor/monitor/" + control.value[i].key, '_blank');
    win.focus();
  }

  onRemoveSharedDashboard(i: number) {
    const control = this.getSharedDashboardsArray();
    control.removeAt(i);
  }

  getTitle() {
    const dashboardFormValue = <Dashboard>this.dashboardForm.value;
    return dashboardFormValue.title;
  }
}
