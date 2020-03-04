import {TitleLocalization} from "@app/shared/form/title-localization/title-localization";

export class Indicator
{
    public id = 0;
    public title = "";
    public refreshTime: number;
    public delayForDelete: number = 5; // propose par defaut
    public type: number;

    public indicatorCalculated: CalculatedIndicator;

    public queryConnectors: QueryConnector[];

    public titleLocalizations: TitleLocalization[];

    public timeManagementId: number;

    public typeDisplay: string;

    public ConnectorTitleListToDisplayed = "";
}

export class QueryConnector
{
    public connectorId: number;
    public query: string;
}

export class CalculatedIndicator
{
    public indicatorOneId = 0;
    public indicatorTwoId = 0;
}
