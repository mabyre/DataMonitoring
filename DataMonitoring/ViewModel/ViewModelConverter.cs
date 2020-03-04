using System;
using DataMonitoring.Model;
using System.Collections.Generic;
using System.Linq;
using Sodevlog.Common.Http;

namespace DataMonitoring.ViewModel
{
    public static class ViewModelConverter
    {
        public static ClientAppSettings GetAppSettings(MonitorSettings settings)
        {
            var clientSettings = new ClientAppSettings
            {
                ApplicationName = settings.ApplicationName,
                ApplicationScope = settings.ApplicationScope,
                ApiServerUrl = settings.ApiServerUrl,
                ApiRetry = settings.ApiRetry,
                ApiTimeout = settings.ApiTimeout,
                DefaultLocale = settings.DefaultLocale,
                DefaultSkin = settings.DefaultSkin,
                WaitIntervalMonitor = settings.WaitIntervalMonitor,
                WaitIntervalQueryBackgroundTask = settings.WaitIntervalQueryBackgroundTask,
                AuthenticationSettings = new AuthenticationSettings
                {
                    AuthorityServerActif = settings.AuthoritySettings.AuthorityServerActif,
                    AutorityServer = settings.AuthoritySettings.AuthorityServerUrl,
                    ClientId = settings.AuthoritySettings.ClientId,
                    RedirectUrl = settings.AuthoritySettings.RedirectUrl,
                    ResponseType = settings.AuthoritySettings.ResponseType,
                    ForbiddenRoute = settings.AuthoritySettings.ForbiddenRoute,
                    StartupRoute = settings.AuthoritySettings.StartupRoute,
                    UnauthorizedRoute = settings.AuthoritySettings.UnauthorizedRoute,
                    PostLogoutRedirectUri = settings.AuthoritySettings.PostLogoutRedirectUri,
                    SilentRenew = settings.AuthoritySettings.SilentRenew,
                    StartCheckSession = settings.AuthoritySettings.StartCheckSession,
                    LogConsoleDebugActive = settings.AuthoritySettings.LogConsoleDebugActive,
                    LogConsoleWarningActive = settings.AuthoritySettings.LogConsoleWarningActive,
                    MaxIdTokenIatOffsetAllowedInSeconds = settings.AuthoritySettings.MaxIdTokenIatOffsetAllowedInSeconds,
                },
                Skins = settings.Skins.Select(GetSkinSetting).ToList()
            };

            foreach (var scope in settings.AuthoritySettings.Scopes)
            {
                clientSettings.AuthenticationSettings.Scope += scope + ' ';
            }

            return clientSettings;
        }
        
        public static SkinSetting GetSkinSetting(MonitorSkinSetting setting)
        {
            return new SkinSetting
            {
                Name = setting.Name,
                Label = setting.Label,
                Logo = setting.Logo,
            };
        }

        public static List<LanguageSettings> GetLanguageSettings(ApplicationSettings settings)
        {
            var languageSettings = new List<LanguageSettings>();

            foreach (var culture in settings.CultureSupported)
            {
                var language = new LanguageSettings
                {
                    Key = culture.Key,
                    Alt = culture.Alt,
                    Title = culture.Title,
                    Culture = culture.Culture
                };
                languageSettings.Add(language);
            }

            return languageSettings;
        }

        public static TimeZoneViewModel GetTimeZone(TimeZoneInfo timeZoneInfo)
        {
            return new TimeZoneViewModel
            {
                Id = timeZoneInfo.Id,
                Name = timeZoneInfo.StandardName,
                DisplayName = timeZoneInfo.DisplayName
            };
        }

        public static ConnectorViewModel GetConnector(Connector connector)
        {
            var connectorViewModel = new ConnectorViewModel
            {
                Id = connector.Id,
                Name = connector.Name,
                TimeZone = connector.TimeZone
            };

            if (connector is ApiConnector apiConnector)
            {
                connectorViewModel.ApiConnector = GetApiConnector(apiConnector);
            }

            if (connector is SqlServerConnector sqlServerConnector)
            {
                connectorViewModel.SqlServerConnector = GetSqlConnector(sqlServerConnector);
            }

            if ( connector is SqLiteConnector sqLiteConnector )
            {
                connectorViewModel.SqLiteConnector = GetSqLiteConnector( sqLiteConnector ); 
            }

            return connectorViewModel;
        }

