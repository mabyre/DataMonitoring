import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import {FormGroup, FormControl, Validators, FormArray} from "@angular/forms";
import {DashboardService} from "@app/dashboard/dashboard.service";
import {ActivatedRoute, Router} from "@angular/router";
import {I18nService} from "@app/shared/i18n/i18n.service";
import {Dashboard, DashboardLocalization, DashboardWidget} from "@app/dashboard/dashboard";
import {Color} from "@app/shared/models/color";
import {WidgetsService} from "@app/widget/widget.service";
import {Widget} from "@app/widget/models/widget";

@Component({
  selector: 'app-dashboard-edit',
  templateUrl: './dashboard-edit.component.html',
  styleUrls: ['./dashboard-edit-ngx-dnd.component.scss'],
  encapsulation: ViewEncapsulation.None  
})
export class DashboardEditComponent implements OnInit {

  public dashboardForm: FormGroup;
  public dashboard: Dashboard;
  public titleView: string;
  public errorMessage: string;
  public dashboardLocalizationCount = 0;
  public dashboardWidgetCount = 0;
  public colorList: Color[];
  public widgetList: Widget[];

  public positionList: number[] = [1, 2];

  constructor(private dashboardService: DashboardService, private widgetService: WidgetsService, private route: ActivatedRoute,
    private router: Router, private i18nService: I18nService) { }

  public currentCount = 1;
  public itemSaved = '';

  sourceBuilderTools2 = [
    { name: 'Section', children: [] as any[], inputType: 'section', icon: 'far fa-square', class: 'wide' },
    { name: 'String', inputType: 'string', icon: 'fas fa-language', class: 'half' },
    { name: 'Number', inputType: 'number', icon: 'fas fa-hashtag', class: 'half' }
  ];
  targetBuilderTools2: any[] = [];
  ngOnInit() {

    this.dashboardService.getColorList()
      .subscribe(result => {
        this.colorList = result;
      }, error => {
        this.errorMessage = error;
      });

    this.widgetService.get()
      .subscribe(data => {
        this.widgetList = data;
      }, error => {
        this.errorMessage = error;
      });

    // l'objet snapshot contient les paramètres de l'URL
    const id = this.route.snapshot.params['id'];

    if (id != null) {
      // mode edition :
      this.titleView = this.i18nService.getTranslation('Edit');
      this.dashboardService.getById(+id)
        .subscribe(result => {
          this.dashboard = result;
          this.initializeForm();
        }, error => {
          this.errorMessage = error;
        });
    } else {
      // mode ajout :
      this.titleView = this.i18nService.getTranslation('Add');
      this.dashboard = new Dashboard();
      this.initializeForm();
    }
  }
  //-----------------------

  droppableItemClass = (item: any) => `${item.class} ${item.inputType}`;

  builderDrag(e: any) {
    const item = e.value;
    item.data =
      item.inputType === 'number'
        ? (Math.random() * 100) | 0
        : Math.random()
          .toString(36)
          .substring(20);
  }

  builderDrop(e: any) {
    const item = e.value;
    const iel = e.el.shildElementCount;

    if (item.inputType === 'number') {
      item.data = (Math.random() * 100)
    }
    //item.data =
    //  item.inputType === 'number'
    //    ? (Math.random() * 100) | 0
    //    : Math.random()
    //      .toString(36)
    //      .substring(20);

    if (item.inputType === 'string') {
      if (item.data == '') {
        item.data = this.currentCount++;
      }
    }
  }

  public saveForm() {
    const item = this.targetBuilderTools2[0];

    for (var i = 0; i < this.targetBuilderTools2.length; i++) {
      this.itemSaved += i.toString() + ':' + this.targetBuilderTools2[i].data + ' | ';
    }
  }

  log(e: any) {
    console.log(e.type, e);
  }

  canMove(e: any): boolean {
    return e.indexOf('Disabled') === -1;
  }

  //-----------------------

