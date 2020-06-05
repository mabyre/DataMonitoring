import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthorizationGuard } from "@app/core/auth";

import { TimeManagementListComponent } from "./components/time-management-list/time-management-list.component";
import { TimeManagementEditComponent } from "./components/time-management-edit/time-management-edit.component";

const routes: Routes = [
  { path: 'times', component: TimeManagementListComponent, data: { pageTitle: 'List' } },
  { path: 'time-edit', component: TimeManagementEditComponent, data: { pageTitle: 'Create' }, canActivate: [AuthorizationGuard] },
  { path: 'time-edit/:id', component: TimeManagementEditComponent, data: { pageTitle: 'Edit' }, canActivate: [AuthorizationGuard] },
];

@NgModule({
  imports:
    [
      RouterModule.forChild(routes)
    ],
  exports: [RouterModule]
})
export class TimeManagementRoutingModule { }
