import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthorizationGuard } from '../core/auth/authorization.guard';

export const appViewsRoutes: Routes = [
    // { 
    //     path: 'forum', 
    //     loadChildren: './+forum/forum.module#ForumModule' 
    // },
    { 
        path: 'profile', 
        canLoad: [AuthorizationGuard],
        data: { pageTitle: 'Profile' },
        loadChildren: './+profile/profile.module#ProfileModule' 
    },
    // { 
    //     path: 'blog', 
    //     loadChildren: './+blog/blog.module#BlogModule' 
    // },
    // { 
    //     path: 'gallery', 
    //     loadChildren: './+gallery/gallery-demo.module#GalleryDemoModule' 
    // },
    // { 
    //     path: 'timeline', 
    //     loadChildren: './+timeline/timeline.module#TimelineModule' 
    // },
    // { 
    //     path: 'projects', 
    //     loadChildren: './+projects/projects-list.module#ProjectsListModule' 
    // },
];

@NgModule({
    imports:
        [
            RouterModule.forChild(appViewsRoutes)
        ],
    exports: [RouterModule]
})
export class AppViewsRouting {}
