import { Routes, RouterModule } from "@angular/router";
import { UnauthorizedComponent } from './unauthorized/unauthorized.component';
import { ForbiddenComponent } from './forbidden/forbidden.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'blank',
    pathMatch: 'full'
  },
  {
    path: 'error404',
    loadChildren: './error404/error404.module#Error404Module'
  },
  {
    path: 'error500',
    loadChildren: './error500/error500.module#Error500Module'
  },
  {
    path: 'unauthorized',
    component:UnauthorizedComponent,
    data: { pageTitle: 'Unauthorized' }
  },
  {
    path: 'forbidden',
    component: ForbiddenComponent,
    data: { pageTitle: 'Forbidden' }
  }
];

export const routing = RouterModule.forChild(routes);




