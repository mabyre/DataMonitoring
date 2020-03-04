using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using DataMonitoring.Model;
using Newtonsoft.Json.Linq;

namespace DataMonitoring.Business
{
    public static class MonitorComponent
    {
        public static bool IsSpecificStyleEqualValue(string actualValue, string expectedValue)
        {
            if (string.IsNullOrEmpty(expectedValue))
            {
                return false;
            }

            if (decimal.TryParse(actualValue, out var actual) && decimal.TryParse(expectedValue, out var expected))
            {
                if (actual == expected)
                {
                    return true;
                }
            }
            else
            {
                if (actualValue == expectedValue)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsSpecificStyleHigherValue(string actualValue, string expectedValue)
        {
            if (string.IsNullOrEmpty(expectedValue))
            {
                return false;
            }

            if (decimal.TryParse(actualValue, out var actual) && decimal.TryParse(expectedValue, out var expected))
            {
                if (actual > expected)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsSpecificStyleLowerValue(string actualValue, string expectedValue)
        {
            if (string.IsNullOrEmpty(expectedValue))
            {
                return false;
            }

            if (decimal.TryParse(actualValue, out var actual) && decimal.TryParse(expectedValue, out var expected))
            {
                if (actual < expected)
                {
                    return true;
                }
            }

            return false;
        }

        public static List<string> GetColumnsToSum(List<IIndicatorColumn> indicatorColumns)
        {
            var list = new List<string>();

            foreach (var column in indicatorColumns)
            {
                if (column.IsNumericFormat)
                {
                    list.Add(column.Code);
                }
            }

            return list;
        }

        public static string GetColumnData(string code, JToken data)
        {
            var columnCode = code.Replace("[", string.Empty).Replace("]", string.Empty);

            var token = data.Children().FirstOrDefault(x => x.Path == $"{data.Path}.{columnCode}");
            return token != null ? token.First.Value<string>() : string.Empty;
        }

        #region Table Widget Type
        public static List<string> GetColumnsNotDisplayed(IndicatorTableWidget indicatorWidget)
        {
            var list = new List<string>();

            foreach (var column in indicatorWidget.TableWidgetColumns)
            {
                if (column is IndicatorTableWidgetColumn indicatorColumn)
                {
                    var existInCalculatedColumn = indicatorWidget.TableWidgetColumns
                        .OfType<CalculatedTableWidgetColumn>()
                        .Any(x => x.TotalValueColumn.Contains(column.Code) ||
                                  x.PartialValueColumn.Contains(column.Code));

                    var existInMaskColumn = indicatorWidget.TableWidgetColumns
                        .OfType<MaskTableWidgetColumn>()
                        .Any(x => x.DisplayModel.Contains(column.Code));

                    if (indicatorColumn.Displayed == false &&
                        indicatorColumn.TranspositionColumn == false &&
                        indicatorColumn.TranspositionRow == false &&
                        indicatorColumn.TranspositionValue == false &&
                        existInMaskColumn == false &&
                        existInCalculatedColumn == false)
                    {
                        list.Add(column.Code);
                    }
                }
            }

            return list;
        }

        public static List<string> GetColumnsToAggregate(IndicatorTableWidget indicatorWidget)
        {
            var list = new List<string>();

            foreach (var column in indicatorWidget.TableWidgetColumns)
            {
                if (column is IndicatorTableWidgetColumn indicatorColumn)
                {
                    if ((indicatorColumn.Displayed ||
                         indicatorColumn.TranspositionRow ||
                         indicatorColumn.TranspositionColumn ||
                         indicatorColumn.TranspositionValue) && indicatorColumn.IsNumericFormat == false)
                    {
                        list.Add(column.Code);
                    }
                }
            }

            return list;
        }

        public static object GetTranspositionColumn(List<IndicatorTableWidgetColumn> indicatorColumn)
        {
            var rowColumnName = string.Empty;
            var colColumnName = string.Empty;
            var dataColumnName = string.Empty;

            foreach (var column in indicatorColumn)
            {
                if (column.TranspositionColumn)
                {
                    colColumnName = column.Code;
                }
                if (column.TranspositionRow)
                {
                    rowColumnName = column.Code;
                }

                if (column.TranspositionValue)
                {
                    dataColumnName = column.Code;
                }
            }

            return new { RowColumnName = rowColumnName, ColColumnName = colColumnName, DataColumnName = dataColumnName };
        }

        public static string GetTargetValue(IndicatorTableWidget indicatorWidget)
        {
            return indicatorWidget.TargetValue.HasValue ? indicatorWidget.TargetValue?.ToString() : string.Empty;
        }

        public static string GetTextMaskColumn(MaskTableWidgetColumn column, JToken data)
        {
            var reg = new Regex(@"\[(.*?)\]");

            var matches = reg.Matches(column.DisplayModel);

            var text = column.DisplayModel;

            foreach (Match match in matches)
            {
                text = text.Replace($"{match.Value}", GetColumnData(match.Value, data));
            }

            return text;
        }

        public static string GetTextCalculatedColumn(CalculatedTableWidgetColumn column, JToken data)
        {
            var reg = new Regex(@"\[(.*?)\]");

            var totalMatches = reg.Matches(column.TotalValueColumn);
            var total = 0m;

            foreach (Match match in totalMatches)
            {
                var value = GetColumnData(match.Value, data);
                if (decimal.TryParse(value, out var val))
                {
                    total += val;
                }
            }

            if (string.IsNullOrEmpty(column.PartialValueColumn))
            {
                return total.ToString(CultureInfo.CurrentCulture);
            }
            
            decimal.TryParse(GetColumnData(column.PartialValueColumn, data), out var partial);
     
            return total == 0 ? "0" : ((partial * 100) / total).ToString(CultureInfo.CurrentCulture);
        }

        public static string GetSpecificColumnStyle(IndicatorTableWidget indicatorWidget, TableWidgetColumn column, JToken data, string text)
        {
            var specificStyle = GetSpecificStyleWhenEqual(indicatorWidget, column, data, text);

            if (!string.IsNullOrEmpty(specificStyle))
            {
                return specificStyle;
            }

            specificStyle = GetSpecificStyleWhenHigher(indicatorWidget, column, data, text);

            if (!string.IsNullOrEmpty(specificStyle))
            {
                return specificStyle;
            }

            specificStyle = GetSpecificStyleWhenLower(indicatorWidget, column, data, text);

            if (!string.IsNullOrEmpty(specificStyle))
            {
                return specificStyle;
            }

            return string.Empty;
        }

        public static string GetSpecificStyleWhenEqual(IndicatorTableWidget indicatorWidget, TableWidgetColumn column, JToken data, string text)
        {
            var expectedValue = string.IsNullOrEmpty(column.EqualsColumnCode1)
                ? column.EqualsValue1
                : GetSpecificValue(indicatorWidget, column.EqualsColumnCode1, data);

            if (!string.IsNullOrEmpty(expectedValue) && IsSpecificStyleEqualValue(text, expectedValue))
            {
                return column.CellStyleWhenEqualValue1;
            }

            expectedValue = string.IsNullOrEmpty(column.EqualsColumnCode2)
                ? column.EqualsValue2
                : GetSpecificValue(indicatorWidget, column.EqualsColumnCode2, data);

            if (!string.IsNullOrEmpty(expectedValue) && IsSpecificStyleEqualValue(text, expectedValue))
            {
                return column.CellStyleWhenEqualValue2;
            }

            expectedValue = string.IsNullOrEmpty(column.EqualsColumnCode3)
                ? column.EqualsValue3
                : GetSpecificValue(indicatorWidget, column.EqualsColumnCode3, data);

            if (!string.IsNullOrEmpty(expectedValue) && IsSpecificStyleEqualValue(text, expectedValue))
            {
                return column.CellStyleWhenEqualValue3;
            }

            return string.Empty;
        }

        public static string GetSpecificStyleWhenLower(IndicatorTableWidget indicatorWidget, TableWidgetColumn column, JToken data, string text)
        {
            var expectedValue = string.IsNullOrEmpty(column.LowerColumnCode)
                ? column.LowerValue
                : GetSpecificValue(indicatorWidget, column.LowerColumnCode, data);

            if (!string.IsNullOrEmpty(expectedValue) && IsSpecificStyleLowerValue(text, expectedValue))
            {
                return column.CellStyleWhenLowerValue;
            }

            return string.Empty;
        }

        public static string GetSpecificStyleWhenHigher(IndicatorTableWidget indicatorWidget, TableWidgetColumn column, JToken data, string text)
        {
            var expectedValue = string.IsNullOrEmpty(column.HigherColumnCode)
                ? column.HigherValue
                : GetSpecificValue(indicatorWidget, column.HigherColumnCode, data);

            if (!string.IsNullOrEmpty(expectedValue) && IsSpecificStyleHigherValue(text, expectedValue))
            {
                return column.CellStyleWhenHigherValue;
            }

            return string.Empty;
        }

        public static string GetSpecificValue(IndicatorTableWidget indicatorWidget, string columnCode, JToken data)
        {
            var column = indicatorWidget.TableWidgetColumns.FirstOrDefault(x => x.Code == columnCode);

            if (column == null)
            {
                return string.Empty;
            }

            if (column is IndicatorTableWidgetColumn)
            {
                return GetColumnData(column.Code, data);
            }

            if (column is TargetTableWidgetColumn)
            {
                return GetTargetValue(indicatorWidget);
            }

            throw new Exception("Get Specific value not implemented");
        }

        #endregion

        #region Bar Widget Type
        public static List<string> GetColumnsNotDisplayed(IndicatorBarWidget indicatorWidget)
        {
            var list = new List<string>();

            foreach (var column in indicatorWidget.IndicatorBarWidgetColumns)
            {
                if (column.Code != indicatorWidget.DataColumnCode && column.Code != indicatorWidget.LabelColumnCode)
                {
                    list.Add(column.Code);
                }
            }

            return list;
        }

        // BRY_WORK_20191212 : This function is bullshit, usefull only to make liste of columns
        //
        public static List<string> GetColumnsToAggregate(IndicatorBarWidget indicatorWidget)
        {
            var list = new List<string>();

            foreach (var column in indicatorWidget.IndicatorBarWidgetColumns)
            {
                if (column.Code == indicatorWidget.LabelColumnCode)
                {
                    list.Add(column.Code);
                }
            }

            return list;
        }

        public static List<int> GetListDataSetIndex(IndicatorBarWidget indicatorWidget, decimal? targetData)
        {
            var indexList = new List<int>();

            if (indicatorWidget.AddTargetBar && targetData.HasValue)
            {
                indexList.Add(1);
                if (indicatorWidget.AddBarStackedToTarget)
                {
                    indexList.Add(2);
                }
            }
            else
            {
                indexList.Add(0);
                if (indicatorWidget.AddBarStackedToTarget && targetData.HasValue)
                {
                    indexList.Add(1);
                }
            }

            return indexList;
        }

        public static decimal? GetTargetData(IEnumerable<JToken> data, IndicatorBarWidget indicatorBarWidget)
        {
            if (indicatorBarWidget.SetSumDataToTarget)
            {
                if (data.Any())
                {
                    var dataColumnCode = indicatorBarWidget.DataColumnCode.Replace("[", string.Empty).Replace("]", string.Empty);
                    return data.Sum(m => (decimal)m.SelectToken(dataColumnCode));
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return indicatorBarWidget.TargetValue;
            }
        }
        
        #endregion

        #region Chart Widget Type 

        public static List<string> GetColumnsNotDisplayed(IndicatorChartWidget indicatorWidget)
        {
            var list = new List<string>();

            list.Add("Group1");
            list.Add("Group2");
            list.Add("Group3");
            list.Add("Group4");
            list.Add("Group5");

            return list;
        }

        public static List<string> GetColumnsToAggregate(IndicatorChartWidget indicatorWidget)
        {
            var list = new List<string>
            {
                "DateUtc"
            };

            return list;
        }

        public static object GetChartValue(FlowIndicatorValue chartIndicator)
        {
            return new
            {
                chartIndicator.DateUtc,
                chartIndicator.Group1,
                chartIndicator.Group2,
                chartIndicator.Group3,
                chartIndicator.Group4,
                chartIndicator.Group5,
                chartIndicator.Value
            };
        }

        #endregion

        #region Gauge Widget Type 
        public static List<string> GetColumnsNotDisplayed(IndicatorGaugeWidget indicatorWidget)
        {
            var list = new List<string>();

            list.Add("Group1");
            list.Add("Group2");
            list.Add("Group3");
            list.Add("Group4");
            list.Add("Group5");

            return list;
        }

        public static object GetLastChartValue(FlowIndicatorValue chartIndicator)
        {
            return new
            {
                chartIndicator.Group1,
                chartIndicator.Group2,
                chartIndicator.Group3,
                chartIndicator.Group4,
                chartIndicator.Group5,
                chartIndicator.Value
            };
        }

        #endregion
    }
}
