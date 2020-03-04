using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataMonitoring.Model
{
    public class TableWidgetColumn
    {
        public TableWidgetColumn() {}

        public TableWidgetColumn(TableWidgetColumn tableWidgetColumn)
        {
            Id = tableWidgetColumn.Id;
            Name = tableWidgetColumn.Name;
            NameDisplayed = tableWidgetColumn.NameDisplayed;
            Code = tableWidgetColumn.Code;
            ColumnStyle = tableWidgetColumn.ColumnStyle;
            Displayed = tableWidgetColumn.Displayed;
            TextBodyColor = tableWidgetColumn.TextBodyColor;
            TextHeaderColor = tableWidgetColumn.TextHeaderColor;
            Sequence = tableWidgetColumn.Sequence;
            DecimalMask = tableWidgetColumn.DecimalMask;
            BoldHeader = tableWidgetColumn.BoldHeader;
            BoldBody = tableWidgetColumn.BoldBody;
            AlignStyle = tableWidgetColumn.AlignStyle;

            CellStyleWhenLowerValue = tableWidgetColumn.CellStyleWhenLowerValue;
            LowerValue = tableWidgetColumn.LowerValue;
            LowerColumnCode = tableWidgetColumn.LowerColumnCode;

            CellStyleWhenHigherValue = tableWidgetColumn.CellStyleWhenHigherValue;
            HigherValue = tableWidgetColumn.HigherValue;
            HigherColumnCode = tableWidgetColumn.HigherColumnCode;

            CellStyleWhenEqualValue1 = tableWidgetColumn.CellStyleWhenEqualValue1;
            EqualsValue1 = tableWidgetColumn.EqualsValue1;
            EqualsColumnCode1 = tableWidgetColumn.EqualsColumnCode1;

            CellStyleWhenEqualValue2 = tableWidgetColumn.CellStyleWhenEqualValue2;
            EqualsValue2 = tableWidgetColumn.EqualsValue2;
            EqualsColumnCode2 = tableWidgetColumn.EqualsColumnCode2;

            CellStyleWhenEqualValue3 = tableWidgetColumn.CellStyleWhenEqualValue3;
            EqualsValue3 = tableWidgetColumn.EqualsValue3;
            EqualsColumnCode3 = tableWidgetColumn.EqualsColumnCode3;

            TableWidgetColumnLocalizations = tableWidgetColumn.TableWidgetColumnLocalizations;
        }

        public long Id { get; set; }
        
        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        public bool NameDisplayed { get; set; }

        [StringLength(60)]
        public string Code { get; set; }

        [Required]
        public int Sequence { get; set; }

        public bool Displayed { get; set; }

        [Required]
        [StringLength(50)]
        public string TextHeaderColor { get; set; }

        [Required]
        [StringLength(50)]
        public string TextBodyColor { get; set; }

        [Required]
        public ColumnStyle ColumnStyle { get; set; }

        public string DecimalMask { get; set; }

        public bool BoldHeader { get; set; }

        public bool BoldBody { get; set; }

        public AlignStyle AlignStyle { get; set; }

        [StringLength(100)]
        public string CellStyleWhenLowerValue { get; set; }

        [StringLength(60)]
        public string LowerValue { get; set; }

        [StringLength(60)]
        public string LowerColumnCode { get; set; }

        [StringLength(100)]
        public string CellStyleWhenHigherValue { get; set; }

        [StringLength(60)]
        public string HigherValue { get; set; }

        [StringLength(60)]
        public string HigherColumnCode { get; set; }

        [StringLength(100)]
        public string CellStyleWhenEqualValue1 { get; set; }

        [StringLength(60)]
        public string EqualsValue1 { get; set; }

        [StringLength(60)]
        public string EqualsColumnCode1 { get; set; }

        [StringLength(100)]
        public string CellStyleWhenEqualValue2 { get; set; }

        [StringLength(60)]
        public string EqualsValue2 { get; set; }

        [StringLength(60)]
        public string EqualsColumnCode2 { get; set; }

        [StringLength(100)]
        public string CellStyleWhenEqualValue3 { get; set; }

        [StringLength(60)]
        public string EqualsValue3 { get; set; }

        [StringLength(60)]
        public string EqualsColumnCode3 { get; set; }

        public long IndicatorTableWidgetId { get; set; }

        [Required]
        [ForeignKey("IndicatorTableWidgetId")]
        public IndicatorTableWidget IndicatorTableWidget { get; set; }

        public ICollection<TableWidgetColumnLocalization> TableWidgetColumnLocalizations { get; set; }
    }

    public class TableWidgetColumnLocalization
    {
        public long Id { get; set; }

        [Required]
        [StringLength(10)]
        public string LocalizationCode { get; set; }

        [Required]
        [StringLength(60)]
        public string Name { get; set; }

        public long TableWidgetColumnId { get; set; }

        [Required]
        [ForeignKey("TableWidgetColumnId")]
        public TableWidgetColumn TableWidgetColumn { get; set; }

    }
}
