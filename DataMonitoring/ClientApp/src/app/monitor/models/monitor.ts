export class Monitor {
  key = 0;
  title = '';
  displayTitle = 'true';
  classColorTitle = 'Green';
  codeLangue = '';
  skin = '';
  timeZone = '';
  isTestMode = false;
  message = '';
  version = '';

  widgets: Widget[];
}

export class Widget {
  id = 0;
  column = 0;
  position = 0;
  widgetIdList:  string;
}



