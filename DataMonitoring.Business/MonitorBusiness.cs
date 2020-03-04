using DataMonitoring.DAL;
using DataMonitoring.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Sodevlog.ExtensionMethods;
using Sodevlog.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Dashboard = DataMonitoring.Model.Dashboard;

namespace DataMonitoring.Business
{
    public class MonitorBusiness : UnitOfWork, IMonitorBusiness
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<MonitorBusiness>();

        public MonitorBusiness() : base()
        {
        }

        public MonitorBusiness(DataMonitoringDbContext dataMonitoringContext ) : base( dataMonitoringContext )
        {
        }

        public async Task<SharedDashboard> GetMonitorAsync(string key)
        {
            var monitor = await Repository<SharedDashboard>().SingleOrDefaultAsync(x => x.Key == key);
            if (monitor != null)
            {
                var password = new PasswordHasher();
                if (password.VerifyHashedPassword(monitor.SecurityStamp, monitor.Key) == PasswordVerificationResult.Success)
                {
                    return monitor;
                }

                throw new SecurityException("Key not conform");
            }

            return monitor;
        }

        public async Task<Dashboard> GetDashboardAsync(long id, string timeZone, string codeLangue = "")
        {
            var dashboard = await Repository<Dashboard>()
                .SingleOrDefaultAsync(x => x.Id == id, x => x.DashboardLocalizations, x => x.Widgets);

            var langue = string.IsNullOrEmpty(codeLangue) ? CultureInfo.CurrentCulture.Name : codeLangue;

            dashboard.TitleToDisplay =
                dashboard.DashboardLocalizations.Any(x => x.LocalizationCode == langue)
                    ? dashboard.DashboardLocalizations.Single(x => x.LocalizationCode == langue).Title
                    : dashboard.Title;

            var timeManagementAdded = false;

            foreach (var widget in dashboard.Widgets)
            {
                widget.Widget = await Repository<Widget>().SingleOrDefaultAsync(x => x.Id == widget.WidgetId);

                if (dashboard.CurrentTimeManagementDisplayed && timeManagementAdded == false)
                {
                    if (widget.Widget.TimeManagementId != null)
                    {
                        if (string.IsNullOrEmpty(timeZone))
                        {
                            dashboard.TitleToDisplay += $" {await GetTimeManagemenentDisplayAsync(widget.Widget.TimeManagementId.Value, null)}";
                        }
                        else
                        {
                            var timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
                            dashboard.TitleToDisplay += $" {await GetTimeManagemenentDisplayAsync(widget.Widget.TimeManagementId.Value, timeZoneInfo)}";
                        }

                        timeManagementAdded = true;
                    }
                }
            }

            return dashboard;
        }

        public async Task<string> CreateHtmlContentWidgetAsync( long id, TimeZoneInfo timeZoneInfo, string position = "" )
        {
            return await CreateHtmlWidgetByType( id, timeZoneInfo, false, position );
        }

        public async Task<string> CreateHtmlContentWidgetForTestAsync( long id, TimeZoneInfo timeZoneInfo, string position = "" )
        {
            return await CreateHtmlWidgetByType( id, timeZoneInfo, true, position );
        }

        public async Task<Widget> GetWidgetAsync(long id, TimeZoneInfo timeZoneInfo)
        {
            var widget = await WidgetBusiness.GetWidgetAsync(id);

            if (widget.TitleDisplayed)
            {
                widget.TitleToDisplay = await QueryingWidgetTitleAsync(id);
                widget.TitleFontSize = widget.TitleFontSize;

                if (widget.CurrentTimeManagementDisplayed && widget.TimeManagementId != null)
                {
                    widget.TitleToDisplay += $" {await GetTimeManagemenentDisplayAsync(widget.TimeManagementId.Value, timeZoneInfo)}";
                }
            }

            if (widget.LastRefreshTimeIndicator)
            {
                widget.LastUpdateUtc = await WidgetRepository.QueryingLastUpdateUtcIndicatorAsync(id);
            }

            return widget;
        }

        private async Task<string> GetTimeManagemenentDisplayAsync(long timeManagementId, TimeZoneInfo timeZoneInfo)
        {
            var timeRange = await TimeManagementBusiness.GetTimeRangeAsync(timeManagementId);

            if (timeRange != null)
            {
                var startDate = timeRange.StartTimeUtc.ToLocalTime();
                if (timeZoneInfo != null)
                {
                    startDate = TimeZoneInfo.ConvertTimeFromUtc(timeRange.StartTimeUtc, timeZoneInfo);
                }

                if (timeRange.EndTimeUtc == null)
                {
                    return $"FROM {startDate.ToString("HH:mm")}";
                }

                var endDate = timeRange.EndTimeUtc.Value.ToLocalTime();
                if (timeZoneInfo != null)
                {
                    endDate = TimeZoneInfo.ConvertTimeFromUtc(timeRange.EndTimeUtc.Value, timeZoneInfo);
                }

                return $"{startDate.ToString("HH:mm")} - {endDate.ToString("HH:mm")}";
            }

            return string.Empty;
        }

        private async Task<string> QueryingWidgetTitleAsync(long id)
        {
            var widget = await Repository<Widget>()
                .SingleOrDefaultAsync(x => x.Id == id, x => x.WidgetLocalizations);

            var title = widget.Title;

            if (widget.WidgetLocalizations.Any(x => x.LocalizationCode == CultureInfo.CurrentCulture.Name))
            {
                title = widget.WidgetLocalizations
                    .Single(x => x.LocalizationCode == CultureInfo.CurrentCulture.Name).Title;
            }

            return title;
        }

        private async Task<string> CreateHtmlWidgetByType(long id, TimeZoneInfo timeZoneInfo, bool forTest, string position)
        {
            var widget = await WidgetRepository.GetAsync( id );

            if ( HelperWidgetType.IsTable( widget.Type ) )
            {
                return await CreateHtmlTableWidgetAsync( widget, forTest );
            }

            if ( widget.Type == WidgetType.Bar )
            {
                return await CreateHtmlBarWidgetAsync( widget, forTest, position );
            }

            if ( widget.Type == WidgetType.Chart )
            {
                return await CreateHtmlChartWidgetAsync( widget, timeZoneInfo, forTest, position );
            }

            if ( widget.Type == WidgetType.Gauge )
            {
                return await CreateHtmlGaugeWidgetAsync( widget, timeZoneInfo, forTest, position );
            }

            Logger.LogError( $"Widget's type : {widget.Type.ToString()} unknown!" );

            return "<p>Error: Widget's type unknown!</p>";
        }

        #region Table Widget Type

        private async Task<string> CreateHtmlTableWidgetAsync(Widget widget, bool forTest = false)
        {
            var htmlContent = string.Empty;
            var cptIndicator = 1;

            Logger.LogInformation( $"Create HtmlTable for widget : {widget.Title}" );

            var indicatorWidgets = Repository<IndicatorTableWidget>().Find(x => x.WidgetId == widget.Id,
                x => x.IndicatorDefinition, x => x.TableWidgetColumns, x => x.Widget).OrderBy(x => x.Sequence).ToList();

            htmlContent += HtmlWidgetComponent.DivRowBegin();

            var nbIndicator = indicatorWidgets.Count;

            foreach (var indicatorWidget in indicatorWidgets)
            {
                if (widget.Type == WidgetType.MultiVerticalTable)
                {
                    var column = Convert.ToInt32(Math.Round(12d / nbIndicator, 0));
                    htmlContent += HtmlWidgetComponent.DivByCol(column);

                    if (indicatorWidget.TitleIndicatorDisplayed)
                    {
                        htmlContent += await TableIndicatorTitleAsync(indicatorWidget.IndicatorDefinitionId, indicatorWidget.TitleIndicatorColor, widget.Type);
                    }
                }

                htmlContent += await TableIndicatorWidgetAsync(widget, indicatorWidget, cptIndicator, nbIndicator, forTest);

                if (widget.Type == WidgetType.MultiVerticalTable)
                {
                    htmlContent += HtmlWidgetComponent.DivEnd();
                }

                cptIndicator++;
            }

            htmlContent += HtmlWidgetComponent.DivEnd();

            return htmlContent;
        }

