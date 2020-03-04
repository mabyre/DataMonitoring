import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';

import { MonitorComponent} from "./components/monitor.component";


const routes: Routes = [
    { path: 'monitor/:key', component: MonitorComponent, data: { pageTitle: 'Monitor' } }
];

@NgModule({
  imports:
  [
    RouterModule.forChild(routes)
  ],
  exports: [RouterModule]
})
export class MonitorRoutingModule { }

