import {IndicatorWidget} from "./widget";

export class IndicatorBarWidget extends IndicatorWidget {
    // Axe X
    public displayAxeX = true;

    // Label
    public labelColumnCode = "";  
    public labelFontSize = 20;
    public labelColorText = "#000000";  

    // Axe Y
    public displayAxeY = false;
    public displayDataAxeY = false;
    public textDataAxeYColor = "#000000"; 
    public displayGridLinesAxeY = false;

    // DataLabel
    public dataColumnCode = ""; 
    public displayDataLabelInBar = true;
    public dataLabelInBarColor = "#000000"; 
    public fontSizeDataLabel = 25;
    public decimalMask = "";
    public barColor = "#000000"; 

    // Stacked bar
    public addBarStackedToTarget = true;
    public barColorStackedToTarget = "#000000";  

    // Target
    public addTargetBar = true;
    public targetBarColor = "#000000";
    public setSumDataToTarget = true;

    public indicatorBarWidgetColumnList: IndicatorBarWidgetColumn[]; 
    public barLabelWidgetList: BarLabelWidget[]; 
}

export class IndicatorBarWidgetColumn {
    public id = 0;
    public code = "";
    public filtered = false;
    public filteredValue = "";
    public isNumericFormat = false;
}

export class BarLabelWidget {
    public id = 0;
    public name = "NEW";
    public sequence = 0;
    public labelTextColor = "#000000";
    public useLabelColorForBar = true;

    public barLabelWidgetLocalizationList: BarLabelWidgetLocalization[];
}

export class BarLabelWidgetLocalization {
    public id = 0;
    public localizationCode = "";
    public title = "";
}
