import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizationCanGuard, AuthorizationGuard } from "@app/core/auth";

import { ConnectorsListComponent } from './components/connectors-list/connectors-list.component';
import { ConnectorEditComponent } from './components/connector-edit/connector-edit.component';

const routes: Routes = [
  { path: 'connectors', component: ConnectorsListComponent, data: { pageTitle: 'List' } },
  { path: 'connector-edit/:id', component: ConnectorEditComponent, data: { pageTitle: 'Edit' } },
  { path: 'connector-edit', component: ConnectorEditComponent, data: { pageTitle: 'Create' } },
  // BRY_20200116
  //{
  //  path: 'connector-edit',
  //  component: ConnectorEditComponent,
  //  data: {
  //    pageTitle: 'Create',
  //    role: "user",
  //    permission: "Connector.Edit"
  //  },
  //  canActivate: [AuthorizationGuard],
  //  canLoad: [AuthorizationCanGuard]
  //},
];

@NgModule({
  imports:
    [
      RouterModule.forChild(routes)
    ],
  exports: [RouterModule]
})
export class ConnectorRoutingModule { }
