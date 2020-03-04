import {IndicatorWidget} from "./widget";

export class IndicatorChartWidget extends IndicatorWidget {
    axeFontSize = 20; 
    decimalMask = "";

    // Axe X
    axeXDisplayed = true;
    axeXColor = "#000000";

    // Axe Y
    axeYDisplayed = true;
    axeYDataDisplayed = true;
    axeYColor = "#000000";

    // Target
    chartTargetDisplayed = false;
    chartTargetColor = "#000000";

    // Average
    chartAverageDisplayed = false;
    chartAverageColor = "#000000";

    // Chart
    chartDataColor = "#000000";
    chartDataFill = true;

    // Groups
    group1 = "";
    group2 = "";
    group3 = "";
    group4 = "";
    group5 = "";

    axeYIsAutoAdjustableAccordingMinValue = false;
    axeYOffsetFromMinValue = 0;

    targetIndicatorChartWidgetList: TargetIndicatorChartWidget[];

    // utilisé pour le formulaire pour avoir la date en string !
    targetIndicatorChartWidgetFormList: TargetIndicatorChartWidgetForm[];
}

export class TargetIndicatorChartWidget {
    id = 0; 
    startDateUtc: Date;
    startTargetValue = "";

    endDateUtc: Date;
    endTargetValue = "";
}

export class TargetIndicatorChartWidgetForm {
    id = 0; 
    startDate: string;
    startTargetValue = "";

    endDate: string;
    endTargetValue = "";
}
