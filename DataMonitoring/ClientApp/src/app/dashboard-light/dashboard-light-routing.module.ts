import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { DashboardLightEditComponent } from "./components/dashboard-light-edit/dashboard-light-edit.component";

const routes: Routes = [
  { path: 'dashboard-light-edit/:id', component: DashboardLightEditComponent, data: { pageTitle: 'Edit' } },
];

@NgModule({
  imports:
    [
      RouterModule.forChild(routes)
    ],
  exports: [RouterModule]
})
export class DashboardLightRoutingModule { }

