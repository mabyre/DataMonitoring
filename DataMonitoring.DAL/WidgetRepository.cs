using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMonitoring.Model;
using Microsoft.EntityFrameworkCore;

namespace DataMonitoring.DAL
{
    public class WidgetRepository : Repository<Widget>, IWidgetRepository
    {
        public WidgetRepository(DbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Widget>> GetAllAsync()
        {
            var dbQuery = Context.Set<Widget>()
                .Include(x => x.WidgetLocalizations);

            return await dbQuery.ToListAsync();
        }

        public override IEnumerable<Widget> GetAll()
        {
            var dbQuery = Context.Set<Widget>()
                .Include(x => x.WidgetLocalizations);

            return dbQuery.ToList();
        }

        public override async Task<Widget> GetAsync(long id)
        {
            IQueryable<Widget> dbQuery = Context.Set<Widget>()
                .Include(x => x.WidgetLocalizations)
                .Include(x => x.Indicators);

            return await dbQuery.SingleOrDefaultAsync(x => x.Id == id);
        }

        public override Widget Get(long id)
        {
            IQueryable<Widget> dbQuery = Context.Set<Widget>()
                .Include(x => x.WidgetLocalizations)
                .Include(x => x.Indicators);

            return dbQuery.SingleOrDefault(x => x.Id == id);
        }

        public override void Delete(Widget entity)
        {
            if (entity.WidgetLocalizations != null)
            {
                foreach (var widgetWidgetLocalization in entity.WidgetLocalizations)
                {
                    Context.Remove<WidgetLocalization>(widgetWidgetLocalization);
                }
            }

            Context.Remove(entity);
        }

        public override void Delete(long id)
        {
            var widget = Get(id);
            if (widget.WidgetLocalizations != null)
            {
                foreach (var widgetWidgetLocalization in widget.WidgetLocalizations)
                {
                    Context.Remove(widgetWidgetLocalization);
                }
            }

            if (widget.Indicators != null)
            {
                RemoveIndicatorWidget(widget.Indicators);
            }

            Context.Remove(widget);
        }

        public override void DeleteRange(IEnumerable<Widget> entities)
        {
            foreach (var widget in entities)
            {
                Delete(widget);
            }
        }

        public override void Update(Widget entity)
        {
            // Localization :
            var widgetLocalizationsToDelete = Context.Set<WidgetLocalization>()
                .Where(x => x.WidgetId == entity.Id &&
                            !entity.WidgetLocalizations.Select(w => w.Id).Contains(x.Id))
                .ToList();

            foreach (var widgetLocalization in widgetLocalizationsToDelete)
            {
                Context.Remove(widgetLocalization);
            }

            foreach (var widgetLocalization in entity.WidgetLocalizations)
            {
                if (widgetLocalization.Id != 0)
                {
                    Context.Entry(widgetLocalization).State = EntityState.Modified;
                }
                else
                {
                    Context.Add(widgetLocalization);
                }
            }

            // IndicatorWidget :
            if (entity.Indicators != null && entity.Indicators.Any())
            {
                var indicatorWidgetsToDelete = Context.Set<IndicatorWidget>()
                    .Where(x => x.WidgetId == entity.Id && 
                                !entity.Indicators.Select(i => i.Id).Contains(x.Id))
                    .ToList();
                RemoveIndicatorWidget(indicatorWidgetsToDelete);

                foreach (var indicator in entity.Indicators)
                {
                    if (indicator is IndicatorTableWidget indicatorTableWidget)
                    {
                        UpdateIndicatorTableWidget(indicatorTableWidget);
                    }

                    if (indicator is IndicatorBarWidget indicatorBarWidget)
                    {
                        UpdateIndicatorBarWidget(indicatorBarWidget);
                    }

                    if (indicator is IndicatorChartWidget indicatorChartWidget)
                    {
                        UpdateIndicatorChartWidget(indicatorChartWidget);
                    }

                    if (indicator.Id != 0)
                    {
                        Context.Entry(indicator).State = EntityState.Modified;
                    }
                    else
                    {
                        Context.Add(indicator);
                    }
                }
            }
            else
            {
                var indicatorWidgetsToDelete = Context.Set<IndicatorWidget>()
                    .Where(x => x.WidgetId == entity.Id)
                    .ToList();
                RemoveIndicatorWidget(indicatorWidgetsToDelete);
            }

            Context.Entry(entity).State = EntityState.Modified;
        }

        private void UpdateIndicatorChartWidget(IndicatorChartWidget indicatorChartWidget)
        {
            var targetIndicatorsListToDelete = Context.Set<TargetIndicatorChartWidget>()
                .Where(x => x.IndicatorChartWidgetId == indicatorChartWidget.Id &&
                            !indicatorChartWidget.TargetIndicatorChartWidgets.Select(w => w.Id).Contains(x.Id))
                .ToList();
            foreach (var targetIndicatorChartWidgetToDelete in targetIndicatorsListToDelete)
            {
                Context.Remove(targetIndicatorChartWidgetToDelete);
            }

            foreach (var targetIndicatorChartWidget in indicatorChartWidget.TargetIndicatorChartWidgets)
            {
                if (targetIndicatorChartWidget.Id != 0)
                {
                    Context.Entry(targetIndicatorChartWidget).State = EntityState.Modified;
                }
                else
                {
                    Context.Add(targetIndicatorChartWidget);
                }
            }
        }

        private void UpdateIndicatorBarWidget(IndicatorBarWidget indicatorBarWidget)
        {
            // IndicatorBarWidgetColumn :
            if (indicatorBarWidget.IndicatorBarWidgetColumns != null)
            {
                var indicatorBarWidgetColumnsToDelete = Context.Set<IndicatorBarWidgetColumn>()
                    .Where(x => x.IndicatorBarWidgetId == indicatorBarWidget.Id &&
                                !indicatorBarWidget.IndicatorBarWidgetColumns.Select(i => i.Id)
                                    .Contains(x.Id))
                    .ToList();
                foreach (var indicatorBarWidgetColumn in indicatorBarWidgetColumnsToDelete)
                {
                    Context.Remove(indicatorBarWidgetColumn);
                }

                foreach (var indicatorBarWidgetColumn in indicatorBarWidget.IndicatorBarWidgetColumns)
                {
                    if (indicatorBarWidgetColumn.Id != 0)
                    {
                        Context.Entry(indicatorBarWidgetColumn).State = EntityState.Modified;
                    }
                    else
                    {
                        Context.Add(indicatorBarWidgetColumn);
                    }
                }
            }

            // BarLabelWidget :
            if (indicatorBarWidget.BarLabelWidgets != null)
            {
                var barLabelWidgetsToDelete = Context.Set<BarLabelWidget>()
                    .Where(x => x.IndicatorBarWidgetId == indicatorBarWidget.Id &&
                                !indicatorBarWidget.BarLabelWidgets.Select(i => i.Id).Contains(x.Id))
                    .ToList();
                foreach (var barLabelWidget in barLabelWidgetsToDelete)
                {
                    RemoveBarLabelWidget(barLabelWidget);
                }

                foreach (var barLabelWidget in indicatorBarWidget.BarLabelWidgets)
                {
                    // BarLabelWidgetLocalization :
                    var barLabelWidgetLocalizationsToDelete = Context.Set<BarLabelWidgetLocalization>()
                        .Where(x => x.BarLabelWidgetId == barLabelWidget.Id &&
                                    !barLabelWidget.BarLabelWidgetLocalizations.Select(w => w.Id).Contains(x.Id))
                        .ToList();
                    foreach (var barLabelWidgetLocalization in barLabelWidgetLocalizationsToDelete)
                    {
                        Context.Remove(barLabelWidgetLocalization);
                    }

                    foreach (var localization in barLabelWidget.BarLabelWidgetLocalizations)
                    {
                        if (localization.Id != 0)
                        {
                            Context.Entry(localization).State = EntityState.Modified;
                        }
                        else
                        {
                            Context.Add(localization);
                        }
                    }

                    if (barLabelWidget.Id != 0)
                    {
                        Context.Entry(barLabelWidget).State = EntityState.Modified;
                    }
                    else
                    {
                        Context.Add(barLabelWidget);
                    }
                }
            }
        }

        private void RemoveBarLabelWidget(BarLabelWidget barLabelWidget)
        {
            var localizations = Context.Set<BarLabelWidgetLocalization>()
                .Where(x => x.BarLabelWidgetId == barLabelWidget.Id)
                .ToList();
            foreach (var localization in localizations)
            {
                Context.Remove(localization);
            }
            Context.Remove(barLabelWidget);
        }

        private void UpdateIndicatorTableWidget(IndicatorTableWidget indicatorTableWidget)
        {
            // TableWidgetColumn
            if (indicatorTableWidget.TableWidgetColumns != null)
            {
                var tableWidgetColumnsToDelete = Context.Set<TableWidgetColumn>()
                    .Where(x => x.IndicatorTableWidgetId == indicatorTableWidget.Id &&
                                !indicatorTableWidget.TableWidgetColumns.Select(i => i.Id).Contains(x.Id))
                    .ToList();
                foreach (var tableWidgetColumn in tableWidgetColumnsToDelete)
                {
                    RemoveTableWidgetColumn(tableWidgetColumn);
                }

                foreach (var tableWidgetColumn in indicatorTableWidget.TableWidgetColumns)
                {
                    // Column Localization :
                    var columnLocalizationsToDelete = Context.Set<TableWidgetColumnLocalization>()
                        .Where(x => x.TableWidgetColumnId == tableWidgetColumn.Id &&
                                    !tableWidgetColumn.TableWidgetColumnLocalizations.Select(w => w.Id).Contains(x.Id))
                        .ToList();
                    foreach (var columnLocalization in columnLocalizationsToDelete)
                    {
                        Context.Remove(columnLocalization);
                    }

                    foreach (var columnLocalization in tableWidgetColumn.TableWidgetColumnLocalizations)
                    {
                        if (columnLocalization.Id != 0)
                        {
                            Context.Entry(columnLocalization).State = EntityState.Modified;
                        }
                        else
                        {
                            Context.Add(columnLocalization);
                        }
                    }

                    if (tableWidgetColumn.Id != 0)
                    {
                        Context.Entry(tableWidgetColumn).State = EntityState.Modified;
                    }
                    else
                    {
                        Context.Add(tableWidgetColumn);
                    }
                }
            }
        }

        private void RemoveTableWidgetColumn(TableWidgetColumn tableWidgetColumn)
        {
            var columnLocalizations = Context.Set<TableWidgetColumnLocalization>()
                .Where(x => x.TableWidgetColumnId == tableWidgetColumn.Id)
                .ToList();
            foreach (var columnLocalization in columnLocalizations)
            {
                Context.Remove(columnLocalization);
            }
            Context.Remove(tableWidgetColumn);
        }

        private void RemoveIndicatorWidget(ICollection<IndicatorWidget> indicatorWidgetsToDelete)
        {
            foreach (var indicatorWidget in indicatorWidgetsToDelete)
            {
                var columns = Context.Set<TableWidgetColumn>()
                    .Where(x => x.IndicatorTableWidgetId == indicatorWidget.Id);
                foreach (var column in columns)
                {
                    RemoveTableWidgetColumn(column);
                }

                var indicatorBarWidgetColumns = Context.Set<IndicatorBarWidgetColumn>()
                    .Where(x => x.IndicatorBarWidgetId == indicatorWidget.Id);
                foreach (var indicatorBarWidgetColumn in indicatorBarWidgetColumns)
                {
                    Context.Remove(indicatorBarWidgetColumn);
                }

                var barLabelWidgets = Context.Set<BarLabelWidget>()
                    .Where(x => x.IndicatorBarWidgetId == indicatorWidget.Id);
                foreach (var barLabelWidget in barLabelWidgets)
                {
                    RemoveBarLabelWidget(barLabelWidget);
                }

                var targetIndicatorChartWidgets = Context.Set<TargetIndicatorChartWidget>()
                    .Where(x => x.IndicatorChartWidgetId == indicatorWidget.Id);
                foreach (var targetIndicatorChartWidget in targetIndicatorChartWidgets)
                {
                    Context.Remove(targetIndicatorChartWidget);
                }

                Context.Remove(indicatorWidget);
            }
        }

        public override void Create(Widget entity)
        {
            Context.Add(entity);

            foreach (var widgetLocalization in entity.WidgetLocalizations)
            {
                Context.Add(widgetLocalization);
            }

            // IndicatorWidget :
            if (entity.Indicators != null && entity.Indicators.Any())
            {
                foreach (var indicator in entity.Indicators)
                {
                    Context.Add(indicator);

                    if (indicator is IndicatorTableWidget indicatorTableWidget)
                    {
                        // TableWidgetColumn
                        if (indicatorTableWidget.TableWidgetColumns != null)
                        {
                            foreach (var tableWidgetColumn in indicatorTableWidget.TableWidgetColumns)
                            {
                                // Column Localization
                                if (tableWidgetColumn.TableWidgetColumnLocalizations != null &&
                                    tableWidgetColumn.TableWidgetColumnLocalizations.Any())
                                {
                                    foreach (var tableWidgetColumnLocalization in tableWidgetColumn.TableWidgetColumnLocalizations)
                                    {
                                        Context.Add(tableWidgetColumnLocalization);
                                    }
                                }
                                Context.Add(tableWidgetColumn);
                            }
                        }
                    }
                    else if (indicator is IndicatorBarWidget indicatorBarWidget)
                    {
                        // IndicatorBarWidgetColumn
                        if (indicatorBarWidget.IndicatorBarWidgetColumns != null)
                        {
                            foreach (var indicatorBarWidgetColumn in indicatorBarWidget.IndicatorBarWidgetColumns)
                            {
                                Context.Add(indicatorBarWidgetColumn);
                            } 
                        }

                        // BarLabelWidget
                        if (indicatorBarWidget.BarLabelWidgets != null)
                        {
                            foreach (var barLabelWidget in indicatorBarWidget.BarLabelWidgets)
                            {
                                // BarLabelWidgetLocalization
                                if (barLabelWidget.BarLabelWidgetLocalizations != null)
                                {
                                    foreach (var barLabelWidgetLocalization in barLabelWidget.BarLabelWidgetLocalizations)
                                    {
                                        Context.Add(barLabelWidgetLocalization);
                                    }
                                }
                                Context.Add(barLabelWidget);
                            }
                        }
                    }
                    else if (indicator is IndicatorChartWidget indicatorChartWidget)
                    {
                        if (indicatorChartWidget.TargetIndicatorChartWidgets != null)
                        {
                            foreach (var targetIndicatorChartWidget in indicatorChartWidget.TargetIndicatorChartWidgets)
                            {
                                Context.Add(targetIndicatorChartWidget);
                            }
                        }
                    }
                }
            }
        }

        public override async Task CreateAsync(Widget entity)
        {
            await Context.AddAsync(entity);
            foreach (var widgetLocalization in entity.WidgetLocalizations)
            {
                await Context.AddAsync(widgetLocalization);
            }
        }

        public override void CreateRange(IEnumerable<Widget> entities)
        {
            foreach (var entity in entities)
            {
                Create(entity);
            }
        }

        public override async Task CreateRangeAsync(IEnumerable<Widget> entities)
        {
            foreach (var entity in entities)
            {
                await CreateAsync(entity);
            }
        }

        public async Task<DateTime?> QueryingLastUpdateUtcIndicatorAsync(long widgetId)
        {
            var query = from widget in Context.Set<Widget>()
                join widgetIndicator in Context.Set<IndicatorWidget>() on widget.Id equals widgetIndicator.WidgetId
                join indicator in Context.Set<IndicatorDefinition>() on widgetIndicator.IndicatorDefinitionId equals indicator.Id
                orderby indicator.LastRefreshUtc
                select indicator.LastRefreshUtc;

            return await query.LastOrDefaultAsync();
        }
    }
}