        private async Task<string> TableIndicatorWidgetAsync
            (
                Widget widget,
                IndicatorTableWidget indicatorWidget,
                int cptIndicator,
                int nbIndicator,
                bool forTest = false
            )
        {
            var htmlContent = string.Empty;

            if ( cptIndicator == 1 )
            {
                htmlContent += HtmlWidgetComponent.TableBegin();
            }

            if ( indicatorWidget.TitleIndicatorDisplayed && widget.Type != WidgetType.MultiVerticalTable )
            {
                var nbColumn = indicatorWidget.TableWidgetColumns.Count( x => x.Displayed );

                htmlContent += HtmlWidgetComponent.TableLineBegin();
                htmlContent += HtmlWidgetComponent.TableColumnBegin( nbColumn );
                htmlContent += await TableIndicatorTitleAsync( indicatorWidget.IndicatorDefinitionId, indicatorWidget.TitleIndicatorColor, widget.Type );
                htmlContent += HtmlWidgetComponent.TableColumnEnd();
                htmlContent += HtmlWidgetComponent.TableLineEnd();
            }

            if ( indicatorWidget.HeaderDisplayed )
            {
                htmlContent += await GetTableIndicatorHeaderAsync( indicatorWidget );
            }

            htmlContent += await GetTableIndicatorBodyAsync( indicatorWidget, forTest );

            if ( cptIndicator == nbIndicator )
            {
                htmlContent += HtmlWidgetComponent.TableEnd();
            }

            return htmlContent;
        }

        private async Task<string> GetTableIndicatorHeaderAsync(IndicatorTableWidget indicatorWidget)
        {
            var htmlContent = HtmlWidgetComponent.TableLineBegin();

            foreach (var column in indicatorWidget.TableWidgetColumns.Where(x => x.Displayed).OrderBy(x => x.Sequence))
            {
                htmlContent += await GetColumnNameHeaderAsync(column);
            }

            htmlContent += HtmlWidgetComponent.TableLineEnd();

            return htmlContent;
        }

        private async Task<string> GetColumnNameHeaderAsync(TableWidgetColumn column)
        {
            var name = string.Empty;

            if (column.NameDisplayed)
            {
                var tableColumn = await Repository<TableWidgetColumn>()
                    .SingleOrDefaultAsync(x => x.Id == column.Id, x => x.TableWidgetColumnLocalizations);

                name = tableColumn.Name;

                if (tableColumn.TableWidgetColumnLocalizations.Any(x =>
                    x.LocalizationCode == CultureInfo.CurrentCulture.Name))
                {
                    name = tableColumn.TableWidgetColumnLocalizations
                        .Single(x => x.LocalizationCode == CultureInfo.CurrentCulture.Name).Name;
                }
            }

            var classColor = await Repository<ColorHtml>().SingleOrDefaultAsync(x => x.Name == column.TextHeaderColor);

            return HtmlWidgetComponent.TableColumn(name, string.Empty, ColumnStyle.Text, string.Empty, column.DecimalMask, classColor, column.BoldHeader, column.AlignStyle, true);
        }

        private async Task<string> GetTableIndicatorBodyAsync(IndicatorTableWidget indicatorWidget, bool forTest = false)
        {
            var htmlContent = string.Empty;

            var timeRange = indicatorWidget.Widget.TimeManagementId != null
                ? await TimeManagementBusiness.GetTimeRangeAsync(indicatorWidget.Widget.TimeManagementId.Value)
                : null;

            var result = GetTableWidgetData(indicatorWidget, timeRange, forTest);

            var data = result.ToList();
            if (data.Any())
            {
                foreach (var line in data)
                {
                    htmlContent += HtmlWidgetComponent.TableLineBegin();

                    foreach (var column in indicatorWidget.TableWidgetColumns.Where(x => x.Displayed)
                        .OrderBy(x => x.Sequence))
                    {
                        htmlContent += await GetColumnBodyAsync(indicatorWidget, column, line);
                    }

                    htmlContent += HtmlWidgetComponent.TableLineEnd();
                }
            }
            else
            {
                htmlContent += HtmlWidgetComponent.TableLineBegin();

                var nbColumn = indicatorWidget.TableWidgetColumns.Count(x => x.Displayed);
                var classColor = await Repository<ColorHtml>().SingleOrDefaultAsync(x => x.Name == "Black");

                var message = "No data found";
                htmlContent += HtmlWidgetComponent.GetMessageLine(message, nbColumn, classColor);

                htmlContent += HtmlWidgetComponent.TableLineEnd();
            }

            return htmlContent;
        }

        private async Task<string> GetColumnBodyAsync(IndicatorTableWidget indicatorWidget, TableWidgetColumn column, JToken data)
        {
            var htmlContent = string.Empty;

            var text = await GetColumnBodyDataAsync(indicatorWidget, column, data);
            var classColor = await Repository<ColorHtml>().SingleOrDefaultAsync(x => x.Name == column.TextBodyColor);

            var specificRowStyleCode = string.Empty;
            if (!string.IsNullOrEmpty(indicatorWidget.ColumnCode))
            {
                var actualValue = MonitorComponent.GetSpecificValue(indicatorWidget, indicatorWidget.ColumnCode, data);
                if (!string.IsNullOrEmpty(actualValue) && MonitorComponent.IsSpecificStyleEqualValue(actualValue, indicatorWidget.EqualsValue))
                {
                    var style = await Repository<Style>().SingleOrDefaultAsync(x => x.Name == indicatorWidget.RowStyleWhenEqualValue);
                    specificRowStyleCode = style.Code;
                }
            }

            var specificColumnStyleName = MonitorComponent.GetSpecificColumnStyle(indicatorWidget, column, data, text);

            var specificColumnStyleCode = string.Empty;

            if (!string.IsNullOrEmpty(specificColumnStyleName))
            {
                var style = await Repository<Style>().SingleOrDefaultAsync(x => x.Name == specificColumnStyleName);
                specificColumnStyleCode = style.Code;
            }

            htmlContent += HtmlWidgetComponent.TableColumn(text, specificRowStyleCode, column.ColumnStyle, specificColumnStyleCode, column.DecimalMask, classColor, column.BoldBody, column.AlignStyle);

            return htmlContent;
        }

        private async Task<string> GetColumnBodyDataAsync(IndicatorTableWidget indicatorWidget, TableWidgetColumn column, JToken data)
        {
            if (column is IndicatorTableWidgetColumn || column is TranspositionColumnTableWidgetColumn)
            {
                return MonitorComponent.GetColumnData(column.Code, data);
            }

            if (column is TitleIndicatorTableWidgetColumn)
            {
                return await IndicatorTitleAsync(indicatorWidget.IndicatorDefinitionId);
            }

            if (column is MaskTableWidgetColumn maskColumn)
            {
                return MonitorComponent.GetTextMaskColumn(maskColumn, data);
            }

            if (column is TargetTableWidgetColumn)
            {
                return MonitorComponent.GetTargetValue(indicatorWidget);
            }

            if (column is CalculatedTableWidgetColumn calculatedColumn)
            {
                return MonitorComponent.GetTextCalculatedColumn(calculatedColumn, data);
            }

            throw new Exception("Column type not implemented");
        }

        public IEnumerable<JToken> GetTableWidgetData( IndicatorTableWidget indicatorWidget, TimeRange timeRange, bool forTest = false )
        {
            var listIndicatorColumns = indicatorWidget.TableWidgetColumns.OfType<IIndicatorColumn>().ToList();
            var listColumnsNotDisplayed = MonitorComponent.GetColumnsNotDisplayed( indicatorWidget );
            var listColumnsAggregate = MonitorComponent.GetColumnsToAggregate( indicatorWidget );

            // BRY_WORK - Avant il y avait ca :
            //var data = forTest
            //    ? GetData(CreateDataToTestWidget(indicatorWidget), listIndicatorColumns,
            //        listColumnsNotDisplayed, listColumnsAggregate)
            //    : GetData(indicatorWidget.IndicatorDefinitionId, listIndicatorColumns, listColumnsNotDisplayed,
            //        listColumnsAggregate, timeRange);

            IEnumerable<JToken> data = null;

            try
            {
                if ( forTest )
                {
                    data = GetDataForTest( CreateDataToTestWidget( indicatorWidget ), listIndicatorColumns, listColumnsNotDisplayed, listColumnsAggregate );
                }
                else
                {
                    data = GetDataIndicatorValue( indicatorWidget.IndicatorDefinitionId, listIndicatorColumns, listColumnsNotDisplayed, listColumnsAggregate, timeRange );
                }
            }
            catch ( Exception e )
            {
                Logger.LogError( e.Message );
            }

            var listIndicatorTableColumn = indicatorWidget.TableWidgetColumns.OfType<IndicatorTableWidgetColumn>().ToList();
            dynamic transpositionColumn = MonitorComponent.GetTranspositionColumn( listIndicatorTableColumn );

            if ( !string.IsNullOrEmpty( transpositionColumn.ColColumnName ) && !string.IsNullOrEmpty( transpositionColumn.DataColumnName ) )
            {
                data = TranspositionData( data, transpositionColumn.RowColumnName, transpositionColumn.ColColumnName, transpositionColumn.DataColumnName );
            }

            return data;
        }

