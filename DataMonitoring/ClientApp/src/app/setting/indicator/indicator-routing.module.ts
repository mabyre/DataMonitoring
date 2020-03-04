import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { IndicatorsListComponent } from './components/indicators-list/indicators-list.component';
import { IndicatorEditComponent } from './components/indicator-edit/indicator-edit.component';

const routes: Routes = [
  { path: 'indicators', component: IndicatorsListComponent, data: { pageTitle: 'List' } },
  { path: 'indicator-edit', component: IndicatorEditComponent, data: { pageTitle: 'Create' } },
  { path: 'indicator-edit/:id', component: IndicatorEditComponent, data: { pageTitle: 'Edit' } },
];

@NgModule({
  imports:
    [
      RouterModule.forChild(routes)
    ],
  exports: [RouterModule]
})
export class IndicatorRoutingModule { }
