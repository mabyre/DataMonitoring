import { IndicatorWidget } from "./widget";

export class IndicatorTableWidget extends IndicatorWidget {

    sequence = 0;
    headerDisplayed = false;

    // Row Style
    rowStyleDisplayed = false;
    rowStyleWhenEqualValue: '';
    equalsValue = '';
    columnCode = '';

    tableWidgetColumnList: TableWidgetColumn[];
}

export class TableWidgetColumn {

    id = 0;
    type = 1;
    sequence = 0;
    code = 'CalculatedData_' + Math.floor(Math.random() * 100) + 1;
    displayed = true;
    name = 'NEW_COLUMN';
    nameDisplayed = true;
    columnStyle = 0;
    textBodyColor = 'Black';
    textHeaderColor = 'Black';
    decimalMask = '';
    boldHeader = false;
    boldBody = false;
    alignStyle = 1;
    // Style LowerValue
    cellStyleWhenLowerValue = '';
    lowerValue = '';
    lowerColumnCode = '';
    // Style HigherValue
    cellStyleWhenHigherValue = '';
    higherValue = '';
    higherColumnCode = '';
    // Style EqualValue 1
    cellStyleWhenEqualValue1 = '';
    equalsValue1 = '';
    equalsColumnCode1 = '';
    // Style EqualValue 2
    cellStyleWhenEqualValue2 = '';
    equalsValue2 = '';
    equalsColumnCode2 = '';
    // Style EqualValue 3
    cellStyleWhenEqualValue3 = '';
    equalsValue3 = '';
    equalsColumnCode3 = '';

    // IndicatorTableWidgetColumn :
    filtered = false;
    filteredValue = '';
    isNumericFormat = false;
    transpositionColumn = false;
    transpositionValue = false;
    transpositionRow = false;

    // MaskTableWidgetColumn :
    displayModel = '';

    // CalculatedTableWidgetColumn :
    partialValueColumn = '';
    totalValueColumn = '';

    columnNameLocalizations: ColumnTranslation[];
}

export class ColumnTranslation {
    id = 0;
    localizationCode = '';
    title = '';
}