        public IEnumerable<JToken> CreateDataToTestWidget(IndicatorTableWidget indicatorWidget)
        {
            JTokenWriter writer = new JTokenWriter();
            writer.WriteStartArray();

            var existTitle = indicatorWidget.TableWidgetColumns.OfType<TitleIndicatorTableWidgetColumn>().Any();
            var existTransposition = indicatorWidget.TableWidgetColumns.OfType<TranspositionColumnTableWidgetColumn>().Any();
            var cpt = existTitle && existTransposition == false ? 1 : 5;
            var cptBadge = 1;

            for (var i = 0; i < cpt; i++)
            {
                writer.WriteStartObject();
                foreach (var column in indicatorWidget.TableWidgetColumns)
                {
                    if (column is IndicatorTableWidgetColumn indicatorColumn)
                    {
                        writer.WritePropertyName(column.Code);
                        if (indicatorColumn.IsNumericFormat || indicatorColumn.ColumnStyle == ColumnStyle.Badge ||
                            indicatorColumn.ColumnStyle == ColumnStyle.ProgressBar)
                        {
                            var t = new Random();

                            if (indicatorColumn.ColumnStyle == ColumnStyle.Badge)
                            {
                                writer.WriteValue(cptBadge);
                                cptBadge++;
                            }
                            else if (indicatorColumn.ColumnStyle == ColumnStyle.ProgressBar)
                            {
                                writer.WriteValue(t.Next(100));
                            }
                            else
                            {
                                writer.WriteValue(t.Next(200));
                            }
                        }
                        else
                        {
                            var t = new Random();
                            if (ColumnCodeExistInCalculatedColumn(indicatorWidget, column.Code))
                            {
                                if (ColumnCodeExistInPartialCalculatedColumn(indicatorWidget, column.Code))
                                {
                                    writer.WriteValue(t.Next(0, 100));
                                }
                                else
                                {
                                    writer.WriteValue(t.Next(100, 200));
                                }
                            }
                            else
                            {
                                var list = indicatorWidget.TableWidgetColumns.OfType<TranspositionColumnTableWidgetColumn>().ToList();

                                if (indicatorColumn.TranspositionColumn)
                                {
                                    writer.WriteValue(list.Any() ? list[t.Next(list.Count)].Code : $"state{t.Next(3)}");
                                }
                                else
                                {
                                    writer.WriteValue(list.Any() ? $"Lib {t.Next(3)}" : $"Product{t.Next(20)}");
                                }
                            }
                        }
                    }
                }
                writer.WriteEndObject();
            }

            writer.WriteEndArray();

            return writer.Token;
        }

        private bool ColumnCodeExistInCalculatedColumn(IndicatorTableWidget indicatorWidget, string code)
        {
            var columns = Repository<CalculatedTableWidgetColumn>()
                .Find(x => x.IndicatorTableWidgetId == indicatorWidget.Id && x.PartialValueColumn.Contains(code) || x.TotalValueColumn.Contains(code));

            return columns.Any();
        }

        private bool ColumnCodeExistInPartialCalculatedColumn(IndicatorTableWidget indicatorWidget, string code)
        {
            var columns = Repository<CalculatedTableWidgetColumn>()
                .Find(x => x.IndicatorTableWidgetId == indicatorWidget.Id && x.PartialValueColumn.Contains(code));

            return columns.Any();
        }

        #endregion

        #region Bar Widget Type

        private async Task<string> CreateHtmlBarWidgetAsync(Widget widget, bool forTest, string position)
        {
            var htmlContent = string.Empty;

            Logger.LogInformation( $"Create HtmlBar for widget : {widget.Title}" );

            var indicatorWidget = await Repository<IndicatorBarWidget>().SingleOrDefaultAsync(x => x.WidgetId == widget.Id,
                x => x.IndicatorDefinition, x => x.BarLabelWidgets, x => x.IndicatorBarWidgetColumns, x => x.Widget);

            if (indicatorWidget != null)
            {
                if (indicatorWidget.TitleIndicatorDisplayed)
                {
                    htmlContent += HtmlWidgetComponent.DivBegin();
                    htmlContent += await TableIndicatorTitleAsync(indicatorWidget.IndicatorDefinitionId, indicatorWidget.TitleIndicatorColor, widget.Type);
                    htmlContent += HtmlWidgetComponent.DivEnd();
                }

                var chartName = $"BarChart{widget.Id}";

                htmlContent += HtmlWidgetComponent.ChartName(chartName);

                htmlContent += HtmlWidgetComponent.ScriptBegin();

                htmlContent += await CreateScriptBarChartAsync(chartName, indicatorWidget, forTest, position);

                htmlContent += HtmlWidgetComponent.ScriptEnd();
            }
            return htmlContent;
        }

        private async Task<string> CreateScriptBarChartAsync(string chartName, IndicatorBarWidget indicatorWidget, bool forTest, string position)
        {
            var htmlContent = string.Empty;

            var timeRange = indicatorWidget.Widget.TimeManagementId != null
                ? await TimeManagementBusiness.GetTimeRangeAsync(indicatorWidget.Widget.TimeManagementId.Value)
                : null;

            var result = GetBarChartWidgetData(indicatorWidget, timeRange, forTest);

            var data = result.ToList();
            var targetData = MonitorComponent.GetTargetData(data, indicatorWidget);

            var codeLabels = GetCodeLabelBarData(data, indicatorWidget);

            var barData = await GetLabelDataSetBarChartAsync(data, targetData, codeLabels, indicatorWidget);
            var barOptions = GetOptionsBarChart(indicatorWidget);
            var plugin = GetPluginBarChar(indicatorWidget, targetData, codeLabels);

            htmlContent += HtmlWidgetComponent.BarChartScript(chartName, position, barData, barOptions, plugin);

            return htmlContent;
        }

        private List<string> GetCodeLabelBarData(IEnumerable<JToken> data, IndicatorBarWidget indicatorBarWidget)
        {
            var labels = new List<string>();

            foreach (var barLabel in indicatorBarWidget.BarLabelWidgets.OrderBy(x => x.Sequence))
            {
                labels.Add(barLabel.Name);
            }

            var labelColumnCode = indicatorBarWidget.LabelColumnCode.Replace("[", string.Empty).Replace("]", string.Empty);

            var labelData = from jsonData in data select jsonData[labelColumnCode].Value<string>();
            foreach (var label in labelData)
            {
                if (!labels.Contains(label))
                {
                    labels.Add(label);
                }
            }

            if (indicatorBarWidget.AddTargetBar)
            {
                labels.Add("TARGET");
            }

            return labels;
        }

        private async Task<string> GetLabelDataSetBarChartAsync(List<JToken> data, decimal? targetData, List<string> codeLabels, IndicatorBarWidget indicatorWidget)
        {
            var htmlContent = string.Empty;

            var labels = await GetLabelBarDataAsync(codeLabels, indicatorWidget);

            htmlContent += HtmlWidgetComponent.Labels(labels);

            htmlContent += "datasets:[";

            if (indicatorWidget.AddTargetBar && targetData.HasValue)
            {
                htmlContent += AddTargetLineBar(indicatorWidget, codeLabels.Count, targetData.Value);
            }

            htmlContent += AddBar(indicatorWidget, codeLabels, data, targetData);

            if (indicatorWidget.AddBarStackedToTarget && targetData.HasValue)
            {
                htmlContent += AddBarStackedToTarget(indicatorWidget, codeLabels, data, targetData.Value);
            }

            htmlContent += "]";

            return htmlContent;
        }

        private async Task<List<string>> GetLabelBarDataAsync(List<string> labels, IndicatorBarWidget indicatorBarWidget)
        {
            var list = new List<string>();

            foreach (var label in labels)
            {
                if (indicatorBarWidget.BarLabelWidgets.Any(x => x.Name == label))
                {
                    var barLabelWidget = await Repository<BarLabelWidget>()
                        .SingleOrDefaultAsync(x => x.Name == label && x.IndicatorBarWidgetId == indicatorBarWidget.Id,
                            x => x.BarLabelWidgetLocalizations);

                    if (barLabelWidget.BarLabelWidgetLocalizations.Any(x =>
                        x.LocalizationCode == CultureInfo.CurrentCulture.Name))
                    {
                        list.Add(barLabelWidget.BarLabelWidgetLocalizations
                            .Single(x => x.LocalizationCode == CultureInfo.CurrentCulture.Name).Name);
                    }
                    else
                    {
                        list.Add(label);
                    }
                }
                else
                {
                    list.Add(label);
                }
            }

            return list;
        }

