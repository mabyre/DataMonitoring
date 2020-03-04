using System.Collections.Generic;
using DataMonitoring.Model;

namespace DataMonitoring.ViewModel
{
    public class IndicatorTableWidgetViewModel : IndicatorWidgetViewModel
    {
        public int Sequence { get; set; }
        public bool HeaderDisplayed { get; set; }

        // Row Style
        public string RowStyleWhenEqualValue { get; set; }
        public string EqualsValue { get; set; }
        public string ColumnCode { get; set; }

        public List<TableWidgetColumnViewModel> TableWidgetColumnList { get; set; }
    }

    public class TableWidgetColumnViewModel
    {
        public long Id { get; set; }
        public int Sequence { get; set; }
        public string Code { get; set; }
        public bool Displayed { get; set; }
        public string Name { get; set; }
        public bool NameDisplayed { get; set; }     
        public ColumnStyle ColumnStyle { get; set; }
        public string TextBodyColor { get; set; }
        public string TextHeaderColor { get; set; }
        public ColumnType Type { get; set; }

        public string DecimalMask { get; set; }
        public bool BoldHeader { get; set; }
        public bool BoldBody { get; set; }
        public AlignStyle AlignStyle { get; set; }
        // Style LowerValue
        public string CellStyleWhenLowerValue { get; set; }
        public string LowerValue { get; set; }
        public string LowerColumnCode { get; set; }
        // Style HigherValue
        public string CellStyleWhenHigherValue { get; set; }
        public string HigherValue { get; set; }
        public string HigherColumnCode { get; set; }
        // Style EqualValue 1
        public string CellStyleWhenEqualValue1 { get; set; }
        public string EqualsValue1 { get; set; }
        public string EqualsColumnCode1 { get; set; }
        // Style EqualValue 2
        public string CellStyleWhenEqualValue2 { get; set; }
        public string EqualsValue2 { get; set; }
        public string EqualsColumnCode2 { get; set; }
        // Style EqualValue 3
        public string CellStyleWhenEqualValue3 { get; set; }
        public string EqualsValue3 { get; set; }
        public string EqualsColumnCode3 { get; set; }


        // IndicatorTableWidgetColumn :
        public bool Filtered { get; set; }
        public string FilteredValue { get; set; }
        public bool IsNumericFormat { get; set; }
        public bool TranspositionColumn { get; set; }
        public bool TranspositionValue { get; set; }
        public bool TranspositionRow { get; set; }

        // MaskTableWidgetColumn :
        public string DisplayModel { get; set; }

        // CalculatedTableWidgetColumn :
        public string PartialValueColumn { get; set; }
        public string TotalValueColumn { get; set; }

        public List<TableWidgetColumnLocalizationViewModel> ColumnNameLocalizations { get; set; }
    }

    public class TableWidgetColumnLocalizationViewModel
    {
        public long Id { get; set; }
        public string LocalizationCode { get; set; }
        public string Title { get; set; }
    }
}
