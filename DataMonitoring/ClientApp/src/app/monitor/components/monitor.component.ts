import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { interval} from 'rxjs';

import { LayoutService } from '@app/shared/layout/layout.service';
import { I18nService } from '@app/shared/i18n/i18n.service';
import { Configuration } from '@app/core/configuration';

import { MonitorService } from '../monitor.service';
import { Monitor, Widget} from '../models/monitor';

@Component({
  selector: 'app-monitor',
  templateUrl: './monitor.component.html'
})
export class MonitorComponent implements OnInit, OnDestroy {

  private timer;
  private timerRefresh;
 
  errorMessage: string;
  key : string;

  public title : string;
  public titleColorclass : string;
  public codeLangue : string;
  public timeZone : string;
  public isTestMode : boolean;
  public version : string;
  public message : string;

  public widget1List : string = null;
  public widget2List : string = null;
  public widget3List : string = null;
  public widget4List : string = null;
  
  constructor(private i18nservice: I18nService, private route: ActivatedRoute, 
    private monitorService: MonitorService, private layoutService: LayoutService,
    private configuration: Configuration) { 
  }

  ngOnInit() {

    this.key = this.route.snapshot.paramMap.get('key');

    if (this.key != null) {

        this.monitorService.getMonitor(this.key)
            .subscribe(result => {
                this.initMonitor(result);
                this.version = result.version;
            },
                error => {
                    this.errorMessage = error.statusText;
                });

      const waitinterval = this.configuration.waitIntervalMonitor * 1000;

      interval(waitinterval).subscribe(x => { 
        this.onReloaData();
      });

      // 2heures
      interval(7200000).subscribe(x => { 
        this.onRefresh();
      });
    
    } else {
      this.errorMessage = "key parameter requis";
    }
  }

  ngOnDestroy(){
    this.timer.unsubscribe();
    this.timerRefresh.unsubscribe();
  }

  onRefresh()
  {
    window.location.reload();
  }

  onReloaData() {

    this.errorMessage = '';
    
      this.monitorService.getMonitor(this.key)
        .subscribe(result => {
            this.initMonitor(result);
         if (this.version != result.version)
         {
           this.onRefresh();
         }
        },
        error => {
            this.errorMessage = error.statusText;
        });
  }

  initMonitor(monitor : Monitor)
  {
      this.titleColorclass = monitor.classColorTitle;
      this.title = monitor.displayTitle ? monitor.title : '';
      this.codeLangue = monitor.codeLangue == '' ? this.i18nservice.currentLanguage : monitor.codeLangue;
      this.timeZone = monitor.timeZone;
      this.isTestMode = monitor.isTestMode;

      if (this.message != monitor.message)
    {
          this.message = monitor.message;
          this.layoutService.emitFooterMessageSubject(monitor.message);
    }

      if (monitor.skin != ''){
          if (monitor.skin != this.layoutService.store.smartSkin)
      {
        const skin = this.configuration.skins.find((_skin) => {
            return _skin.name == monitor.skin;
        });
        if (skin != null)
        {
          this.layoutService.onSkinChange(skin);
        }
      }
    }
   
    let widget1IdList = '';
    let widget2IdList = '';
    let widget3IdList = '';
    let widget4IdList = '';

      monitor.widgets.forEach(element => {
      if (element.column == 1 && element.position == 1)
      {
        widget1IdList += widget1IdList == '' ? element.id : ';' + element.id;
      }

      if (element.column == 2 && element.position == 1)
      {
        widget2IdList += widget2IdList == '' ? element.id : ';' + element.id;
      }

      if (element.column == 1 && element.position == 2)
      {
        widget3IdList += widget3IdList == '' ? element.id : ';' + element.id;
      }

      if (element.column == 2 && element.position == 2)
      {
        widget4IdList += widget4IdList == '' ? element.id : ';' + element.id;
      }

      this.widget1List = widget1IdList != '' ? widget1IdList : null;
      this.widget2List = widget2IdList != '' ? widget2IdList : null;
      this.widget3List = widget3IdList != '' ? widget3IdList : null;
      this.widget4List = widget4IdList != '' ? widget4IdList : null;
      
    });
  }

  onFullScreenToggle() {
    this.layoutService.onFullScreenToggle();
  }
}