        private string AddTargetLineBar(IndicatorBarWidget indicatorBarWidget, int numberOfPoint, decimal targetData)
        {
            var listData = new List<string>();

            for (var i = 0; i < numberOfPoint; i++)
            {
                listData.Add(string.IsNullOrEmpty(indicatorBarWidget.DecimalMask)
                                    ? targetData.ToString(CultureInfo.CurrentCulture)
                                    : targetData.ToString(indicatorBarWidget.DecimalMask));
            }

            var classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorBarWidget.TargetBarColor);
            return HtmlWidgetComponent.BarLineData("Target", listData, classColor != null ? classColor.HexColorCode : "Black");
        }

        private string AddBar(IndicatorBarWidget indicatorBarWidget, List<string> labels, List<JToken> data, decimal? targetData)
        {
            var listData = new List<string>();
            var listColor = new List<string>();

            foreach (var label in labels)
            {
                if (label == "TARGET")
                {
                    var classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorBarWidget.TargetBarColor);
                    listColor.Add(classColor != null ? classColor.HexColorCode : "Black");

                    var targetValue = targetData ?? 0;

                    listData.Add(string.IsNullOrEmpty(indicatorBarWidget.DecimalMask)
                        ? targetValue.ToString(CultureInfo.CurrentCulture)
                        : targetValue.ToString(indicatorBarWidget.DecimalMask));
                }
                else
                {
                    var lines = (from jsonData in data
                                 where jsonData[indicatorBarWidget.LabelColumnCode].Value<string>() == label
                                 select jsonData).ToList();

                    var val = lines.Sum(m => (decimal)m.SelectToken(indicatorBarWidget.DataColumnCode));

                    listData.Add(string.IsNullOrEmpty(indicatorBarWidget.DecimalMask)
                        ? val.ToString(CultureInfo.CurrentCulture)
                        : val.ToString(indicatorBarWidget.DecimalMask));

                    var classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorBarWidget.BarColor);
                    var color = classColor != null ? classColor.HexColorCode : "Black";

                    var barLabel = indicatorBarWidget.BarLabelWidgets.FirstOrDefault(x => x.Name == label);
                    if (barLabel != null && barLabel.UseLabelColorForBar)
                    {
                        if (!string.IsNullOrEmpty(barLabel.LabelTextColor))
                        {
                            classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == barLabel.LabelTextColor);
                            color = classColor != null ? classColor.HexColorCode : "Black";
                        }
                    }

                    listColor.Add(color);
                }
            }

            return HtmlWidgetComponent.BarData("defaultBar", listData, listColor, "s1");
        }

        private string AddBarStackedToTarget(IndicatorBarWidget indicatorBarWidget, List<string> labels, List<JToken> data, decimal target)
        {
            var listData = new List<string>();

            foreach (var label in labels)
            {
                if (label != "TARGET")
                {
                    var lines = (from jsonData in data
                                 where jsonData[indicatorBarWidget.LabelColumnCode].Value<string>() == label
                                 select jsonData).ToList();

                    var val = target - lines.Sum(m => (decimal)m.SelectToken(indicatorBarWidget.DataColumnCode));

                    listData.Add(string.IsNullOrEmpty(indicatorBarWidget.DecimalMask)
                        ? val.ToString(CultureInfo.CurrentCulture)
                        : val.ToString(indicatorBarWidget.DecimalMask));
                }
                else
                {
                    listData.Add("0");
                }
            }

            var classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorBarWidget.BarColorStackedToTarget);

            var barStackedColor = classColor != null ? classColor.HexColorCode : "Black";

            return HtmlWidgetComponent.BarData("stackedToTarget", listData, barStackedColor, "s1");
        }

        private string GetOptionsBarChart(IndicatorBarWidget indicatorWidget)
        {
            var classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorWidget.TextDataAxeYColor);

            var dataAxeYColor = classColor != null ? classColor.HexColorCode : "Black";

            classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorWidget.LabelColorText);

            var labelColor = classColor != null ? classColor.HexColorCode : "Black";

            var isStacked = indicatorWidget.AddBarStackedToTarget;

            return HtmlWidgetComponent.BarOption(indicatorWidget.DisplayAxeX, indicatorWidget.DisplayAxeY,
                                                    indicatorWidget.DisplayGridLinesAxeY, indicatorWidget.DisplayDataAxeY,
                                                    dataAxeYColor, labelColor, indicatorWidget.LabelFontSize, isStacked);
        }

        private string GetPluginBarChar(IndicatorBarWidget indicatorWidget, decimal? targetData, List<string> codeLabels)
        {
            var indexList = MonitorComponent.GetListDataSetIndex(indicatorWidget, targetData);

            var targetIndex = codeLabels.IndexOf("TARGET");

            var afterDataSetUpdate = HtmlWidgetComponent.AfterDatasetUpdateScript(indexList, targetIndex, 90, 10);

            var labelBars = new List<string>();
            labelBars.Add("defaultBar");

            var labelColors = GetLabelColor(indicatorWidget, codeLabels);

            var classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorWidget.TargetBarColor);
            var targetColor = classColor != null ? classColor.HexColorCode : "Black";

            classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorWidget.DataLabelInBarColor);
            var dataLabelColor = classColor != null ? classColor.HexColorCode : "Black";

            var afterDraw = HtmlWidgetComponent.AfterDraw(labelBars, indicatorWidget.DisplayDataLabelInBar,
                dataLabelColor, indicatorWidget.FontSizeDataLabel, targetIndex,
                targetColor, indicatorWidget.LabelFontSize, labelColors);

            return $"{afterDataSetUpdate} {afterDraw}";
        }

        public List<string> GetLabelColor(IndicatorBarWidget indicatorWidget, List<string> codeLabels)
        {
            var colors = new List<string>();

            foreach (var label in codeLabels)
            {
                if (label == "TARGET")
                {
                    var classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorWidget.TargetBarColor);
                    colors.Add(classColor != null ? classColor.HexColorCode : "Black");
                }
                else
                {
                    var barLabel = indicatorWidget.BarLabelWidgets.FirstOrDefault(x => x.Name == label);
                    if (barLabel != null)
                    {
                        if (string.IsNullOrEmpty(barLabel.LabelTextColor))
                        {
                            var classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorWidget.LabelColorText);
                            colors.Add(classColor != null ? classColor.HexColorCode : "Black");
                        }
                        else
                        {
                            var classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == barLabel.LabelTextColor);
                            colors.Add(classColor != null ? classColor.HexColorCode : "Black");
                        }
                    }
                    else
                    {
                        var classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorWidget.LabelColorText);
                        colors.Add(classColor != null ? classColor.HexColorCode : "Black");
                    }
                }
            }

            return colors;
        }

        public IEnumerable<JToken> GetBarChartWidgetData(IndicatorBarWidget indicatorWidget, TimeRange timeRange, bool forTest = false)
        {
            var listIndicatorColumns = indicatorWidget.IndicatorBarWidgetColumns.OfType<IIndicatorColumn>().ToList();
            var listColumnsNotDisplayed = MonitorComponent.GetColumnsNotDisplayed(indicatorWidget);
            var listColumnsAggregate = MonitorComponent.GetColumnsToAggregate(indicatorWidget);

            // BRY_WORK
            //var data = forTest
            //    ? GetData(CreateDataToTestWidget(indicatorWidget), listIndicatorColumns,
            //        listColumnsNotDisplayed, listColumnsAggregate)
            //    : GetData(indicatorWidget.IndicatorDefinitionId, listIndicatorColumns, listColumnsNotDisplayed,
            //        listColumnsAggregate, timeRange);

            IEnumerable<JToken> data = null;

            try
            {
                if ( forTest )
                {
                    data = GetDataForTest( CreateDataToTestWidget( indicatorWidget ), listIndicatorColumns, listColumnsNotDisplayed, listColumnsAggregate );
                }
                else
                {
                    data = GetDataIndicatorValue( indicatorWidget.IndicatorDefinitionId, listIndicatorColumns, listColumnsNotDisplayed, listColumnsAggregate, timeRange );
                }
            }
            catch ( Exception e )
            {
                Logger.LogError( e.Message );
            }

            return data;
        }

        public IEnumerable<JToken> CreateDataToTestWidget(IndicatorBarWidget indicatorWidget)
        {
            JTokenWriter writer = new JTokenWriter();
            writer.WriteStartArray();

            for (int i = 0; i < 10; i++)
            {
                writer.WriteStartObject();
                foreach (var column in indicatorWidget.IndicatorBarWidgetColumns)
                {
                    writer.WritePropertyName(column.Code);
                    var t = new Random();
                    if (column.IsNumericFormat)
                    {
                        writer.WriteValue(t.Next(200));
                    }
                    else
                    {
                        if (indicatorWidget.BarLabelWidgets.Any())
                        {
                            var index = t.Next(indicatorWidget.BarLabelWidgets.Count);
                            writer.WriteValue(indicatorWidget.BarLabelWidgets.ToArray()[index].Name);
                        }
                        else
                        {
                            writer.WriteValue($"Libelle {t.Next(4)}");
                        }
                    }
                }
                writer.WriteEndObject();
            }

            writer.WriteEndArray();

            return writer.Token;
        }

        #endregion

        #region Chart Widget Type

        private async Task<string> CreateHtmlChartWidgetAsync(Widget widget, TimeZoneInfo timeZoneInfo, bool forTest, string position)
        {
            var htmlContent = string.Empty;

            Logger.LogInformation( $"Create HtmlChart for widget : {widget.Title}" );

            var indicatorWidget = await Repository<IndicatorChartWidget>().SingleOrDefaultAsync(x => x.WidgetId == widget.Id,
                x => x.IndicatorDefinition, x => x.TargetIndicatorChartWidgets, x => x.Widget);

            if (indicatorWidget != null)
            {
                if (indicatorWidget.TitleIndicatorDisplayed)
                {
                    htmlContent += HtmlWidgetComponent.DivBegin();
                    htmlContent += await TableIndicatorTitleAsync(indicatorWidget.IndicatorDefinitionId, indicatorWidget.TitleIndicatorColor, widget.Type);
                    htmlContent += HtmlWidgetComponent.DivEnd();
                }

                var chartName = $"Chart{widget.Id}";

                htmlContent += HtmlWidgetComponent.ChartName(chartName);

                htmlContent += HtmlWidgetComponent.ScriptBegin();

                htmlContent += await CreateScriptChartAsync(chartName, indicatorWidget, timeZoneInfo, forTest, position);

                htmlContent += HtmlWidgetComponent.ScriptEnd();
            }
            return htmlContent;
        }

        private async Task<string> CreateScriptChartAsync(string chartName, IndicatorChartWidget indicatorWidget, TimeZoneInfo timeZoneInfo, bool forTest, string position)
        {
            var htmlContent = string.Empty;

            var timeRange = indicatorWidget.Widget.TimeManagementId != null
                ? await TimeManagementBusiness.GetTimeRangeAsync(indicatorWidget.Widget.TimeManagementId.Value)
                : null;

            if (timeRange == null)
            {
                return htmlContent;
            }

            var data = GetChartWidgetData(indicatorWidget, timeRange, forTest).ToList();
            htmlContent += HtmlWidgetComponent.CreateVariableData(data, indicatorWidget.DecimalMask, timeZoneInfo, true);

            var yMinValue = GetYMinValue(
                indicatorWidget.AxeYIsAutoAdjustableAccordingMinValue, 
                indicatorWidget.AxeYOffsetFromMinValue, 
                data);

            var dateMinUtc = timeRange.StartTimeUtc;
            var dateMaxUtc = DateTime.UtcNow;

            if (timeRange.EndTimeUtc.HasValue)
            {
                dateMaxUtc = timeRange.EndTimeUtc.Value;
            }

            var dataSets = string.Empty;

            if (indicatorWidget.ChartTargetDisplayed)
            {
                dataSets += AddChartDataSetTarget(indicatorWidget, timeZoneInfo, dateMinUtc, dateMaxUtc);
            }

            if (indicatorWidget.ChartAverageDisplayed && data.Any())
            {
                dataSets += AddChartDataSetAverage(indicatorWidget, data, timeZoneInfo, dateMinUtc, dateMaxUtc);
            }

            var classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorWidget.ChartDataColor);
            var chartDataColor = classColor != null ? classColor.HexColorCode : "Black";

            if (data.Any())
            {
                dataSets += HtmlWidgetComponent.ScriptChartData(string.Empty, "data", indicatorWidget.ChartDataFill, chartDataColor, 1, false);
            }

            var dateMinLocal = timeZoneInfo != null
                ? TimeZoneInfo.ConvertTimeFromUtc(dateMinUtc, timeZoneInfo)
                : dateMinUtc.ToLocalTime();

            var dateMaxLocal = timeZoneInfo != null
                ? TimeZoneInfo.ConvertTimeFromUtc(dateMaxUtc, timeZoneInfo)
                : dateMaxUtc.ToLocalTime();

            classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorWidget.AxeXColor);
            var axeXColor = classColor != null ? classColor.HexColorCode : "Black";

            classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorWidget.AxeYColor);
            var axeYColor = classColor != null ? classColor.HexColorCode : "Black";

            var options = HtmlWidgetComponent.ScriptChartOptions(false, indicatorWidget.AxeXDisplayed,
                indicatorWidget.AxeYDisplayed, indicatorWidget.AxeFontSize, axeXColor,
                axeYColor, indicatorWidget.AxeYDataDisplayed, dateMinLocal, dateMaxLocal, yMinValue);

            var plugin = HtmlWidgetComponent.ScriptPluginChart(indicatorWidget.AxeFontSize);

            htmlContent += HtmlWidgetComponent.getChartConfig("labels", dataSets, options, plugin);

            htmlContent += HtmlWidgetComponent.ScriptChart(chartName, position);

            return htmlContent;
        }

        private static int GetYMinValue(bool axeYIsAutoAdjustableAccordingMinValue, int? axeYOffsetFromMinValue, IEnumerable<JToken> data)
        {
            var yDataMinValue  = GetYDataMinValue(data);
            var yMinValue = 0;
            if (yDataMinValue > 10)
            {
                yMinValue = axeYIsAutoAdjustableAccordingMinValue
                    ? yDataMinValue - axeYOffsetFromMinValue ?? yDataMinValue
                    : 0;
            }

            return yMinValue;
        }

        private static int GetYDataMinValue(IEnumerable<JToken> data)
        {
            decimal dataMin = 0;

            if (data == null || !data.Any())
            {
                return (int)dataMin;
            }

            try
            {
                dataMin = data.Min(x => x.SelectToken("Value").Value<decimal>());
            }
            catch (Exception e)
            {
                Logger.LogError($"Error during get Y data min value of data. ExceptionMessage: {e.Message}.");
                return 0;
            }

            return (int)dataMin;
        }

        private string AddChartDataSetTarget(IndicatorChartWidget indicatorWidget, TimeZoneInfo timeZoneInfo, DateTime dateMinUtc, DateTime dateMaxUtc)
        {
            var dataSet = string.Empty;

            if (indicatorWidget.ChartTargetDisplayed)
            {
                var dateMinLocal = timeZoneInfo != null
                    ? TimeZoneInfo.ConvertTimeFromUtc(dateMinUtc, timeZoneInfo)
                    : dateMinUtc.ToLocalTime();

                var dateMaxLocal = timeZoneInfo != null
                    ? TimeZoneInfo.ConvertTimeFromUtc(dateMaxUtc, timeZoneInfo)
                    : dateMaxUtc.ToLocalTime();

                if (indicatorWidget.TargetIndicatorChartWidgets.Any())
                {
                    var targetData = string.Empty;
                    foreach (var target in indicatorWidget.TargetIndicatorChartWidgets)
                    {
                        var currentDate = DateTime.UtcNow;

                        var startTargetDateUtc = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, target.StartDateUtc.Hour, target.StartDateUtc.Minute, 0);
                        var endTargetDateUtc = new DateTime();

                        if (target.EndDateUtc >= target.StartDateUtc)
                        {
                            endTargetDateUtc = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day,
                                target.EndDateUtc.Hour, target.EndDateUtc.Minute, 0);
                        }
                        if (target.EndDateUtc < target.StartDateUtc)
                        {
                            var newDate = currentDate.AddDays(1);
                            endTargetDateUtc = new DateTime(newDate.Year, newDate.Month, newDate.Day,
                                target.EndDateUtc.Hour, target.EndDateUtc.Minute, 0);
                        }

                        if (startTargetDateUtc >= dateMinUtc && startTargetDateUtc <= dateMaxUtc &&
                            endTargetDateUtc >= dateMinUtc && endTargetDateUtc <= dateMaxUtc)
                        {
                            var startTargetDateLocal = timeZoneInfo != null
                                ? TimeZoneInfo.ConvertTimeFromUtc(startTargetDateUtc, timeZoneInfo)
                                : startTargetDateUtc.ToLocalTime();

                            targetData += HtmlWidgetComponent.ScriptChartDataset(startTargetDateLocal, target.StartTargetValue, indicatorWidget.DecimalMask);

                            var endTargetDateLocal = timeZoneInfo != null
                                ? TimeZoneInfo.ConvertTimeFromUtc(endTargetDateUtc, timeZoneInfo)
                                : endTargetDateUtc.ToLocalTime();

                            targetData += HtmlWidgetComponent.ScriptChartDataset(endTargetDateLocal, target.EndTargetValue, indicatorWidget.DecimalMask);
                        }
                    }

                    var classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorWidget.ChartTargetColor);
                    var charTargetColor = classColor != null ? classColor.HexColorCode : "Black";

                    dataSet += HtmlWidgetComponent.ScriptChartData("TARGET",
                        HtmlWidgetComponent.ScriptChartDataset(targetData), false, charTargetColor, 5, true);
                }
                else
                {
                    if (indicatorWidget.TargetValue.HasValue)
                    {
                        var targetData = HtmlWidgetComponent.ScriptChartDataset(dateMinLocal,
                            indicatorWidget.TargetValue.Value, indicatorWidget.DecimalMask);

                        targetData += HtmlWidgetComponent.ScriptChartDataset(dateMaxLocal,
                            indicatorWidget.TargetValue.Value, indicatorWidget.DecimalMask);

                        var classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorWidget.ChartTargetColor);
                        var charTargetColor = classColor != null ? classColor.HexColorCode : "Black";

                        dataSet += HtmlWidgetComponent.ScriptChartData("TARGET",
                            HtmlWidgetComponent.ScriptChartDataset(targetData), false, charTargetColor, 5, true);
                    }
                }
            }

            return dataSet;
        }

        private string AddChartDataSetAverage(IndicatorChartWidget indicatorWidget, List<JToken> data, TimeZoneInfo timeZoneInfo, DateTime dateMinUtc, DateTime dateMaxUtc)
        {
            var dataSet = string.Empty;

            if (!indicatorWidget.ChartAverageDisplayed || !data.Any()) return dataSet;

            var dateMinLocal = timeZoneInfo != null
                ? TimeZoneInfo.ConvertTimeFromUtc(dateMinUtc, timeZoneInfo)
                : dateMinUtc.ToLocalTime();

            var dateMaxLocal = timeZoneInfo != null
                ? TimeZoneInfo.ConvertTimeFromUtc(dateMaxUtc, timeZoneInfo)
                : dateMaxUtc.ToLocalTime();

            var avg = data.Average(m => (decimal)m.SelectToken("Value"));

            var avgData = HtmlWidgetComponent.ScriptChartDataset(dateMinLocal, avg, indicatorWidget.DecimalMask);
            avgData += HtmlWidgetComponent.ScriptChartDataset(dateMaxLocal, avg, indicatorWidget.DecimalMask);

            var classColor = Repository<ColorHtml>().SingleOrDefault(x => x.Name == indicatorWidget.ChartAverageColor);
            var charAverageColor = classColor != null ? classColor.HexColorCode : "Black";

            dataSet += HtmlWidgetComponent.ScriptChartData("AVERAGE",
                HtmlWidgetComponent.ScriptChartDataset(avgData), false, charAverageColor, 5, false);

            return dataSet;
        }

        public IEnumerable<JToken> GetChartWidgetData(IndicatorChartWidget indicatorWidget, TimeRange timeRange, bool forTest = false)
        {
            var listIndicatorColumns = indicatorWidget.IndicatorChartWidgetColumns.OfType<IIndicatorColumn>().ToList();
            var listColumnsNotDisplayed = MonitorComponent.GetColumnsNotDisplayed(indicatorWidget);
            var listColumnsAggregate = MonitorComponent.GetColumnsToAggregate(indicatorWidget);

            if (forTest)
            {
                return GetDataForTest(CreateDataToTestWidget(indicatorWidget, timeRange), listIndicatorColumns,
                    listColumnsNotDisplayed, listColumnsAggregate);
            }

            var indicatorDefinition = Repository<IndicatorDefinition>().SingleOrDefault(x => x.Id == indicatorWidget.IndicatorDefinitionId);

            switch (indicatorDefinition.Type)
            {
                case IndicatorType.Snapshot:
                    throw new InvalidOperationException();

                case IndicatorType.Flow:
                    return GetDataIndicatorValue(indicatorWidget.IndicatorDefinitionId, listIndicatorColumns, listColumnsNotDisplayed,
                        listColumnsAggregate, timeRange);

                case IndicatorType.Ratio:
                    var indicator = (IndicatorCalculated)indicatorDefinition;

                    var data1 = GetDataIndicatorValue(indicator.IndicatorDefinitionId1, listIndicatorColumns, listColumnsNotDisplayed,
                        listColumnsAggregate, timeRange);

                    var data2 = GetDataIndicatorValue(indicator.IndicatorDefinitionId2, listIndicatorColumns, listColumnsNotDisplayed,
                        listColumnsAggregate, timeRange);

                    return GetDataCalculated(data1, data2);

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private IEnumerable<JToken> GetDataCalculated(IEnumerable<JToken> data1, IEnumerable<JToken> data2)
        {
            var result = new JArray();

            foreach (var line in data1)
            {
                var dateUtc = line.SelectToken("DateUtc").Value<DateTime>();

                var line2 = (from jsonData in data2
                             where jsonData["DateUtc"].Value<DateTime>() == dateUtc
                             select jsonData).FirstOrDefault();

                if (line2 != null)
                {
                    var val1 = line.SelectToken("Value").Value<decimal>();
                    var val2 = line2.SelectToken("Value").Value<decimal>();

                    line["Value"] = val2 != 0 ? val1 / val2 : 0;

                    result.Add(line);
                }
            }

            return result;
        }

        public IEnumerable<JToken> CreateDataToTestWidget(IndicatorChartWidget indicatorWidget, TimeRange timeRange)
        {
            var writer = new JTokenWriter();
            writer.WriteStartArray();

            var valueInit = indicatorWidget.TargetValue ?? 100;
            var decrement = 0m;

            if (indicatorWidget.TargetIndicatorChartWidgets.Any())
            {
                var dateMinUtc = timeRange.StartTimeUtc;
                var dateMaxUtc = DateTime.UtcNow;

                if (timeRange.EndTimeUtc.HasValue)
                {
                    dateMaxUtc = timeRange.EndTimeUtc.Value;
                }

                var currentDate = DateTime.UtcNow;

                foreach (var target in indicatorWidget.TargetIndicatorChartWidgets)
                {
                    var startTargetDateUtc = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day,
                        target.StartDateUtc.Hour, target.StartDateUtc.Minute, 0);

                    var endTargetDateUtc = new DateTime();

                    if (target.EndDateUtc >= target.StartDateUtc)
                    {
                        endTargetDateUtc = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day,
                            target.EndDateUtc.Hour, target.EndDateUtc.Minute, 0);
                    }

                    if (target.EndDateUtc < target.StartDateUtc)
                    {
                        var newDate = currentDate.AddDays(1);
                        endTargetDateUtc = new DateTime(newDate.Year, newDate.Month, newDate.Day,
                            target.EndDateUtc.Hour, target.EndDateUtc.Minute, 0);
                    }

                    if (startTargetDateUtc >= dateMinUtc && startTargetDateUtc <= dateMaxUtc &&
                        endTargetDateUtc >= dateMinUtc && endTargetDateUtc <= dateMaxUtc)
                    {
                        valueInit = target.StartTargetValue;
                        var delta = endTargetDateUtc - startTargetDateUtc;
                        decrement = valueInit / Convert.ToDecimal(delta.TotalMinutes / 15);
                        break;
                    }
                }
            }

            var dateDeb = timeRange.StartTimeUtc;
            var dateFin = DateTime.UtcNow;

            var dateData = dateDeb;
            var currentValue = valueInit;

            while (dateData < dateFin)
            {
                writer.WriteStartObject();
                writer.WritePropertyName("DateUtc");
                writer.WriteValue(dateData);
                writer.WritePropertyName("Group1");
                writer.WriteValue(indicatorWidget.Group1);
                writer.WritePropertyName("Group2");
                writer.WriteValue(indicatorWidget.Group2);
                writer.WritePropertyName("Group3");
                writer.WriteValue(indicatorWidget.Group3);
                writer.WritePropertyName("Group4");
                writer.WriteValue(indicatorWidget.Group4);
                writer.WritePropertyName("Group5");
                writer.WriteValue(indicatorWidget.Group5);
                writer.WritePropertyName("Value");

                var t = new Random();
                writer.WriteValue(t.Next(Convert.ToInt32(currentValue) - 10, Convert.ToInt32(currentValue) + 10));
                writer.WriteEndObject();

                dateData = dateData.AddMinutes(15);
                currentValue = currentValue - decrement;
                if (currentValue < 10) currentValue = 10;
            }

            writer.WriteEndArray();

            return writer.Token;
        }

        #endregion

        #region Gauge Widget Type

        private async Task<string> CreateHtmlGaugeWidgetAsync(Widget widget, TimeZoneInfo timeZoneInfo, bool forTest, string position)
        {
            var htmlContent = string.Empty;

            Logger.LogInformation( $"Create HtmlGauge for widget : {widget.Title}" );

            var indicatorWidget = await Repository<IndicatorGaugeWidget>().SingleOrDefaultAsync(x => x.WidgetId == widget.Id,
                x => x.IndicatorDefinition, x => x.Widget);

            if (indicatorWidget != null)
            {
                if (indicatorWidget.TitleIndicatorDisplayed)
                {
                    htmlContent += HtmlWidgetComponent.DivBegin();
                    htmlContent += await TableIndicatorTitleAsync(indicatorWidget.IndicatorDefinitionId, indicatorWidget.TitleIndicatorColor, widget.Type);
                    htmlContent += HtmlWidgetComponent.DivEnd();
                }

                if (indicatorWidget.TargetDisplayed)
                {
                    htmlContent += HtmlWidgetComponent.DivBegin();
                    htmlContent += await GetGaugeTargetAsync(indicatorWidget);
                    htmlContent += HtmlWidgetComponent.DivEnd();
                }

                var chartName = $"Gauge{widget.Id}";

                htmlContent += HtmlWidgetComponent.ChartName(chartName);

                htmlContent += HtmlWidgetComponent.ScriptBegin();

                htmlContent += await GetScriptGaugeAsync(chartName, indicatorWidget, timeZoneInfo, forTest, position);

                htmlContent += HtmlWidgetComponent.ScriptEnd();
            }

            return htmlContent;
        }

        private async Task<string> GetScriptGaugeAsync(string chartName, IndicatorGaugeWidget indicatorWidget, TimeZoneInfo timeZoneInfo, bool forTest, string position)
        {
            var htmlContent = string.Empty;

            var timeRange = indicatorWidget.Widget.TimeManagementId != null
                ? await TimeManagementBusiness.GetTimeRangeAsync(indicatorWidget.Widget.TimeManagementId.Value)
                : null;

            var data = GetGaugeWidgetData(indicatorWidget, timeRange, forTest);

            var target = 0m;
            if (indicatorWidget.TargetDisplayed && indicatorWidget.TargetValue.HasValue)
            {
                target = indicatorWidget.TargetValue.Value;
            }

            var targetClassColor = await Repository<ColorHtml>().SingleOrDefaultAsync(x => x.Name == indicatorWidget.GaugeTargetColor) ??
                                   new ColorHtml { HexColorCode = "#656567" };
            var range1ClassColor = await Repository<ColorHtml>().SingleOrDefaultAsync(x => x.Name == indicatorWidget.GaugeRange1Color) ?? 
                                   new ColorHtml { HexColorCode = "#F6D9B6" };
            var range2ClassColor = await Repository<ColorHtml>().SingleOrDefaultAsync(x => x.Name == indicatorWidget.GaugeRange2Color) ?? 
                                   new ColorHtml();
            var range3ClassColor = await Repository<ColorHtml>().SingleOrDefaultAsync(x => x.Name == indicatorWidget.GaugeRange3Color) ??
                                   new ColorHtml();

            var valueMin = indicatorWidget.GaugeRange1MinValue ?? 0;

            var valueMax = indicatorWidget.GaugeRange3Displayed != null && indicatorWidget.GaugeRange3Displayed.Value && indicatorWidget.GaugeRange3MaxValue != null
                ? indicatorWidget.GaugeRange3MaxValue.Value
                : (indicatorWidget.GaugeRange2Displayed != null && indicatorWidget.GaugeRange2Displayed.Value && indicatorWidget.GaugeRange2MaxValue != null
                    ? indicatorWidget.GaugeRange2MaxValue.Value
                    : indicatorWidget.GaugeRange1MaxValue ?? 600);

            var range2Displayed = indicatorWidget.GaugeRange2Displayed != null && indicatorWidget.GaugeRange2Displayed.Value;
            var range3Displayed = indicatorWidget.GaugeRange3Displayed != null && indicatorWidget.GaugeRange3Displayed.Value;

            var range1MinValue = 0m;
            var range1MaxValue = 600m;
            if (indicatorWidget.GaugeRange1MinValue.HasValue)
            {
                range1MinValue = indicatorWidget.GaugeRange1MinValue.Value;
            }
            if (indicatorWidget.GaugeRange1MaxValue.HasValue)
            {
                range1MaxValue = indicatorWidget.GaugeRange1MaxValue.Value;
            }

            var range2MinValue = 0m;
            var range2MaxValue = 0m;
            if (range2Displayed)
            {
                if (indicatorWidget.GaugeRange2MinValue.HasValue)
                {
                    range2MinValue = indicatorWidget.GaugeRange2MinValue.Value;
                }
                if (indicatorWidget.GaugeRange2MaxValue.HasValue)
                {
                    range2MaxValue = indicatorWidget.GaugeRange2MaxValue.Value;
                }
            }
            
            var range3MinValue = 0m;
            var range3MaxValue = 0m;
            if (range3Displayed)
            {
                if (indicatorWidget.GaugeRange3MinValue.HasValue)
                {
                    range3MinValue = indicatorWidget.GaugeRange3MinValue.Value;
                }
                if (indicatorWidget.GaugeRange3MaxValue.HasValue)
                {
                    range3MaxValue = indicatorWidget.GaugeRange3MaxValue.Value;
                }
            }

            htmlContent += HtmlWidgetComponent.GaugeOption(Convert.ToInt32(target), targetClassColor.HexColorCode,
                range1ClassColor.HexColorCode, Convert.ToInt32(range1MinValue), Convert.ToInt32(range1MaxValue),
                range2Displayed, range2ClassColor.HexColorCode, Convert.ToInt32(range2MinValue), Convert.ToInt32(range2MaxValue),
                range3Displayed, range3ClassColor.HexColorCode, Convert.ToInt32(range3MinValue), Convert.ToInt32(range3MaxValue));

            htmlContent += HtmlWidgetComponent.ScriptGauge(chartName, position, Convert.ToInt32(valueMin), Convert.ToInt32(valueMax), Convert.ToInt32(data));

            return htmlContent;
        }

        public decimal GetGaugeWidgetData(IndicatorGaugeWidget indicatorWidget, TimeRange timeRange, bool forTest = false)
        {
            if ( forTest )
            {
                return CreateGaugeDataToTestWidget( indicatorWidget );
            }

            if (timeRange == null) { return 0; }

            var listIndicatorColumns = indicatorWidget.IndicatorGaugeWidgetColumns.OfType<IIndicatorColumn>().ToList();
            var listColumnsNotDisplayed = MonitorComponent.GetColumnsNotDisplayed(indicatorWidget);

            var indicatorDefinition = Repository<IndicatorDefinition>().SingleOrDefault(x => x.Id == indicatorWidget.IndicatorDefinitionId);

            switch (indicatorDefinition.Type)
            {
                case IndicatorType.Snapshot:
                    throw new InvalidOperationException();

                case IndicatorType.Flow:
                    var data = GetLastFlowValueIndicator(indicatorWidget.IndicatorDefinitionId, timeRange.StartTimeUtc);
                    data = GetDataForTest(data, listIndicatorColumns, listColumnsNotDisplayed, new List<string>());

                    return data.Sum(m => (decimal)m.SelectToken("Value"));

                case IndicatorType.Ratio:
                    var indicator = (IndicatorCalculated)indicatorDefinition;

                    var data1 = GetLastFlowValueIndicator(indicator.IndicatorDefinitionId1, timeRange.StartTimeUtc);
                    data1 = GetDataForTest(data1, listIndicatorColumns, listColumnsNotDisplayed, new List<string>());

                    var data2 = GetLastFlowValueIndicator(indicator.IndicatorDefinitionId2, timeRange.StartTimeUtc);
                    data2 = GetDataForTest(data2, listIndicatorColumns, listColumnsNotDisplayed, new List<string>());

                    var val1 = data1.Sum(m => (decimal)m.SelectToken("Value"));
                    var val2 = data2.Sum(m => (decimal)m.SelectToken("Value"));

                    return val2 != 0 ? val1 / val2 : 0;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task<string> GetGaugeTargetAsync( IndicatorGaugeWidget indicatorWidget )
        {
            var target = 0m;
            if ( indicatorWidget.TargetDisplayed && indicatorWidget.TargetValue.HasValue )
            {
                target = indicatorWidget.TargetValue.Value;
            }

            var targetClassColor = await Repository<ColorHtml>().SingleOrDefaultAsync( x => x.Name == indicatorWidget.GaugeTargetColor ) ??
                                   new ColorHtml { HexColorCode = "#656567" };

            var targetToDisplayed = $"TARGET : {Convert.ToInt32( target )}";

            return $"<h5 class='{targetClassColor.TxtClassName} {HtmlWidgetComponent.GetTextAlignClass( AlignStyle.Right )}'><strong>{targetToDisplayed}</strong></h5>";
        }

        public decimal CreateGaugeDataToTestWidget(IndicatorGaugeWidget indicatorWidget)
        {
            var t = new Random();

            if (indicatorWidget.TargetDisplayed && indicatorWidget.TargetValue.HasValue)
            {
                var target = Convert.ToInt32(indicatorWidget.TargetValue.Value);
                var min = target - (target * 10 / 100);
                var max = target + (target * 10 / 100);
                return t.Next(min, max);
            }

            return t.Next(200, 400);
        }

        #endregion

        #region Table Widget Type

        private async Task<string> TableIndicatorTitleAsync(long id, string color, WidgetType type)
        {
            var title = await IndicatorTitleAsync( id );
            var htmlContent = string.Empty;

            var colorHtml = await Repository<ColorHtml>().SingleOrDefaultAsync( x => x.Name == color );

            var align = type == WidgetType.MultiVerticalTable ? AlignStyle.Center : AlignStyle.Left;
            if ( type == WidgetType.Bar || type == WidgetType.Chart )
            {
                htmlContent = HtmlWidgetComponent.HtmlTitleH1( title, colorHtml, align );
            }
            else
            {
                htmlContent = HtmlWidgetComponent.HtmlTitleH3( title, colorHtml, align );
            }

            return htmlContent;
        }
         
        #endregion

        #region Indicator Base Widget Type

        private async Task<string> IndicatorTitleAsync(long id)
        {
            var indicatorDefinition = await Repository<IndicatorDefinition>()
                .SingleOrDefaultAsync(x => x.Id == id, x => x.IndicatorLocalizations);

            var title = indicatorDefinition.Title;

            if (indicatorDefinition.IndicatorLocalizations.Any(x => x.LocalizationCode == CultureInfo.CurrentCulture.Name))
            {
                title = indicatorDefinition.IndicatorLocalizations
                    .Single(x => x.LocalizationCode == CultureInfo.CurrentCulture.Name).Title;
            }

            return title;
        }

        public virtual IEnumerable<JToken> GetSnapshotValueIndicator(long indicatorDefinitionId, DateTime? date)
        {
            SnapshotIndicatorValue data;

            if (date == null)
            {
                data = Repository<SnapshotIndicatorValue>()
                    .Find(x => x.IndicatorDefinitionId == indicatorDefinitionId).LastOrDefault();
            }
            else
            {
                data = Repository<SnapshotIndicatorValue>()
                    .Find(x => x.IndicatorDefinitionId == indicatorDefinitionId && x.DateUtc >= date).LastOrDefault();
            }

            return data != null ? JArray.Parse(data.TableValue) : new JArray();
        }

        public virtual IEnumerable<JToken> GetFlowValueIndicator(long indicatorDefinitionId, DateTime dateUtc)
        {
            var data = Repository<FlowIndicatorValue>()
                .Find(x => x.IndicatorDefinitionId == indicatorDefinitionId && x.DateUtc >= dateUtc)
                .Select(MonitorComponent.GetChartValue).ToList();

            return data.Any() ? (JArray)JToken.FromObject(data) : new JArray();
        }

        public virtual IEnumerable<JToken> GetLastFlowValueIndicator(long indicatorDefinitionId, DateTime dateUtc)
        {
            var query = Repository<FlowIndicatorValue>()
                .Find(x => x.IndicatorDefinitionId == indicatorDefinitionId && x.DateUtc >= dateUtc);

            var dateMax = new DateTime();

            if (query.Any())
            {
                dateMax = query.Max(x => x.DateUtc);
            }
            else
            {
                return new JArray();
            }

            var data = Repository<FlowIndicatorValue>()
                .Find(x => x.IndicatorDefinitionId == indicatorDefinitionId && x.DateUtc == dateMax)
                .Select(MonitorComponent.GetLastChartValue).ToList();

            return data.Any() ? (JArray)JToken.FromObject(data) : new JArray();
        }

        #endregion

        #region Operation Data : Get / Filter/ Aggregation / Transposition

        private IEnumerable<JToken> GetDataForTest(IEnumerable<JToken> data, List<IIndicatorColumn> indicatorsColumns, List<string> columnNameNotDisplayed, List<string> columnNameToAggregate)
        {
            data = FilterData(data, indicatorsColumns);

            if (columnNameNotDisplayed.Any())
            {
                data = FilterDataColumn(data, columnNameNotDisplayed);
            }

            if (columnNameToAggregate.Any())
            {
                data = Aggregation(data.ToList(), indicatorsColumns, columnNameToAggregate);
            }

            return data;
        }

		// BRY_WORK 17/02/2020
        private IEnumerable<JToken> GetDataIndicatorValue( long indicatorDefinitionId, List<IIndicatorColumn> indicatorsColumns, List<string> columnNameNotDisplayed, List<string> columnNameToAggregate, TimeRange timeRange )
        {
            if ( timeRange == null )
            {
                return new JArray();
            }

            var indicatorDefinition = Repository<IndicatorDefinition>().SingleOrDefault( x => x.Id == indicatorDefinitionId );

            var data = indicatorDefinition.Type == IndicatorType.Flow
                ? GetFlowValueIndicator( indicatorDefinition.Id, timeRange.StartTimeUtc )
                : GetSnapshotValueIndicator( indicatorDefinition.Id, timeRange.StartTimeUtc );

            data = FilterData( data, indicatorsColumns );

            if ( columnNameNotDisplayed.Any() )
            {
                data = FilterDataColumn( data, columnNameNotDisplayed );
            }

            if ( columnNameToAggregate.Any() )
            {
                data = Aggregation( data.ToList(), indicatorsColumns, columnNameToAggregate );
            }

            return data;
        }

        public IEnumerable<JToken> FilterData(IEnumerable<JToken> data, List<IIndicatorColumn> indicatorColumns)
        {
            var result = data;

            foreach (var column in indicatorColumns)
            {
                if (column.Filtered)
                {
                    result = from jsonData in result
                             where jsonData[column.Code].Value<string>() == column.FilteredValue
                             select jsonData;
                }
            }

            return result;
        }

        public IEnumerable<JToken> FilterDataColumn(IEnumerable<JToken> data, List<string> columnToFilter)
        {
            var result = new JArray();

            foreach (var line in data)
            {
                foreach (var column in columnToFilter)
                {
                    var token = line.Children().FirstOrDefault(x => x.Path == $"{line.Path}.{column}");
                    token?.Remove();
                }

                result.Add(line);
            }

            return result;
        }

        public IEnumerable<JToken> TranspositionData(IEnumerable<JToken> data, string rowColumnName, string colColumnName, string dataColumnName)
        {
            if (string.IsNullOrEmpty(colColumnName) ||
                string.IsNullOrEmpty(dataColumnName))
            {
                return data;
            }

            var list = data.Select(token => new
            {
                rowSelector = string.IsNullOrEmpty(rowColumnName) ? string.Empty : token.Children().FirstOrDefault(x => x.Path == $"{token.Path}.{rowColumnName}").First().Value<string>(),
                colSelector = token.Children().FirstOrDefault(x => x.Path == $"{token.Path}.{colColumnName}").First().Value<string>(),
                dataSelector = token.Children().FirstOrDefault(x => x.Path == $"{token.Path}.{dataColumnName}").First().Value<decimal>(),
            });

            var pivot = list.ToPivotArray(
                item => item.colSelector,
                item => item.rowSelector,
                items => items.Any() ? items.Sum(item => item.dataSelector) : 0,
                rowColumnName);

            return JArray.Parse(JsonConvert.SerializeObject(pivot, new KeyValuePairConverter()));
        }

        public IEnumerable<JToken> Aggregation(List<JToken> data, List<IIndicatorColumn> indicatorColumns, List<string> columnsToAggregate)
        {
            return AggregateColumns(data, indicatorColumns, columnsToAggregate);
        }

        private IEnumerable<JToken> AggregateColumns(List<JToken> data, List<IIndicatorColumn> indicatorColumns,
            List<string> columnToAggregates)
        {
            var dataAggregates = new JArray();

            if (columnToAggregates.Any())
            {
                var column = columnToAggregates.First();

                var groupValues = from jsonData in data
                                  group jsonData by jsonData[column]
                                  into col
                                  select col.Key;

                foreach (var value in groupValues)
                {
                    if (value == null)
                    {
                        continue;
                    }

                    var lines = (from jsonData in data
                                 where jsonData[column].Value<string>() == value.Value<string>()
                                 select jsonData).ToList();

                    if (lines.Count > 1)
                    {
                        if (columnToAggregates.Count > 1)
                        {
                            var newColumnToAggregates = new List<string>();
                            foreach (var aggregate in columnToAggregates)
                            {
                                if (aggregate != column)
                                {
                                    newColumnToAggregates.Add(aggregate);
                                }
                            }

                            var lineToAdd = AggregateColumns(lines, indicatorColumns, newColumnToAggregates);

                            foreach (var line in lineToAdd)
                            {
                                dataAggregates.Add(line);
                            }
                        }
                        else
                        {
                            foreach (var sumColumn in MonitorComponent.GetColumnsToSum(indicatorColumns))
                            {
                                var total = lines.Sum(m => (decimal)m.SelectToken(sumColumn));
                                lines.First()[sumColumn] = total;
                            }

                            dataAggregates.Add(lines.First());
                        }
                    }
                    else
                    {
                        dataAggregates.Add(lines.First());
                    }
                }
            }
            else
            {
                var lines = (from jsonData in data
                             select jsonData).ToList();

                if (lines.Any())
                {
                    foreach (var sumColumn in MonitorComponent.GetColumnsToSum(indicatorColumns))
                    {
                        var total = lines.Sum(m => (decimal)m.SelectToken(sumColumn));
                        lines.First()[sumColumn] = total;
                    }
                    dataAggregates.Add(lines.First());
                }
            }

            return dataAggregates;
        }

        #endregion
    }
}
