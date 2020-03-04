import { IndicatorTableWidget } from "./indicator-table-widget";
import { IndicatorBarWidget } from "./indicator-bar-widget";
import { IndicatorChartWidget } from "./indicator-chart-widget";
import { IndicatorGaugeWidget } from "./indicator-gauge-widget";

export class Widget {
    id = 0;
    title = '';
    titleFontSize = 15;
    titleTranslate: TitleWidgetTranlation[];
    titleColorName = 'Black';
    refreshTime = 0;
    type = 0;
    titleDisplayed = true;
    currentTimeManagementDisplayed = true;
    lastRefreshTimeIndicatorDisplayed = true;
    timeManagementId = 0;

    indicatorTitleListToDisplayed = '';
    isTestModePreview = true;
    typeDisplay = "";

    // Types :
    indicatorTableWidgetList: IndicatorTableWidget[];
    indicatorBarWidget: IndicatorBarWidget;
    indicatorChartWidget: IndicatorChartWidget;
    indicatorGaugeWidget: IndicatorGaugeWidget;
}

export class TitleWidgetTranlation {
    id = 0;
    localizationCode = '';
    title = '';
}

export abstract class IndicatorWidget {
    id = 0;
    indicatorId = 0;
    targetValue: number;

    // Title Indicator
    titleIndicatorDisplayed = false;
    titleIndicatorColor: 'Black';
}




