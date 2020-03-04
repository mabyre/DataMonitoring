export class DashboardLight {
  public id = 0;
  public title = "";
  public titleDisplayed = false;
  public titleColorName = "Black";
  public currentTimeManagementDisplayed = false;

  public dashboardLocalizations: DashboardLightLocalization[];

}

export class DashboardLightLocalization {
  public id = 0;
  public localizationCode = "";
  public title = "";
}
