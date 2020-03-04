using DataMonitoring.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataMonitoring.ViewModel
{
    public static class BusinessConverter
    {
        public static Connector GetConnector(ConnectorViewModel connectorViewModel)
        {
            if (connectorViewModel.ApiConnector != null)
            {
                return GetConnector(connectorViewModel,new ApiConnector());
            }

            if (connectorViewModel.SqlServerConnector != null)
            {
                return GetConnector(connectorViewModel,new SqlServerConnector());
            }

            if ( connectorViewModel.SqLiteConnector != null )
            {
                return GetConnector( connectorViewModel, new SqLiteConnector() );
            }

            throw new InvalidDataException();
        }

        public static Connector GetConnector(ConnectorViewModel connectorViewModel, Connector connector)
        {
            if (connectorViewModel.ApiConnector != null)
            {
                return GetApiConnector(connectorViewModel,(ApiConnector)connector);
            }

            if (connectorViewModel.SqlServerConnector != null)
            {
                return GetSqlServerConnector(connectorViewModel,(SqlServerConnector)connector);
            }

            if ( connectorViewModel.SqLiteConnector != null )
            {
                return GetSqLiteConnector( connectorViewModel, (SqLiteConnector)connector );
            }

            throw new InvalidDataException();
        }

        internal static Connector GetConnector(ConnectorViewModel value, object connector)
        {
            throw new NotImplementedException();
        }

        private static ApiConnector GetApiConnector(ConnectorViewModel connectorViewModel, ApiConnector connector)
        {
            if (connectorViewModel == null) { throw new InvalidDataException(); }

            connector.Name = connectorViewModel.Name;
            connector.TimeZone = connectorViewModel.TimeZone;
            connector.AccessTokenUrl = connectorViewModel.ApiConnector.AccessTokenUrl;
            connector.AcceptHeader = connectorViewModel.ApiConnector.AcceptHeader;
            connector.AutorisationType = connectorViewModel.ApiConnector.AutorisationType;
            connector.BaseUrl = connectorViewModel.ApiConnector.BaseUrl;
            connector.ClientId = connectorViewModel.ApiConnector.ClientId;
            connector.ClientSecret = connectorViewModel.ApiConnector.ClientSecret;
            connector.GrantType = connectorViewModel.ApiConnector.GrantType;
            connector.HttpMethod = connectorViewModel.ApiConnector.HttpMethod;

            return connector;
        }

        private static SqlServerConnector GetSqlServerConnector(ConnectorViewModel connectorViewModel, SqlServerConnector connector)
        {
            if (connectorViewModel.SqlServerConnector == null) { throw new InvalidDataException(); }

            connector.Name = connectorViewModel.Name;
            connector.TimeZone = connectorViewModel.TimeZone;
            connector.HostName = connectorViewModel.SqlServerConnector.HostName;
            connector.DatabaseName = connectorViewModel.SqlServerConnector.DatabaseName;
            connector.UseIntegratedSecurity = connectorViewModel.SqlServerConnector.UseIntegratedSecurity;
            connector.UserName = connectorViewModel.SqlServerConnector.UserName;
            connector.Password = connectorViewModel.SqlServerConnector.Password;

            return connector;
        }

        private static SqLiteConnector GetSqLiteConnector( ConnectorViewModel connectorViewModel, SqLiteConnector connector )
        {
            if ( connectorViewModel.SqLiteConnector == null ) { throw new InvalidDataException(); }

            connector.Name = connectorViewModel.Name;
            connector.TimeZone = connectorViewModel.TimeZone;
            connector.HostName = connectorViewModel.SqLiteConnector.HostName;
            connector.DatabaseName = connectorViewModel.SqLiteConnector.DatabaseName;
            connector.UseIntegratedSecurity = connectorViewModel.SqLiteConnector.UseIntegratedSecurity;
            connector.UserName = connectorViewModel.SqLiteConnector.UserName;
            connector.Password = connectorViewModel.SqLiteConnector.Password;

            return connector;
        }

        public static IndicatorDefinition GetIndicatorDefinition(IndicatorDefinitionViewModel value)
        {
            if (value.IndicatorCalculated != null)
            {
                return GetIndicatorCalculated(value);
            }

            return new IndicatorDefinition
            {
                Id = value.Id,
                Title = value.Title,
                RefreshTime = value.RefreshTime,
                DelayForDelete = value.DelayForDelete,
                Type = value.Type,
                Queries = value.QueryConnectors?.Select(GetIndicatorQuery).ToList()
                    ?? new List<IndicatorQuery>(),
                IndicatorLocalizations = value.TitleLocalizations?.Select(GetIndicatorLocalization).ToList()
                    ?? new List<IndicatorLocalization>(),
                TimeManagementId = value.TimeManagementId,
            };
        }



        private static IndicatorLocalization GetIndicatorLocalization(TitleLocalizationViewModel localization)
        {
            return new IndicatorLocalization
            {
                LocalizationCode = localization.LocalizationCode,
                Title = localization.Title,
            };
        }

        public static IndicatorQuery GetIndicatorQuery(QueryConnectorViewModel queryConnector)
        {
            return new IndicatorQuery
            {
                ConnectorId = queryConnector.ConnectorId,
                Query = queryConnector.Query,
            };
        }

        private static IndicatorDefinition GetIndicatorCalculated(IndicatorDefinitionViewModel value)
        {
            return new IndicatorCalculated
            {
                Id = value.Id,
                Title = value.Title,
                IndicatorLocalizations = value.TitleLocalizations?.Select(GetIndicatorLocalization).ToList()
                    ?? new List<IndicatorLocalization>(),
                RefreshTime = value.RefreshTime,
                Type = value.Type,
                IndicatorDefinitionId1 = value.IndicatorCalculated.IndicatorOneId,
                IndicatorDefinitionId2 = value.IndicatorCalculated.IndicatorTwoId,
            };
        }

        public static Widget GetWidget(WidgetViewModel widgetViewModel)
        {
            var widget = new Widget
            {
                Id = widgetViewModel.Id,
                Title = widgetViewModel.Title,
                TitleFontSize = widgetViewModel.TitleFontSize,
                TitleColorName = widgetViewModel.TitleColorName,
                Type = widgetViewModel.Type,
                RefreshTime = widgetViewModel.RefreshTime,
                TitleDisplayed = widgetViewModel.TitleDisplayed,
                CurrentTimeManagementDisplayed = widgetViewModel.CurrentTimeManagementDisplayed,
                LastRefreshTimeIndicator = widgetViewModel.LastRefreshTimeIndicatorDisplayed,
                TimeManagementId = widgetViewModel.TimeManagementId,
                WidgetLocalizations = widgetViewModel.TitleTranslate.Select(x => GetWidgetLocalization(x, widgetViewModel.Id)).ToList(),
            };

            if (IsTypeTable(widgetViewModel.Type) &&
                widgetViewModel.IndicatorTableWidgetList != null && widgetViewModel.IndicatorTableWidgetList.Any())
            {
                widget.Indicators = widgetViewModel.IndicatorTableWidgetList.Select(GetIndicatorTableWidget).ToList();
            }

            if (widgetViewModel.Type == WidgetType.Bar && widgetViewModel.IndicatorBarWidget != null)
            {
                var indicatorBarWidget = GetIndicatorBarWidget(widgetViewModel.IndicatorBarWidget);
                widget.Indicators = new List<IndicatorWidget> { indicatorBarWidget };
            }

            if (widgetViewModel.Type == WidgetType.Chart && widgetViewModel.IndicatorChartWidget != null)
            {
                var indicatorChartWidget = GetIndicatorChartWidget(widgetViewModel.IndicatorChartWidget);
                widget.Indicators = new List<IndicatorWidget> { indicatorChartWidget };
            }

            if (widgetViewModel.Type == WidgetType.Gauge && widgetViewModel.IndicatorGaugeWidget != null)
            {
                var indicatorGaugeWidget = GetIndicatorGaugeWidget(widgetViewModel.IndicatorGaugeWidget);
                widget.Indicators = new List<IndicatorWidget> { indicatorGaugeWidget };
            }

            return widget;
        }

        private static IndicatorGaugeWidget GetIndicatorGaugeWidget(IndicatorGaugeWidgetViewModel value)
        {
            IndicatorGaugeWidget gaugeWidget = new IndicatorGaugeWidget
            {
                Id = value.Id,
                TargetValue = value.TargetValue,
                TitleIndicatorDisplayed = value.TitleIndicatorDisplayed,
                TitleIndicatorColor = value.TitleIndicatorColor,
                IndicatorDefinitionId = value.IndicatorId,
                TargetDisplayed = value.TargetDisplayed,
                Group1 = value.Group1,
                Group2 = value.Group2,
                Group3 = value.Group3,
                Group4 = value.Group4,
                Group5 = value.Group5,
                GaugeTargetColor = value.GaugeTargetColor,
                GaugeRange1Color = value.GaugeRange1Color,
                GaugeRange1MinValue = value.GaugeRange1MinValue,
                GaugeRange1MaxValue = value.GaugeRange1MaxValue,
                GaugeRange2Displayed = value.GaugeRange2Displayed,
                GaugeRange2Color = value.GaugeRange2Color,
                GaugeRange2MinValue = value.GaugeRange2MinValue,
                GaugeRange2MaxValue = value.GaugeRange2MaxValue,
                GaugeRange3Displayed = value.GaugeRange3Displayed,
                GaugeRange3Color = value.GaugeRange3Color,
                GaugeRange3MinValue = value.GaugeRange3MinValue,
                GaugeRange3MaxValue = value.GaugeRange3MaxValue,
            };

            return gaugeWidget;
        }

        private static IndicatorChartWidget GetIndicatorChartWidget(IndicatorChartWidgetViewModel value)
        {
            return new IndicatorChartWidget
            {
                Id = value.Id,
                TargetValue = value.TargetValue,
                TitleIndicatorDisplayed = value.TitleIndicatorDisplayed,
                TitleIndicatorColor = value.TitleIndicatorColor,
                IndicatorDefinitionId = value.IndicatorId,
                AxeFontSize = value.AxeFontSize,
                AxeXColor = value.AxeXColor,
                AxeXDisplayed = value.AxeXDisplayed,
                AxeYColor = value.AxeYColor,
                AxeYDataDisplayed = value.AxeYDataDisplayed,
                AxeYDisplayed = value.AxeYDisplayed,
                ChartAverageColor = value.ChartAverageColor,
                ChartAverageDisplayed = value.ChartAverageDisplayed,
                ChartDataColor = value.ChartDataColor,
                ChartDataFill = value.ChartDataFill,
                ChartTargetColor = value.ChartTargetColor,
                ChartTargetDisplayed = value.ChartTargetDisplayed,
                DecimalMask = value.DecimalMask,
                Group1 = value.Group1,
                Group2 = value.Group2,
                Group3 = value.Group3,
                Group4 = value.Group4,
                Group5 = value.Group5,
                AxeYIsAutoAdjustableAccordingMinValue = value.AxeYIsAutoAdjustableAccordingMinValue,
                AxeYOffsetFromMinValue = value.AxeYOffsetFromMinValue,
                TargetIndicatorChartWidgets = value.TargetIndicatorChartWidgetList?.Select(GetTargetIndicatorCharWidget).ToList()
                                              ?? new List<TargetIndicatorChartWidget>(),
            };
        }

        private static TargetIndicatorChartWidget GetTargetIndicatorCharWidget(TargetIndicatorChartWidgetViewModel value)
        {
            return new TargetIndicatorChartWidget
            {
                Id = value.Id,
                StartDateUtc = value.StartDateUtc,
                StartTargetValue = value.StartTargetValue,
                EndDateUtc = value.EndDateUtc,
                EndTargetValue = value.EndTargetValue,
            };
        }

        private static IndicatorBarWidget GetIndicatorBarWidget(IndicatorBarWidgetViewModel value)
        {
            return new IndicatorBarWidget
            {
                Id = value.Id,
                TargetValue = value.TargetValue,
                TitleIndicatorDisplayed = value.TitleIndicatorDisplayed,
                TitleIndicatorColor = value.TitleIndicatorColor,
                IndicatorDefinitionId = value.IndicatorId,
                AddBarStackedToTarget = value.AddBarStackedToTarget,
                AddTargetBar = value.AddTargetBar,
                BarColor = value.BarColor,
                BarColorStackedToTarget = value.BarColorStackedToTarget,
                DataColumnCode = value.DataColumnCode,
                DataLabelInBarColor = value.DataLabelInBarColor,
                DecimalMask = value.DecimalMask,
                DisplayAxeX = value.DisplayAxeX,
                DisplayAxeY = value.DisplayAxeY,
                DisplayDataAxeY = value.DisplayDataAxeY,
                DisplayDataLabelInBar = value.DisplayDataLabelInBar,
                DisplayGridLinesAxeY = value.DisplayGridLinesAxeY,
                FontSizeDataLabel = value.FontSizeDataLabel,
                LabelColorText = value.LabelColorText,
                LabelColumnCode = value.LabelColumnCode,
                LabelFontSize = value.LabelFontSize,
                SetSumDataToTarget = value.SetSumDataToTarget,
                TargetBarColor = value.TargetBarColor,
                TextDataAxeYColor = value.TextDataAxeYColor,

                IndicatorBarWidgetColumns = value.IndicatorBarWidgetColumnList?.Select(GetIndicatorBarWidgetColumn).ToList()
                                            ?? new List<IndicatorBarWidgetColumn>(),
                BarLabelWidgets = value.BarLabelWidgetList?.Select(GetBarLabelWidget).ToList()
                                  ?? new List<BarLabelWidget>(),
            };
        }

        private static BarLabelWidget GetBarLabelWidget(BarLabelWidgetViewModel value)
        {
            return new BarLabelWidget
            {
                Id = value.Id,
                Name = value.Name,
                Sequence = value.Sequence,
                LabelTextColor = value.LabelTextColor,
                UseLabelColorForBar = value.UseLabelColorForBar,

                BarLabelWidgetLocalizations = value.BarLabelWidgetLocalizationList?.Select(GetBarLabelWidgetLocalization).ToList()
                                              ?? new List<BarLabelWidgetLocalization>(),
            };
        }

        private static BarLabelWidgetLocalization GetBarLabelWidgetLocalization(BarLabelWidgetLocalizationViewModel value)
        {
            return new BarLabelWidgetLocalization
            {
                Id = value.Id,
                Name = value.Title,
                LocalizationCode = value.LocalizationCode,
            };
        }

        private static IndicatorBarWidgetColumn GetIndicatorBarWidgetColumn(IndicatorBarWidgetColumnViewModel value)
        {
            return new IndicatorBarWidgetColumn
            {
                Id = value.Id,
                Code = value.Code,
                FilteredValue = value.FilteredValue,
                Filtered = value.Filtered,
                IsNumericFormat = value.IsNumericFormat,
            };
        }

        private static bool IsTypeTable(WidgetType type)
        {
            return type == WidgetType.Table || type == WidgetType.MultiVerticalTable || type == WidgetType.MultiHorizontalTable;
        }

        private static IndicatorWidget GetIndicatorTableWidget(IndicatorTableWidgetViewModel indicatorTableWidgetViewModel)
        {
            return new IndicatorTableWidget
            {
                Id = indicatorTableWidgetViewModel.Id,
                IndicatorDefinitionId = indicatorTableWidgetViewModel.IndicatorId,
                ColumnCode = indicatorTableWidgetViewModel.ColumnCode,
                EqualsValue = indicatorTableWidgetViewModel.EqualsValue,
                HeaderDisplayed = indicatorTableWidgetViewModel.HeaderDisplayed,
                RowStyleWhenEqualValue = indicatorTableWidgetViewModel.RowStyleWhenEqualValue,
                Sequence = indicatorTableWidgetViewModel.Sequence,
                TitleIndicatorColor = indicatorTableWidgetViewModel.TitleIndicatorColor,
                TitleIndicatorDisplayed = indicatorTableWidgetViewModel.TitleIndicatorDisplayed,
                TableWidgetColumns = indicatorTableWidgetViewModel.TableWidgetColumnList.Select(GetTableWidgetColumn).ToList(),
                TargetValue = indicatorTableWidgetViewModel.TargetValue,
            };
        }

        private static TableWidgetColumn GetTableWidgetColumn(TableWidgetColumnViewModel value)
        {
            var tableWidgetColumn = new TableWidgetColumn
            {
                Id = value.Id,
                Name = value.Name,
                NameDisplayed = value.NameDisplayed,
                Code = value.Code,
                ColumnStyle = value.ColumnStyle,
                Displayed = value.Displayed,
                TextBodyColor = value.TextBodyColor,
                TextHeaderColor = value.TextHeaderColor,
                Sequence = value.Sequence,
                DecimalMask = value.DecimalMask,
                BoldHeader = value.BoldHeader,
                BoldBody = value.BoldBody,
                AlignStyle = value.AlignStyle,

                CellStyleWhenLowerValue = value.CellStyleWhenLowerValue,
                LowerValue = value.LowerValue,
                LowerColumnCode = value.LowerColumnCode,

                CellStyleWhenHigherValue = value.CellStyleWhenHigherValue,
                HigherValue = value.HigherValue,
                HigherColumnCode = value.HigherColumnCode,

                CellStyleWhenEqualValue1 = value.CellStyleWhenEqualValue1,
                EqualsValue1 = value.EqualsValue1,
                EqualsColumnCode1 = value.EqualsColumnCode1,

                CellStyleWhenEqualValue2 = value.CellStyleWhenEqualValue2,
                EqualsValue2 = value.EqualsValue2,
                EqualsColumnCode2 = value.EqualsColumnCode2,

                CellStyleWhenEqualValue3 = value.CellStyleWhenEqualValue3,
                EqualsValue3 = value.EqualsValue3,
                EqualsColumnCode3 = value.EqualsColumnCode3,

                TableWidgetColumnLocalizations = value.ColumnNameLocalizations?.Select(GetColumnLocalization).ToList()
                        ?? new List<TableWidgetColumnLocalization>(),
            };

            switch (value.Type)
            {
                case ColumnType.IndicatorData:
                    return new IndicatorTableWidgetColumn(tableWidgetColumn)
                    {
                        Filtered = value.Filtered,
                        FilteredValue = value.FilteredValue,
                        IsNumericFormat = value.IsNumericFormat,
                        TranspositionColumn = value.TranspositionColumn,
                        TranspositionValue = value.TranspositionValue,
                        TranspositionRow = value.TranspositionRow
                    };
                case ColumnType.CalculatedData:
                    return new CalculatedTableWidgetColumn(tableWidgetColumn)
                    {
                        PartialValueColumn = value.PartialValueColumn,
                        TotalValueColumn = value.TotalValueColumn,
                    };
                case ColumnType.TargetData:
                    return new TargetTableWidgetColumn(tableWidgetColumn);
                case ColumnType.TranspositionColumnData:
                    return new TranspositionColumnTableWidgetColumn(tableWidgetColumn);
                case ColumnType.TitleIndicator:
                    return new TitleIndicatorTableWidgetColumn(tableWidgetColumn);
                case ColumnType.MaskData:
                    return new MaskTableWidgetColumn(tableWidgetColumn)
                    {
                        DisplayModel = value.DisplayModel,
                    };
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static TableWidgetColumnLocalization GetColumnLocalization(TableWidgetColumnLocalizationViewModel value)
        {
            return new TableWidgetColumnLocalization
            {
                Id = value.Id,
                Name = value.Title,
                LocalizationCode = value.LocalizationCode,
            };
        }

        public static WidgetLocalization GetWidgetLocalization(WidgetTitleTranslation widgetTitleTranslation, long widgetId)
        {
            return new WidgetLocalization
            {
                Id = widgetTitleTranslation.Id,
                LocalizationCode = widgetTitleTranslation.LocalizationCode,
                Title = widgetTitleTranslation.Title,
                WidgetId = widgetId
            };
        }

        public static TimeManagement GetTimeManagement(TimeManagementViewModel value)
        {
            TimeManagement timeManagement = new TimeManagement
            {
                Id = value.Id,
                Name = value.Name,
                SlipperyTime = value.SlipperyTime != null
                    ? GetSlipperyTime( value.SlipperyTime )
                    : null,
                TimeRanges = value.TimeRanges?.Select( GetTimeRange ).ToList()
                    ?? new List<TimeRange>(),
            };

            return timeManagement;
        }

        private static SlipperyTime GetSlipperyTime(SlipperyTimeViewModel value)
        {
            return new SlipperyTime
            {
                TimeBack = value.TimeBack,
                UnitOfTime = value.UnitOfTime,
            };
        }

        private static TimeRange GetTimeRange(TimeRangeViewModel value)
        {
            return new TimeRange
            {
                Name = value.Name,
                StartTimeUtc = value.StartTimeUtc,
                EndTimeUtc = value.EndTimeUtc,
            };
        }

        public static Style GetStyle(StyleViewModel value)
        {
            return new Style
            {
                Id = value.Id,
                Name = value.Name,
                Code = value.Code,
            };
        }

        public static Dashboard GetDashboard(DashboardViewModel value)
        {
            var dashboard = new Dashboard
            {
                DashboardLocalizations = new List<DashboardLocalization>(),
                Widgets = new List<DashboardWidget>(),
                SharedDashboards = new List<SharedDashboard>()
            };

            return GetDashboard(value, dashboard);
        }

        public static Dashboard GetDashboard(DashboardViewModel value, Dashboard dashboard)
        {
            dashboard.CurrentTimeManagementDisplayed = value.CurrentTimeManagementDisplayed;
            dashboard.Title = value.Title;
            dashboard.TitleColorName = value.TitleColorName;
            dashboard.TitleDisplayed = value.TitleDisplayed;
            dashboard.DashboardLocalizations = GetDashboardLocalization(value.DashboardLocalizations, dashboard.DashboardLocalizations);
            dashboard.Widgets = GetDashboardWidget(value.Widgets);
            dashboard.SharedDashboards = GetSharedDashboard(value.SharedDashboards, dashboard.SharedDashboards);
            return dashboard;
        }

        public static Dashboard GetDashboardLight(DashboardLightViewModel value, Dashboard dashboard)
        {
            dashboard.CurrentTimeManagementDisplayed = value.CurrentTimeManagementDisplayed;
            dashboard.Title = value.Title;
            dashboard.TitleColorName = value.TitleColorName;
            dashboard.TitleDisplayed = value.TitleDisplayed;
            dashboard.DashboardLocalizations = GetDashboardLocalization(value.DashboardLocalizations, dashboard.DashboardLocalizations);
            return dashboard;
        }

        private static ICollection<DashboardLocalization> GetDashboardLocalization(List<DashboardLocalizationViewModel> values, ICollection<DashboardLocalization> dashboardLocalizations)
        {
            var list = new List<DashboardLocalization>();

            foreach (var localization in values)
            {
                var dashboardLocalization = dashboardLocalizations.FirstOrDefault(x => x.Id == localization.Id);

                list.Add(dashboardLocalization != null
                        ? GetDashboardLocalization(localization, dashboardLocalization)
                        : GetDashboardLocalization(localization)
                );
            }

            return list;
        }

        private static DashboardLocalization GetDashboardLocalization(DashboardLocalizationViewModel value)
        {
            return GetDashboardLocalization(value, new DashboardLocalization());
        }

        private static DashboardLocalization GetDashboardLocalization(DashboardLocalizationViewModel value, DashboardLocalization dashboardLocalization)
        {
            dashboardLocalization.LocalizationCode = value.LocalizationCode;
            dashboardLocalization.Title = value.Title;

            return dashboardLocalization;
        }

        private static ICollection<DashboardWidget> GetDashboardWidget(List<DashboardWidgetViewModel> values)
        {
            var list = new List<DashboardWidget>();

            foreach (var widget in values)
            {
                list.Add(GetDashboardWidget(widget));
            }

            return list;
        }

        private static DashboardWidget GetDashboardWidget(DashboardWidgetViewModel value)
        {
            return GetDashboardWidget(value, new DashboardWidget());
        }
        
        private static DashboardWidget GetDashboardWidget(DashboardWidgetViewModel value, DashboardWidget dashboardWidget)
        {
            dashboardWidget.WidgetId = value.WidgetId;
            dashboardWidget.Column = value.Column;
            dashboardWidget.Position = value.Position;

            return dashboardWidget;
        }
        
        private static ICollection<SharedDashboard> GetSharedDashboard(List<SharedDashboardViewModel> values, ICollection<SharedDashboard> sharedDashboards)
        {
            var list = new List<SharedDashboard>();

            if (values == null)
            {
                return list;
            }

            foreach (var share in values)
            {
                var sharedDashboard = sharedDashboards.FirstOrDefault(x => x.Id == share.Id);

                list.Add(sharedDashboard != null
                    ? GetSharedDashboard(share, sharedDashboard)
                    : GetSharedDashboard(share)
                );
            }

            return list;
        }

        private static SharedDashboard GetSharedDashboard(SharedDashboardViewModel value)
        {
            var sharedDashboard = GetSharedDashboard(value, new SharedDashboard());
            sharedDashboard.Key = value.Key;

            return sharedDashboard;
        }
        
        private static SharedDashboard GetSharedDashboard(SharedDashboardViewModel value, SharedDashboard sharedDashboard)
        {
            sharedDashboard.IsTestMode = value.IsTestMode;
            sharedDashboard.TimeZone = value.TimeZone;
            sharedDashboard.CodeLangue = value.CodeLangue;
            sharedDashboard.Skin = value.Skin;
            sharedDashboard.Message = value.Message;

            return sharedDashboard;
        }
        
        public static ColorHtml GetColor(ColorViewModel value)
        {
            return new ColorHtml
            {
                Id = value.Id,
                Name = value.Name,
                BgClassName = value.BgClassName,
                TxtClassName = value.TxtClassName,
                HexColorCode = value.HexColorCode,
            };
        }
    }
}
