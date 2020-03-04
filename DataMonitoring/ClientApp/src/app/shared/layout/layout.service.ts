import { Injectable } from '@angular/core';
import { Subject, fromEvent } from 'rxjs';
import { debounceTime, map } from 'rxjs/operators';
import { NotificationService } from '@app/shared/layout/notification.service';
import { Configuration, SkinSetting } from '@app/core/configuration'

const store = {

  smartSkin: '',
  skin: new SkinSetting(),
  skins: new Array<any>(),
  fixedHeader: localStorage.getItem('sm-fixed-header') == 'true',
  fixedNavigation: localStorage.getItem('sm-fixed-navigation') == 'true',
  fixedRibbon: localStorage.getItem('sm-fixed-ribbon') == 'true',
  fixedPageFooter: localStorage.getItem('sm-fixed-page-footer') == 'true',
  insideContainer: localStorage.getItem('sm-inside-container') == 'true',
  menuOnTop: localStorage.getItem('sm-menu-on-top') == 'true',

  shortcutOpen: false,
  isMobile: (/iphone|ipad|ipod|android|blackberry|mini|windows\sce|palm/i.test(navigator.userAgent.toLowerCase())),
  device: '',

  mobileViewActivated: false,
  menuCollapsed: false,
  menuMinified: false,
};

declare var $: any;

@Injectable({
  providedIn: 'root',
})
export class LayoutService {

  footerMessage = new Subject<string>();

  isActivated: boolean;
  skin: string;

  store: any;

  private subject: Subject<any>;

  trigger() {
    this.processBody(this.store);
    this.subject.next(this.store);
  }

  subscribe(next, err?, complete?) {
    return this.subject.subscribe(next, err, complete);
  }

  constructor(private notificationService: NotificationService, private configuration: Configuration) {

    this.subject = new Subject();

    store.fixedPageFooter = true;

    store.smartSkin = localStorage.getItem('sm-skin') || this.configuration.defaultSkin;

    store.skin = this.configuration.skins.find((_skin) => {
      return _skin.name == (localStorage.getItem('sm-skin') || this.configuration.defaultSkin);
    });

    if (store.skin == null)
    {
      store.skin =  this.configuration.skins.find((_skin) => {
        return _skin.name ==  this.configuration.defaultSkin;
      });
      store.smartSkin = this.configuration.defaultSkin;
    }

    store.skins =  this.configuration.skins;
    
    this.store = store;
    this.trigger();

    fromEvent(window, 'resize').pipe(
      debounceTime(100),
      map(() => {
        this.trigger();
      })
    )
      .subscribe();
  }

  onSkinChange(skin) {
    this.store.skin = skin;
    this.store.smartSkin = skin.name;
    this.dumpStorage();
    this.trigger();
  }

  onFixedHeader() {
    this.store.fixedHeader = !this.store.fixedHeader;
    if (this.store.fixedHeader == false) {
      this.store.fixedRibbon = false;
      this.store.fixedNavigation = false;
    }
    this.dumpStorage();
    this.trigger();
  }

  onFixedNavigation() {
    this.store.fixedNavigation = !this.store.fixedNavigation;

    if (this.store.fixedNavigation) {
      this.store.insideContainer = false;
      this.store.fixedHeader = true;
    } else {
      this.store.fixedRibbon = false;
    }
    this.dumpStorage();
    this.trigger();
  }

  onFixedRibbon() {
    this.store.fixedRibbon = !this.store.fixedRibbon;
    if (this.store.fixedRibbon) {
      this.store.fixedHeader = true;
      this.store.fixedNavigation = true;
      this.store.insideContainer = false;
    }
    this.dumpStorage();
    this.trigger();
  }

  onFixedPageFooter() {
    this.store.fixedPageFooter = !this.store.fixedPageFooter;
    this.dumpStorage();
    this.trigger();
  }

  onInsideContainer() {
    this.store.insideContainer = !this.store.insideContainer;
    if (this.store.insideContainer) {
      this.store.fixedRibbon = false;
      this.store.fixedNavigation = false;
    }
    this.dumpStorage();
    this.trigger();
  }

  onMenuOnTop() {
    this.store.menuOnTop = !this.store.menuOnTop;
    this.dumpStorage();
    this.trigger();
  }

