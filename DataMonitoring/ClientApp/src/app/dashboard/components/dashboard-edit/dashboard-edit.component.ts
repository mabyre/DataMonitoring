/*
 * Builder_Drag_And_Drop
 * Drag_And_Drop
 * TODO : put log in comment
 * 
 */
import { Component, OnInit, ViewEncapsulation } from '@angular/core';
import { FormGroup, FormControl, Validators, FormArray } from "@angular/forms";
import { DashboardService } from "@app/dashboard/dashboard.service";
import { ActivatedRoute, Router } from "@angular/router";
import { I18nService } from "@app/shared/i18n/i18n.service";
import { Dashboard, DashboardLocalization, DashboardWidget } from "@app/dashboard/dashboard";
import { Color } from "@app/shared/models/color";
import { WidgetsService } from "@app/widget/widget.service";
import { Widget } from "@app/widget/models/widget";

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

  errorMessage: string;
  successMessage: string;

  public dashboardLocalizationCount = 0;
  public dashboardWidgetCount = 0;
  public colorList: Color[];
  public widgetList: Widget[];

  public positionList: number[] = [1, 2];

  constructor(private dashboardService: DashboardService, private widgetService: WidgetsService, private route: ActivatedRoute,
    private router: Router, private i18nService: I18nService) { }

  public currentCount = 1;
  public itemSaved = '';

  //sourceBuilderTools2 = [
  //  { name: 'Section', children: [] as any[], inputType: 'section', icon: 'far fa-square', class: 'wide' },
  //  { name: 'String', inputType: 'string', icon: 'fas fa-language', class: 'half' },
  //  { name: 'Number', inputType: 'number', icon: 'fas fa-hashtag', class: 'half' }
  //];

  public targetBuilderItemList: any[] = [];
  //public targetBuilderItemList: DashboardWidget[];

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

    if (this.dashboard.widgets != null) {
      // Builder_Drag_And_Drop
      this.pushTargetBuilderItemsList();
    }
  }

  onSubmitForm() {
    this.errorMessage = '';
    this.successMessage = '';

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
    this.saveFormForDragAndDrop();

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
          // BRY retour a la lite
          //.subscribe(result => {
          //  this.router.navigate(['/dashboard/dashboards']);
          //},
          //  error => {
          //    this.errorMessage = error;
          //  });
          .subscribe(result => {
            this.successMessage = this.i18nService.getTranslation('Mise à jour effectuée...');
            setTimeout(() => this.successMessage = '', 5000);
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
    const formArray = this.getDashboardWidgetsArray();
    const control = formArray.controls[i];
    const selectedWidgetId = control.get('widgetId').value;

    // suppress in form
    formArray.removeAt(i);

    // suppress in builder DragAndDrop Source
    this.dashboard.widgets.splice(i, 1);

    // suppress in builder DragAndDrop Target
    const index = this.targetBuilderItemList.findIndex(x => x.widgetId == selectedWidgetId);
    this.targetBuilderItemList.splice(index, 1);

    this.successMessage = this.i18nService.getTranslation('Wigdet supprimé...');
    setTimeout(() => this.successMessage = '', 5000);
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

  getTitleToDisplayInTemplate(id: number) {
    if (this.widgetList != null) {
      const widgetTitle = this.widgetList.find(x => x.id == id).title;
      return widgetTitle;
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

  getCoordinateToDisplayInTemplate(id: number) {
    if (this.widgetList != null) {
      const widget = this.dashboard.widgets.find(x => x.widgetId == id);
      const widgetCol = widget.column;
      const widgetPos = widget.position;
      return "" + widgetPos + " . " + widgetCol + "";
    }
  }

  //----------------------- Drag_And_Drop

  //droppableItemClass = (item: any) => `${item.class} ${item.inputType}`;
  droppableItemClass = (item: any) => `half`;

  builderSourceDrag(e: any) {
    const item = e.value;

    this.eLog('source->', e);

    //item.data =
    //  item.inputType === 'number'
    //    ? (Math.random() * 100) | 0
    //    : Math.random()
    //      .toString(36)
    //      .substring(20);
  }

  builderTargetDrop(e: any) {
    const item = e.value;
    const iel = e.el.shildElementCount;
    const dropi = e.dropIndex;
    const id = item.widgetId;

    this.eLog('target->', e);

    // if (this.widgetList != null) {
    //   const widget = this.dashboard.widgets.find(x => x.widgetId == id);

    //   if (dropi == 0) {
    //     widget.position = 1;
    //     widget.column = 1;
    //   }
    //   else if (dropi == 1) {
    //     widget.position = 1;
    //     widget.column = 2;
    //   }
    //   else if (dropi == 2) {
    //     widget.position = 2;
    //     widget.column = 1;
    //   }
    //   else if (dropi == 3) {
    //     widget.position = 2;
    //     widget.column = 2;
    //   }
    //   else {
    //     this.errorMessage = "Pas plus de 4 widgets";
    //   }
    // }

    // if (item.inputType === 'number') {
    //   item.data = (Math.random() * 100)
    // }

    //item.data =
    //  item.inputType === 'number'
    //    ? (Math.random() * 100) | 0
    //    : Math.random()
    //      .toString(36)
    //      .substring(20);

    // if (item.inputType === 'string') {
    //   if (item.data == '') {
    //     item.data = this.currentCount++;
    //   }
    // }
  }

  public pushTargetBuilderItemsList() {
    for (var i = 1; i <= 2; i++) {
      for (var j = 1; j <= 2; j++) {
        const widget = this.dashboard.widgets.find( x => ( x.position == i && x.column == j )  );
        if (widget != null)
          this.targetBuilderItemList.push(widget);
      }
    }
  }

  public saveFormForDragAndDrop() {

    for (var i = 0; i < this.targetBuilderItemList.length; i++) {
      const newWidget = this.targetBuilderItemList[i];
      const widgetId = newWidget.widgetId;

      const widget = this.dashboard.widgets.find( x => x.widgetId == widgetId);

      if (i == 0) {
        widget.position = 1;
        widget.column = 1;
      }
      else if (i == 1) {
        widget.position = 1;
        widget.column = 2;
      }
      else if (i == 2) {
        widget.position = 2;
        widget.column = 1;
      }
      else if (i == 3) {
        widget.position = 2;
        widget.column = 2;
      }
      else {
        this.errorMessage = "Pas plus de 4 widgets";
      }
    }
  }

  log(e: any) {
    console.log(e.type, e);
  }

  eLog(m: any, e: any) {
    console.log(m, e.type);
  }

  canMove(e: any): boolean {
    return e.indexOf('Disabled') === -1;
  }

  //----------------------- Fin Drag_And_Drop
}
