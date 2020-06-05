import { NgModule } from "@angular/core";

import { AppViewsRouting } from "./app-views.routing";
import { SharedModule } from "../shared/shared.module";

@NgModule({
  declarations: [

  ],
  imports: [
    SharedModule,
    AppViewsRouting,

  ],
  entryComponents: []
})
export class AppViewsModule {
}
