import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { WidgetListComponent, WidgetEditComponent, WidgetEditTlComponent } from './components';

const routes: Routes = [
  { path: 'widgets', component: WidgetListComponent, data: { pageTitle: 'List' } },
  { path: 'widgetCreate', component: WidgetEditComponent, data: { pageTitle: 'Create' } },
  { path: 'widgetEdit/:id', component: WidgetEditComponent, data: { pageTitle: 'Edit' } },
  { path: 'widgetEditTl/:id', component: WidgetEditTlComponent, data: { pageTitle: 'Edit TL' } }
];

@NgModule({
  imports:
  [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class WidgetRoutingModule { }

