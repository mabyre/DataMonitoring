import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ProfileRoutingModule } from './profile-routing.module';
import { ProfileComponent } from './profile.component';
import { DataMonitoringLayoutModule } from "../../shared/layout/layout.module";
//import { StatsModule } from "../../shared/stats/stats.module";

@NgModule({
    imports: [
        CommonModule,
        DataMonitoringLayoutModule,
        //    StatsModule,
        ProfileRoutingModule
    ],
    declarations: [ProfileComponent]
})
export class ProfileModule {
}