        private static ApiConnectorViewModel GetApiConnector(ApiConnector connector)
        {
            return new ApiConnectorViewModel
            {
                AcceptHeader = connector.AcceptHeader,
                AccessTokenUrl = connector.AccessTokenUrl,
                AutorisationType = connector.AutorisationType,
                BaseUrl = connector.BaseUrl,
                ClientId = connector.ClientId,
                ClientSecret = connector.ClientSecret,
                GrantType = connector.GrantType,
                HttpMethod = connector.HttpMethod
            };
        }

        private static SqlServerConnectorViewModel GetSqlConnector( SqlServerConnector connector )
        {
            return new SqlServerConnectorViewModel
            {
                HostName = connector.HostName,
                DatabaseName = connector.DatabaseName,
                UseIntegratedSecurity = connector.UseIntegratedSecurity,
                UserName = connector.UserName,
                Password = connector.Password,
            };
        }

        private static SqLiteConnectorViewModel GetSqLiteConnector( SqLiteConnector connector )
        {
            return new SqLiteConnectorViewModel
            {
                HostName = connector.HostName,
                DatabaseName = connector.DatabaseName,
                UseIntegratedSecurity = connector.UseIntegratedSecurity,
                UserName = connector.UserName,
                Password = connector.Password,
            };
        }

        internal static IndicatorDefinitionViewModel GetIndicatorDefinition(IndicatorDefinition indicator)
        {
            var indicatorDefinitionViewModel = new IndicatorDefinitionViewModel
            {
                Id = indicator.Id,
                Title = indicator.Title,
                RefreshTime = indicator.RefreshTime,
                DelayForDelete = indicator.DelayForDelete,
                Type = indicator.Type,
                QueryConnectors = indicator.Queries?.Select(GetQueryConnectors).ToList(),
                TitleLocalizations = indicator.IndicatorLocalizations?.Select(GetLocalization).ToList(),
                TimeManagementId = indicator.TimeManagementId,
            };

            if (indicator is IndicatorCalculated indicatorCalculated)
            {
                indicatorDefinitionViewModel.IndicatorCalculated = GetIndicatorCalculated(indicatorCalculated);
            }

            return indicatorDefinitionViewModel;
        }

        private static TitleLocalizationViewModel GetLocalization(IndicatorLocalization localization)
        {
            return new TitleLocalizationViewModel
            {
                Title = localization.Title,
                LocalizationCode = localization.LocalizationCode,
            };
        }

        private static QueryConnectorViewModel GetQueryConnectors(IndicatorQuery indicatorQuery)
        {
            return new QueryConnectorViewModel
            {
                ConnectorId = indicatorQuery.ConnectorId,
                Query = indicatorQuery.Query,
            };
        }

        private static IndicatorCalculatedViewModel GetIndicatorCalculated(IndicatorCalculated indicatorCalculated)
        {
            return new IndicatorCalculatedViewModel
            {
                IndicatorOneId = indicatorCalculated.IndicatorDefinitionId1,
                IndicatorTwoId = indicatorCalculated.IndicatorDefinitionId2,
            };
        }

        public static WidgetViewModel GetWidget(Widget widget)
        {
            var widgetViewModel = new WidgetViewModel
            {
                Id = widget.Id,
                Title = widget.Title,
                TitleFontSize = widget.TitleFontSize,
                Type = widget.Type,
                RefreshTime = widget.RefreshTime,
                TitleColorName = widget.TitleColorName,
                TitleDisplayed = widget.TitleDisplayed,
                CurrentTimeManagementDisplayed = widget.CurrentTimeManagementDisplayed,
                LastRefreshTimeIndicatorDisplayed = widget.LastRefreshTimeIndicator,
                TimeManagementId = widget.TimeManagementId,
                TitleTranslate = widget.WidgetLocalizations.Select(GetWidgetTitleTranslation).ToList()
            };

            if (widget.Indicators != null && widget.Indicators.Any())
            {
                if (widget.Indicators.First() is IndicatorTableWidget)
                {
                    widgetViewModel.IndicatorTableWidgetList = widget.Indicators.Select(x => GetIndicatorTableWidget((IndicatorTableWidget)x)).ToList();
                }
                else if (widget.Indicators.First() is IndicatorBarWidget)
                {
                    widgetViewModel.IndicatorBarWidget = GetIndicatorBarWidget((IndicatorBarWidget)widget.Indicators.First());
                }
                else if (widget.Indicators.First() is IndicatorChartWidget)
                {
                    widgetViewModel.IndicatorChartWidget = GetIndicatorChartWidget((IndicatorChartWidget)widget.Indicators.First());
                }
                else if (widget.Indicators.First() is IndicatorGaugeWidget)
                {
                    widgetViewModel.IndicatorGaugeWidget = GetIndicatorGaugeWidget((IndicatorGaugeWidget)widget.Indicators.First());
                }
            }
            //else 
            //{
            //    switch ( widget.Type )
            //    {
            //        case WidgetType.UniqueTable:
            //            break;
            //        case WidgetType.MultiHorizontalTable:
            //            break;
            //        case WidgetType.MultiVerticalTable:
            //            break;
            //        case WidgetType.Chart:
            //            widgetViewModel.IndicatorChartWidget = new IndicatorChartWidgetViewModel();
            //            break;
            //        case WidgetType.Bar:
            //            break;
            //        case WidgetType.Gauge:
            //            break;
            //        default:
            //            break;
            //    }
            //}

            return widgetViewModel;
        }

