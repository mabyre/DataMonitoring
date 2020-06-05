import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizationGuard } from "@app/core/auth";

import { ConnectorsListComponent } from './components/connectors-list/connectors-list.component';
import { ConnectorEditComponent } from './components/connector-edit/connector-edit.component';

const connectorRoutes: Routes = [
  { path: 'connectors', component: ConnectorsListComponent, data: { pageTitle: 'List' } },
  { 
    path: 'connector-edit/:id', component: ConnectorEditComponent, 
    data: { pageTitle: 'Edit' }, 
    canActivate: [AuthorizationGuard] 
  },
  {
    path: 'connector-edit',
    component: ConnectorEditComponent,
    data: { pageTitle: 'Create' },
    canActivate: [AuthorizationGuard]
  },
];

@NgModule({
  imports:
    [
      RouterModule.forChild(connectorRoutes)
    ],
  exports: [RouterModule]
})
export class ConnectorRoutingModule { }
