export class Dashboard {
    public id = 0;
    public title = "";
    public titleDisplayed = false;
    public titleColorName = "Black";
    public currentTimeManagementDisplayed = false;

    widgetTitleListToDisplayed = '';

    public widgets: DashboardWidget[];
    public dashboardLocalizations: DashboardLocalization[];
    public sharedDashboards: SharedDashboard[];
}

export class DashboardWidget {
    public widgetId = 0;
    public column = 1;
    public position = 1;
}

export class DashboardLocalization {
    public id = 0;
    public localizationCode = "";
    public title = "";
}

export class SharedDashboard {
    id = 0;
    key = "";
    codeLangue = "";
    skin = "";
    timeZone="";
    isTestMode = false;
    message="";
}
