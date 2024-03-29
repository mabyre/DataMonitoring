import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AuthorizationGuard } from "@app/core/auth";
import { ColorListComponent } from "./components/color-list/color-list.component";
import { ColorEditComponent } from "./components/color-edit/color-edit.component";

const routes: Routes = [
    { path: 'colors', component: ColorListComponent, data: { pageTitle: 'List', roles: ['SuperAdmin', 'Admin', 'Customer'] }, canActivate: [AuthorizationGuard] },
    { path: 'color-edit', component: ColorEditComponent, data: { pageTitle: 'Create' } },
    { path: 'color-edit/:id', component: ColorEditComponent, data: { pageTitle: 'Edit' } },
];

@NgModule( {
    imports:
        [
            RouterModule.forChild( routes )
        ],
    exports: [RouterModule]
} )
export class ColorRoutingModule { }