  onCollapseMenu(value?) {
    if (typeof value !== 'undefined') {
      this.store.menuCollapsed = value;
    } else {
      this.store.menuCollapsed = !this.store.menuCollapsed;
    }

    this.trigger();
  }

  onMinifyMenu() {
    this.store.menuMinified = !this.store.menuMinified;
    this.trigger();
  }

  onShortcutToggle(condition?: any) {
    if (condition == null) {
      this.store.shortcutOpen = !this.store.shortcutOpen;
    } else {
      this.store.shortcutOpen = !!condition;
    }

    this.trigger();
  }

  onFullScreenToggle() {
    var $body = $('body');
    var documentMethods = {
      enter: ['requestFullscreen', 'mozRequestFullScreen', 'webkitRequestFullscreen', 'msRequestFullscreen'],
      exit: ['cancelFullScreen', 'mozCancelFullScreen', 'webkitCancelFullScreen', 'msCancelFullScreen']
    };

    if (!$body.hasClass("full-screen")) {
      $body.addClass("full-screen");
      document.documentElement[documentMethods.enter.filter((method) => {
        return document.documentElement[method];
      })[0]]();
    } else {
      $body.removeClass("full-screen");
      document[documentMethods.exit.filter((method) => {
        return document[method];
      })[0]]();
    }
  }

  dumpStorage() {
    localStorage.setItem('sm-skin', this.store.smartSkin);
    localStorage.setItem('sm-fixed-header', this.store.fixedHeader);
    localStorage.setItem('sm-fixed-navigation', this.store.fixedNavigation);
    localStorage.setItem('sm-fixed-ribbon', this.store.fixedRibbon);
    localStorage.setItem('sm-fixed-page-footer', this.store.fixedPageFooter);
    localStorage.setItem('sm-inside-container', this.store.insideContainer);
    localStorage.setItem('sm-menu-on-top', this.store.menuOnTop);
  }

  factoryReset() {
    this.notificationService.smartMessageBox({
      title: "<i class='fa fa-refresh' style='color:green'></i> Clear Local Storage",
      content: "Would you like to RESET all your saved widgets and clear LocalStorage?",
      buttons: '[No][Yes]'
    }, (ButtonPressed) => {
      if (ButtonPressed == "Yes" && localStorage) {
        localStorage.clear();
        location.reload();
      }
    });
  }

  processBody(state) {

    let $body = $('body');

    $body.removeClass('app-loading-background');
    
    $body.removeClass(state.skins.map((it)=>(it.name)).join(' '));
    $body.addClass(state.skin.name);

    $("#logo img").attr('src', state.skin.logo);

    $body.toggleClass('fixed-header', state.fixedHeader);
    $body.toggleClass('fixed-navigation', state.fixedNavigation);
    $body.toggleClass('fixed-ribbon', state.fixedRibbon);
    $body.toggleClass('fixed-page-footer', state.fixedPageFooter);
    $body.toggleClass('container', state.insideContainer);
    $body.toggleClass('menu-on-top', state.menuOnTop);
    $body.toggleClass('shortcut-on', state.shortcutOpen);

    state.mobileViewActivated = $(window).width() < 979;
    $body.toggleClass('mobile-view-activated', state.mobileViewActivated);
    if (state.mobileViewActivated) {
      $body.removeClass('minified');
    }

    if (state.isMobile) {
      $body.addClass("mobile-detected");
    } else {
      $body.addClass("desktop-detected");
    }

    if (state.menuOnTop) $body.removeClass('minified');

    if (!state.menuOnTop) {
      $body.toggleClass("hidden-menu-mobile-lock", state.menuCollapsed);
      $body.toggleClass("hidden-menu", state.menuCollapsed);
      $body.removeClass("minified");
    } else if (state.menuOnTop && state.mobileViewActivated) {
      $body.toggleClass("hidden-menu-mobile-lock", state.menuCollapsed);
      $body.toggleClass("hidden-menu", state.menuCollapsed);
      $body.removeClass("minified");
    }

    if (state.menuMinified && !state.menuOnTop && !state.mobileViewActivated) {
      $body.addClass("minified");
      $body.removeClass("hidden-menu");
      $body.removeClass("hidden-menu-mobile-lock");
    }
  }

  emitFooterMessageSubject(message : string) {
    this.footerMessage.next(message);
  }

}
