import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizationGuard } from "@app/core/auth";
import { StyleListComponent } from "./components/style-list/style-list.component";
import { StyleEditComponent } from "./components/style-edit/style-edit.component";

const routes: Routes = [
    { path: 'styles', component: StyleListComponent, data: { pageTitle: 'List', roles: ['SuperAdmin', 'Admin', 'Customer'] }, canActivate: [AuthorizationGuard] },
    { path: 'style-edit', component: StyleEditComponent, data: { pageTitle: 'Create' } },
    { path: 'style-edit/:id', component: StyleEditComponent, data: { pageTitle: 'Edit' } },
];

@NgModule( {
    imports:
        [
            RouterModule.forChild( routes )
        ],
    exports: [RouterModule]
} )
export class StyleRoutingModule { }
