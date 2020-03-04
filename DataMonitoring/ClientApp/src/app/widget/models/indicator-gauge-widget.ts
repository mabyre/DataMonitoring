import {IndicatorWidget} from "./widget";

export class IndicatorGaugeWidget extends IndicatorWidget {

    // Target
    targetDisplayed = false;
    gaugeTargetColor = "White";

    // Groups
    group1 = "";
    group2 = "";
    group3 = "";
    group4 = "";
    group5 = "";

    // range 1
    gaugeRange1Color = "Red";
    gaugeRange1MinValue = 0;
    gaugeRange1MaxValue = 200;

    // range 2
    gaugeRange2Displayed = true;
    gaugeRange2Color = "Orange";
    gaugeRange2MinValue = 200;
    gaugeRange2MaxValue = 400;

    // range 3
    gaugeRange3Displayed = true;
    gaugeRange3Color = "Green";
    gaugeRange3MinValue = 400;
    gaugeRange3MaxValue = 600;
}