  initializeForm() {
    this.dashboardForm = new FormGroup({
      'title': new FormControl(this.dashboard.title, [Validators.required]),
      'titleDisplayed': new FormControl(this.dashboard.titleDisplayed, [Validators.required]),
      'titleColorName': new FormControl(this.dashboard.titleColorName),
      'currentTimeManagementDisplayed': new FormControl(this.dashboard.currentTimeManagementDisplayed),
      'widgets': new FormArray([]),
      'dashboardLocalizations': new FormArray([]),
    });

    if (this.dashboard.dashboardLocalizations != null) {
      this.dashboard.dashboardLocalizations.forEach(x => this.onAddDashboardLocalization(x));
    }

    if (this.dashboard.widgets != null) {
      this.dashboard.widgets.forEach(x => this.onAddDashboardWidget(x));
    }
  }

  onSubmitForm() {
    this.errorMessage = null;

    // stop here if form is invalid
    if (this.dashboardForm.invalid) {
      return;
    }

    const dashboardFormValue = <Dashboard>this.dashboardForm.value;
    this.dashboard.title = dashboardFormValue.title;
    this.dashboard.titleColorName = dashboardFormValue.titleColorName;
    this.dashboard.titleDisplayed = dashboardFormValue.titleDisplayed;
    this.dashboard.currentTimeManagementDisplayed = dashboardFormValue.currentTimeManagementDisplayed;
    this.dashboard.dashboardLocalizations = dashboardFormValue.dashboardLocalizations;
    this.dashboard.widgets = dashboardFormValue.widgets;

    this.errorMessage = this.widgetsAreValid(this.dashboard.widgets);

    if (this.errorMessage == null) {
      if (this.dashboard.id === 0) {
        this.dashboardService.post(this.dashboard)
          .subscribe(result => {
            this.router.navigate(['/dashboard/dashboards']);
          },
          error => {
            this.errorMessage = error;
          });
      } else {
        this.dashboardService.put(this.dashboard.id, this.dashboard)
          .subscribe(result => {
            this.router.navigate(['/dashboard/dashboards']);
          },
          error => {
            this.errorMessage = error;
          });
      }
    }
  }

  widgetsAreValid(widgets: DashboardWidget[]): string {
    let errorMessage = null;
    const widgetIdList: number[] = [];
    widgets.forEach(element => {
      if (widgetIdList.find(x => x == element.widgetId)) {
        errorMessage = this.i18nService.getTranslation('MultiplDashboardWidgetError');
      } else {
        widgetIdList.push(element.widgetId);
      }
    });
    return errorMessage;
  }



  onAddDashboardWidget(x: DashboardWidget): void {
    if (x == null) {
      x = new DashboardWidget();
    }
    const formArray = this.getDashboardWidgetsArray();
    formArray.push(new FormGroup({
      'widgetId': new FormControl(x.widgetId, [Validators.required, Validators.pattern('^(?!0).*')]),
      'column': new FormControl(x.column, [Validators.required, Validators.min(1)]),
      'position': new FormControl(x.position, [Validators.required, Validators.min(1)])
    }));
    this.dashboardWidgetCount = formArray.length;
  }

  onAddDashboardLocalization(x: DashboardLocalization): void {
    if (x == null) {
      x = new DashboardLocalization();
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
    return this.dashboardForm.get('dashboardLocalizations') as FormArray;
  }

  getDashboardWidgetsArray(): FormArray {
    return this.dashboardForm.get('widgets') as FormArray;
  }

  onRemoveDashboardWidget(i: number) {
    const control = this.getDashboardWidgetsArray();
    control.removeAt(i);
  }

  getTitleToDisplay(i: number) {
    const formArray = this.getDashboardWidgetsArray();
    const control = formArray.controls[i];
    const selectedWidgetId = control.get('widgetId').value;
    if (selectedWidgetId != null && selectedWidgetId != 0) {
      if (this.widgetList != null) {
        const widgetTitle = this.widgetList.find(x => x.id == selectedWidgetId).title;
        return widgetTitle;
      }
    } else {
      return "NEW";
    }

  }

  getCoordinateToDisplay(i: number) {
    const formArray = this.getDashboardWidgetsArray();
    const control = formArray.controls[i];
    const selectedWidgetId = control.get('widgetId').value;
    if (selectedWidgetId != null && selectedWidgetId != 0) {
      const column = control.get('column').value;
      const position = control.get('position').value;
      return "" + position + " . " + column + "";
    }
  }
}
