using System.Collections.Generic;

namespace DataMonitoring.ViewModel
{
    public class IndicatorBarWidgetViewModel : IndicatorWidgetViewModel
    {
        // Axe X
        public bool DisplayAxeX { get; set; }

        // Label
        public string LabelColumnCode { get; set; } 
        public int LabelFontSize { get; set; }
        public string LabelColorText { get; set; } 

        // Axe Y
        public bool DisplayAxeY { get; set; }
        public bool DisplayDataAxeY { get; set; }
        public string TextDataAxeYColor { get; set; } 
        public bool DisplayGridLinesAxeY { get; set; }

        // DataLabel
        public string DataColumnCode { get; set; } 
        public bool DisplayDataLabelInBar { get; set; }
        public string DataLabelInBarColor { get; set; } 
        public int FontSizeDataLabel { get; set; }
        public string DecimalMask { get; set; }
        public string BarColor { get; set; } 

        // Stacked bar
        public bool AddBarStackedToTarget { get; set; }
        public string BarColorStackedToTarget { get; set; }

        // Target
        public bool AddTargetBar { get; set; }
        public string TargetBarColor { get; set; } 
        public bool SetSumDataToTarget { get; set; }

        public List<IndicatorBarWidgetColumnViewModel> IndicatorBarWidgetColumnList { get; set; }
        public List<BarLabelWidgetViewModel> BarLabelWidgetList { get; set; }
    }

    public class IndicatorBarWidgetColumnViewModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public bool Filtered { get; set; }
        public string FilteredValue { get; set; }
        public bool IsNumericFormat { get; set; }
    }

    public class BarLabelWidgetViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; } 
        public int Sequence { get; set; }
        public string LabelTextColor { get; set; } 
        public bool UseLabelColorForBar { get; set; }

        public List<BarLabelWidgetLocalizationViewModel> BarLabelWidgetLocalizationList { get; set; }
    }

    public class BarLabelWidgetLocalizationViewModel
    {
        public long Id { get; set; }
        public string LocalizationCode { get; set; }
        public string Title { get; set; }
    }
}
