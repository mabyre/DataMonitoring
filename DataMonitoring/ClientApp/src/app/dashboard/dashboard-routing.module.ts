import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import {DashboardListComponent} from "./components/dashboard-list/dashboard-list.component";
import {DashboardEditComponent} from "./components/dashboard-edit/dashboard-edit.component";
import {DashboardPublishComponent} from "./components/dashboard-publish/dashboard-publish.component";


const routes: Routes = [
  { path: 'dashboards', component: DashboardListComponent, data: { pageTitle: 'List' }},
  { path: 'dashboard-edit', component: DashboardEditComponent, data: { pageTitle: 'Create' }},
  { path: 'dashboard-edit/:id', component: DashboardEditComponent, data: { pageTitle: 'Edit' }},
  { path: 'dashboard-publish/:id', component: DashboardPublishComponent, data: { pageTitle: 'Publish' }}
];

@NgModule({
  imports:
  [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }

