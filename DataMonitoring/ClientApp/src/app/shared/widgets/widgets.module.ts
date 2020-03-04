import { NgModule } from "@angular/core";
import { CommonModule } from "@angular/common";

import { WidgetComponent } from "./widget/widget.component";
import { WidgetsGridComponent } from "./widgets-grid/widgets-grid.component";

@NgModule({
  imports: [CommonModule],
  declarations: [WidgetComponent, WidgetsGridComponent],
  exports: [WidgetComponent, WidgetsGridComponent]
})
export class WidgetsModule { }