        private static IndicatorGaugeWidgetViewModel GetIndicatorGaugeWidget(IndicatorGaugeWidget value)
        {
            return new IndicatorGaugeWidgetViewModel
            {
                Id = value.Id,
                TargetValue = value.TargetValue,
                TitleIndicatorDisplayed = value.TitleIndicatorDisplayed,
                TitleIndicatorColor = value.TitleIndicatorColor,
                IndicatorId = value.IndicatorDefinitionId,
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
        }

        private static IndicatorChartWidgetViewModel GetIndicatorChartWidget(IndicatorChartWidget value)
        {
            return new IndicatorChartWidgetViewModel
            {
                Id = value.Id,
                TitleIndicatorDisplayed = value.TitleIndicatorDisplayed,
                TitleIndicatorColor = value.TitleIndicatorColor,
                IndicatorId = value.IndicatorDefinitionId,
                TargetValue = value.TargetValue,
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
                TargetIndicatorChartWidgetList = value.TargetIndicatorChartWidgets?.Select(GetTargetIndicatorChartWidget).ToList()
                                                 ?? new List<TargetIndicatorChartWidgetViewModel>(),
            };
        }

        private static TargetIndicatorChartWidgetViewModel GetTargetIndicatorChartWidget(TargetIndicatorChartWidget value)
        {
            return new TargetIndicatorChartWidgetViewModel
            {
                Id = value.Id,
                StartDateUtc = value.StartDateUtc,
                StartTargetValue = value.StartTargetValue,
                EndDateUtc = value.EndDateUtc,
                EndTargetValue = value.EndTargetValue,
            };
        }

        private static IndicatorBarWidgetViewModel GetIndicatorBarWidget(IndicatorBarWidget value)
        {
            return new IndicatorBarWidgetViewModel
            {
                Id = value.Id,
                TitleIndicatorDisplayed = value.TitleIndicatorDisplayed,
                TitleIndicatorColor = value.TitleIndicatorColor,
                IndicatorId = value.IndicatorDefinitionId,
                TargetValue = value.TargetValue,
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

                IndicatorBarWidgetColumnList = value.IndicatorBarWidgetColumns?.Select(GetIndicatorBarWidgetColumn).ToList()
                                            ?? new List<IndicatorBarWidgetColumnViewModel>(),
                BarLabelWidgetList = value.BarLabelWidgets?.Select(GetBarLabelWidget).ToList()
                                  ?? new List<BarLabelWidgetViewModel>(),
            };
        }

        private static BarLabelWidgetViewModel GetBarLabelWidget(BarLabelWidget value)
        {
            return new BarLabelWidgetViewModel
            {
                Id = value.Id,
                Name = value.Name,
                Sequence = value.Sequence,
                LabelTextColor = value.LabelTextColor,
                UseLabelColorForBar = value.UseLabelColorForBar,

                BarLabelWidgetLocalizationList = value.BarLabelWidgetLocalizations?.Select(GetBarLabelWidgetLocalization).ToList()
                                              ?? new List<BarLabelWidgetLocalizationViewModel>(),
            };
        }

        private static BarLabelWidgetLocalizationViewModel GetBarLabelWidgetLocalization(BarLabelWidgetLocalization value)
        {
            return new BarLabelWidgetLocalizationViewModel
            {
                Id = value.Id,
                Title = value.Name,
                LocalizationCode = value.LocalizationCode,
            };
        }

        private static IndicatorBarWidgetColumnViewModel GetIndicatorBarWidgetColumn(IndicatorBarWidgetColumn value)
        {
            return new IndicatorBarWidgetColumnViewModel
            {
                Id = value.Id,
                Code = value.Code,
                FilteredValue = value.FilteredValue,
                Filtered = value.Filtered,
                IsNumericFormat = value.IsNumericFormat,
            };
        }

        private static IndicatorTableWidgetViewModel GetIndicatorTableWidget(IndicatorTableWidget indicatorWidget)
        {
            return new IndicatorTableWidgetViewModel
            {
                Id = indicatorWidget.Id,
                RowStyleWhenEqualValue = indicatorWidget.RowStyleWhenEqualValue,
                TitleIndicatorDisplayed = indicatorWidget.TitleIndicatorDisplayed,
                HeaderDisplayed = indicatorWidget.HeaderDisplayed,
                Sequence = indicatorWidget.Sequence,
                TitleIndicatorColor = indicatorWidget.TitleIndicatorColor,
                ColumnCode = indicatorWidget.ColumnCode,
                EqualsValue = indicatorWidget.EqualsValue,
                IndicatorId = indicatorWidget.IndicatorDefinitionId,
                TableWidgetColumnList = indicatorWidget.TableWidgetColumns.Select(GetTableWidgetColumn).ToList(),
                TargetValue = indicatorWidget.TargetValue,
            };
        }

        private static TableWidgetColumnViewModel GetTableWidgetColumn(TableWidgetColumn value)
        {
            var tableWidgetColumnViewModel = new TableWidgetColumnViewModel
            {
                Id = value.Id,
                Name = value.Name,
                NameDisplayed = value.NameDisplayed,
                Sequence = value.Sequence,
                Code = value.Code,
                ColumnStyle = value.ColumnStyle,
                Displayed = value.Displayed,
                TextBodyColor = value.TextBodyColor,
                TextHeaderColor = value.TextHeaderColor,
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

                ColumnNameLocalizations = value.TableWidgetColumnLocalizations?.Select(GetColumnLocalization).ToList() 
                                          ?? new List<TableWidgetColumnLocalizationViewModel>(), 
            };

            if (value is IndicatorTableWidgetColumn indicatorTableWidgetColumn)
            {
                tableWidgetColumnViewModel.Type = ColumnType.IndicatorData;
                tableWidgetColumnViewModel.Filtered = indicatorTableWidgetColumn.Filtered;
                tableWidgetColumnViewModel.FilteredValue = indicatorTableWidgetColumn.FilteredValue;
                tableWidgetColumnViewModel.IsNumericFormat = indicatorTableWidgetColumn.IsNumericFormat;
                tableWidgetColumnViewModel.TranspositionColumn = indicatorTableWidgetColumn.TranspositionColumn;
                tableWidgetColumnViewModel.TranspositionValue = indicatorTableWidgetColumn.TranspositionValue;
                tableWidgetColumnViewModel.TranspositionRow = indicatorTableWidgetColumn.TranspositionRow;
            }
            if (value is MaskTableWidgetColumn maskTableWidgetColumn)
            {
                tableWidgetColumnViewModel.Type = ColumnType.MaskData;
                tableWidgetColumnViewModel.DisplayModel = maskTableWidgetColumn.DisplayModel;
            }
            if (value is TranspositionColumnTableWidgetColumn)
            {
                tableWidgetColumnViewModel.Type = ColumnType.TranspositionColumnData;
            }
            if (value is TargetTableWidgetColumn)
            {
                tableWidgetColumnViewModel.Type = ColumnType.TargetData;
            }
            if (value is TitleIndicatorTableWidgetColumn)
            {
                tableWidgetColumnViewModel.Type = ColumnType.TitleIndicator;
            }
            if (value is CalculatedTableWidgetColumn calculatedTableWidgetColumn)
            {
                tableWidgetColumnViewModel.Type = ColumnType.CalculatedData;
                tableWidgetColumnViewModel.PartialValueColumn = calculatedTableWidgetColumn.PartialValueColumn;
                tableWidgetColumnViewModel.TotalValueColumn = calculatedTableWidgetColumn.TotalValueColumn;
            }

            return tableWidgetColumnViewModel;
        }

        private static TableWidgetColumnLocalizationViewModel GetColumnLocalization(TableWidgetColumnLocalization value)
        {
            return new TableWidgetColumnLocalizationViewModel
            {
                Id = value.Id,
                Title = value.Name,
                LocalizationCode = value.LocalizationCode,
            };
        }

        public static WidgetTitleTranslation GetWidgetTitleTranslation(WidgetLocalization widgetTitleTranslation)
        {
            return new WidgetTitleTranslation
            {
                Id = widgetTitleTranslation.Id,
                LocalizationCode = widgetTitleTranslation.LocalizationCode,
                Title = widgetTitleTranslation.Title
            };
        }

        public static TimeManagementViewModel GetTimeManagement(TimeManagement timeManagement)
        {
            TimeManagementViewModel timeManegementVM =  new TimeManagementViewModel()
            {
                Id = timeManagement.Id,
                Name = timeManagement.Name,
                SlipperyTime = timeManagement.SlipperyTime != null
                ? GetSlipperyTime( timeManagement.SlipperyTime )
                : null,
                TimeRanges = timeManagement.TimeRanges?.Select( GetTimeRange ).ToList(),
            };

            return timeManegementVM;
        }

        private static TimeRangeViewModel GetTimeRange(TimeRange value)
        {
            return new TimeRangeViewModel
            {
                Name = value.Name,
                StartTimeUtc = value.StartTimeUtc,
                EndTimeUtc = value.EndTimeUtc,
            };
        }

        private static SlipperyTimeViewModel GetSlipperyTime(SlipperyTime value)
        {
            return new SlipperyTimeViewModel
            {
                TimeBack = value.TimeBack,
                UnitOfTime = value.UnitOfTime,
            };
        }

        public static ColorViewModel GetColor(ColorHtml color)
        {
            return new ColorViewModel
            {
                Id = color.Id,
                Name = color.Name,
                TxtClassName = color.TxtClassName,
                BgClassName = color.BgClassName,
                HexColorCode = color.HexColorCode,
            };
        }

        public static StyleViewModel GetStyle(Style value)
        {
            return new StyleViewModel
            {
                Id = value.Id,
                Name = value.Name,
                Code = value.Code,
            };
        }

        public static MonitorViewModel GetMonitor(SharedDashboard sharedDashboard, Dashboard dashboard, List<ColorHtml> colors)
        {
            return new MonitorViewModel
            {
                Key = sharedDashboard.Key,
                CodeLangue = sharedDashboard.CodeLangue,
                TimeZone = sharedDashboard.TimeZone,
                IsTestMode = sharedDashboard.IsTestMode,
                Skin = sharedDashboard.Skin,
                Title = dashboard.TitleToDisplay,
                DisplayTitle = dashboard.TitleDisplayed,
                Message =  sharedDashboard.Message,
                ClassColorTitle = colors.Single(x => x.Name == dashboard.TitleColorName).TxtClassName,
                Widgets = dashboard.Widgets.Select(GetMonitorWidget).ToList(),
                Version = dashboard.Version,
            };
        }

        public static MonitorWidgetViewModel GetMonitorWidget(DashboardWidget widget)
        {
            return new MonitorWidgetViewModel
            {
                Id = widget.WidgetId,
                Column = widget.Column,
                Position = widget.Position
            };
        }

        public static DashboardViewModel GetDashboard(Dashboard value)
        {
            return new DashboardViewModel
            {
                Id = value.Id,
                Title = value.Title,
                CurrentTimeManagementDisplayed = value.CurrentTimeManagementDisplayed,
                TitleColorName = value.TitleColorName,
                TitleDisplayed = value.TitleDisplayed,
                DashboardLocalizations = value.DashboardLocalizations?.Select(GetDashboardLocalization).ToList() 
                            ?? new List<DashboardLocalizationViewModel>(),
                Widgets = value.Widgets?.Select(GetDashboardWidget).ToList() 
                            ?? new List<DashboardWidgetViewModel>(),
                SharedDashboards = value.SharedDashboards?.Select(GetSharedDashboard).ToList() 
                            ?? new List<SharedDashboardViewModel>(),
            };
        }

        public static DashboardLightViewModel GetDashboardLight(Dashboard value)
        {
            return new DashboardLightViewModel
            {
                Id = value.Id,
                Title = value.Title,
                CurrentTimeManagementDisplayed = value.CurrentTimeManagementDisplayed,
                TitleColorName = value.TitleColorName,
                TitleDisplayed = value.TitleDisplayed,
            };
        }

        private static SharedDashboardViewModel GetSharedDashboard(SharedDashboard value)
        {
            return new SharedDashboardViewModel
            {
                Id = value.Id,
                CodeLangue = value.CodeLangue,
                Key = value.Key,
                Skin = value.Skin,
                TimeZone = value.TimeZone,
                IsTestMode = value.IsTestMode,
                Message = value.Message,
            };
        }

        private static DashboardWidgetViewModel GetDashboardWidget(DashboardWidget value)
        {
            return new DashboardWidgetViewModel
            {
                WidgetId = value.WidgetId,
                Column = value.Column,
                Position = value.Position,
            };
        }

        private static DashboardLocalizationViewModel GetDashboardLocalization(DashboardLocalization value)
        {
            return new DashboardLocalizationViewModel
            {
                Id = value.Id,
                LocalizationCode = value.LocalizationCode,
                Title = value.Title,
            };
        }
    }
}
