import { Component, OnInit } from '@angular/core';
import { LayoutService } from '@app/shared/layout/layout.service';

@Component({
  selector: 'app-full-screen',
  templateUrl: './full-screen.component.html'
})
export class FullScreenComponent implements OnInit {

  constructor(private layoutService: LayoutService) {
  }

  ngOnInit() {
  }

  onToggle() {
    this.layoutService.onFullScreenToggle();
  }
}
