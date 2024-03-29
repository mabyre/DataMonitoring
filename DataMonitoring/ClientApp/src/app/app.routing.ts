import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MainLayoutComponent } from "./shared/layout/app-layouts/main-layout.component";
import { EmptyLayoutComponent } from "./shared/layout/app-layouts/empty-layout.component";
import { AuthorizationGuard } from './core/auth';

const appRoutes: Routes = [
    {
        path: '',
        component: MainLayoutComponent,
        data: { pageTitle: "Home" },
        children: [
            {
                path: '',
                redirectTo: "Home",
                pathMatch: "full"
            },
            {
                path: 'Home',
                component: HomeComponent,
                pathMatch: "full"
            },
            {
                path: 'indicator',
                loadChildren: './setting/indicator/indicator.module#IndicatorModule',
                data: { pageTitle: 'Indicator' }
            },
            {
                path: "connector",
                canLoad: [AuthorizationGuard],
                loadChildren: './setting/connector/connector.module#ConnectorModule',
                data: { pageTitle: 'Connector' }
            },
            {
                path: "time",
                canLoad: [AuthorizationGuard],
                loadChildren: './setting/time-management/time-management.module#TimeManagementModule',
                data: { pageTitle: 'TimeManagement' }
            },
            {
                path: "style",
                canLoad: [AuthorizationGuard],                
                loadChildren: './setting/style/style.module#StyleModule',
                data: { pageTitle: 'Style' }
            },
            {
                path: "color",
                canLoad: [AuthorizationGuard],
                loadChildren: './setting/color/color.module#ColorModule',
                data: { pageTitle: 'Color' }
            },
            {
                path: "widget",
                canLoad: [AuthorizationGuard],
                loadChildren: './widget/widget.module#WidgetModule',
                data: { pageTitle: 'Widget' }
            },
            {
                path: "dashboard",
                loadChildren: './dashboard/dashboard.module#DashboardModule',
                data: { pageTitle: 'Dashboard' }
            },
            {
                path: "dashboard-light",
                loadChildren: './dashboard-light/dashboard-light.module#DashboardLightModule',
                data: { pageTitle: 'Dashboard Light' }
            },
            {
                path: 'app-views',
                loadChildren: './app-views/app-views.module#AppViewsModule',
                data: { pageTitle: 'App Views' }
            },
            {
                path: "errors",
                loadChildren: "./shared/errors/errors.module#ErrorsModule",
                data: { pageTitle: "Error" }
            },
        ]
    },
    {
        path: "monitor",
        component: EmptyLayoutComponent,
        loadChildren: './monitor/monitor.module#MonitorModule',
        data: { pageTitle: 'Monitor' }
    },
    { path: '**', redirectTo: 'errors/error404' }
];

@NgModule({
    imports: [RouterModule.forRoot(appRoutes)],
    exports: [RouterModule]
})
export class AppRouting { }
