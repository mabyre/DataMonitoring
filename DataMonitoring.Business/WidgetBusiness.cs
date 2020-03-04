using System;
using System.Linq;
using System.Threading.Tasks;
using DataMonitoring.DAL;
using DataMonitoring.Model;
using Microsoft.Extensions.Logging;
using Sodevlog.Tools;

namespace DataMonitoring.Business
{
    public class WidgetBusiness : UnitOfWork, IWidgetBusiness
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<WidgetBusiness>();

        public WidgetBusiness() : base()
        {
        }

        public WidgetBusiness(DataMonitoringDbContext dataMonitoringContext ) : base( dataMonitoringContext )
        {
        }

        public async Task<Widget> GetWidgetAsync(long id)
        {
            var widget = await WidgetRepository.GetAsync(id);

            foreach (var indicator in widget.Indicators)
            {
                if (indicator is IndicatorTableWidget indicatorTableWidget)
                {
                    indicatorTableWidget.TableWidgetColumns = Repository<TableWidgetColumn>().Find(x => x.IndicatorTableWidgetId == indicatorTableWidget.Id).ToList();
                    foreach (var tableWidgetColumn in indicatorTableWidget.TableWidgetColumns)
                    {
                        tableWidgetColumn.TableWidgetColumnLocalizations = Repository<TableWidgetColumnLocalization>().Find(x => x.TableWidgetColumnId == tableWidgetColumn.Id).ToList();
                    }
                }
                else if (indicator is IndicatorBarWidget indicatorBarWidget)
                {
                    indicatorBarWidget.IndicatorBarWidgetColumns = Repository<IndicatorBarWidgetColumn>().Find(x => x.IndicatorBarWidgetId == indicatorBarWidget.Id).ToList();
                    indicatorBarWidget.BarLabelWidgets = Repository<BarLabelWidget>().Find(x => x.IndicatorBarWidgetId == indicatorBarWidget.Id).ToList();
                    foreach (var barLabelWidget in indicatorBarWidget.BarLabelWidgets)
                    {
                        barLabelWidget.BarLabelWidgetLocalizations = Repository<BarLabelWidgetLocalization>().Find(x => x.BarLabelWidgetId == barLabelWidget.Id).ToList();
                    }
                }
                else if (indicator is IndicatorChartWidget indicatorChartWidget)
                {
                    indicatorChartWidget.TargetIndicatorChartWidgets = Repository<TargetIndicatorChartWidget>().Find(x => x.IndicatorChartWidgetId == indicatorChartWidget.Id).ToList();
                }
            }

            return widget;
        }

        public long CreateOrUpdateWidget(Widget widget)
        {
            if (widget.Id == 0)
            {
                Logger.LogInformation("Create new widget");

                WidgetRepository.Create(widget);
            }
            else
            {
                Logger.LogInformation($"Update widget id {widget.Id}");
                WidgetRepository.Update(widget);
            }

            Save();
            return widget.Id;
        }

        public void DeleteWidget(long id)
        {
            Logger.LogInformation($"Delete widget id {id}");

            var dashboardWidget = Repository<DashboardWidget>().Find(x => x.WidgetId == id).ToList();
            if (dashboardWidget.Any())
            {
                throw new InvalidOperationException($"Dashboard exist for widget id {id}");
            }

            WidgetRepository.Delete(id);
            Save();
        }

        public void DuplicateWidget(Widget widget, string copyMessage)
        {
            widget.Id = 0;
            widget.Title = widget.Title + $" - {copyMessage}";

            foreach (var widgetWidgetLocalization in widget.WidgetLocalizations)
            {
                widgetWidgetLocalization.Id = 0;
            }

            foreach (var widgetIndicator in widget.Indicators)
            {
                widgetIndicator.Id = 0;
                if (widgetIndicator is IndicatorTableWidget indicatorTableWidget)
                {
                    foreach (var tableWidgetColumn in indicatorTableWidget.TableWidgetColumns)
                    {
                        tableWidgetColumn.Id = 0;
                        foreach (var tableWidgetColumnLocalization in tableWidgetColumn.TableWidgetColumnLocalizations)
                        {
                            tableWidgetColumnLocalization.Id = 0;
                        }
                    }
                }
                else if (widgetIndicator is IndicatorBarWidget indicatorBarWidget)
                {
                    foreach (var indicatorBarWidgetColumn in indicatorBarWidget.IndicatorBarWidgetColumns)
                    {
                        indicatorBarWidgetColumn.Id = 0;
                    }

                    foreach (var barLabelWidget in indicatorBarWidget.BarLabelWidgets)
                    {
                        barLabelWidget.Id = 0;
                        foreach (var localization in barLabelWidget.BarLabelWidgetLocalizations)
                        {
                            localization.Id = 0;
                        }
                    }
                }
                else if (widgetIndicator is IndicatorChartWidget indicatorChartWidget)
                {
                    foreach (var targetIndicatorChartWidget in indicatorChartWidget.TargetIndicatorChartWidgets)
                    {
                        targetIndicatorChartWidget.Id = 0;
                    }
                }
            }

            WidgetRepository.Create(widget);
            Save();
        }

        public string GetIndicatorTitleList(long id)
        {
            var indicatorTitleListResult = string.Empty;
            var indicatorsWidgetList = Repository<IndicatorWidget>().Find(x => x.WidgetId == id).ToList();
            foreach (var indicatorWidget in indicatorsWidgetList)
            {
                var indicator = Repository<IndicatorDefinition>().Find(x => x.Id == indicatorWidget.IndicatorDefinitionId).FirstOrDefault();
                if (indicator != null)
                {
                    indicatorTitleListResult += $"{indicator.Title} ; ";
                }
            }

            return indicatorTitleListResult;
        }
    }
}
