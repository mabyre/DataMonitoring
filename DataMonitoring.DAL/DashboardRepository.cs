using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMonitoring.Model;
using Microsoft.EntityFrameworkCore;
//using NuGet.Protocol.Core.Types;

namespace DataMonitoring.DAL
{
    public class DashboardRepository : Repository<Dashboard>, IDashboardRepository
    {
        public DashboardRepository(DbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<Dashboard>> GetAllAsync()
        {
            var dbQuery = Context.Set<Dashboard>()
                .Include(x => x.DashboardLocalizations);

            return await dbQuery.ToListAsync();
        }

        public override IEnumerable<Dashboard> GetAll()
        {
            var dbQuery = Context.Set<Dashboard>()
                .Include(x => x.DashboardLocalizations);

            return dbQuery.ToList();
        }

        public override async Task<Dashboard> GetAsync(long id)
        {
            IQueryable<Dashboard> dbQuery = Context.Set<Dashboard>()
                .Include(x => x.DashboardLocalizations)
                .Include(x => x.Widgets)
                .Include(x => x.SharedDashboards);

            return await dbQuery.SingleOrDefaultAsync(x => x.Id == id);
        }

        public override Dashboard Get(long id)
        {
            IQueryable<Dashboard> dbQuery = Context.Set<Dashboard>()
                .Include(x => x.DashboardLocalizations)
                .Include(x => x.Widgets)
                .Include(x => x.SharedDashboards);

            return dbQuery.SingleOrDefault(x => x.Id == id);
        }

        public override void Delete(Dashboard entity)
        {
            if (entity.DashboardLocalizations != null)
            {
                foreach (var localization in entity.DashboardLocalizations)
                {
                    Context.Remove<DashboardLocalization>(localization);
                }
            }
            if (entity.Widgets != null)
            {
                foreach (var widget in entity.Widgets)
                {
                    Context.Remove<DashboardWidget>(widget);
                }
            }
            if (entity.SharedDashboards != null)
            {
                foreach (var sharedDashboard in entity.SharedDashboards)
                {
                    Context.Remove<SharedDashboard>(sharedDashboard);
                }
            }

            Context.Remove(entity);
        }

        public override void Delete(long id)
        {
            var dashboard = Get(id);
            Delete(dashboard);
        }

        public override void DeleteRange(IEnumerable<Dashboard> entities)
        {
            foreach (var dashboard in entities)
            {
                Delete(dashboard);
            }
        }

        public override void Update(Dashboard entity)
        {
            // Localizations
            //
            var localizationsToDelete = Context.Set<DashboardLocalization>()
                .Where(x => x.DashboardId == entity.Id &&
                            !entity.DashboardLocalizations.Select(w => w.Id).Contains(x.Id));

            foreach (var localization in localizationsToDelete)
            {
                Context.Remove(localization);
            }

            foreach (var localization in entity.DashboardLocalizations)
            {
                if (localization.Id <= 0)
                {
                    Context.Add(localization);
                }
                else
                {
                    Context.Entry(localization).State = EntityState.Modified;
                }
            }

            // Widgets
            //
            var widgetsToDelete = Context.Set<DashboardWidget>()
                .Where(x => x.DashboardId == entity.Id &&
                            !entity.Widgets.Select(w => w.WidgetId).Contains(x.WidgetId)).ToList();

            foreach (var widget in widgetsToDelete)
            {
                Context.Remove(widget);
            }

            var oldWidgets = Context.Set<DashboardWidget>().Where(x => x.DashboardId == entity.Id).ToList();

            foreach (var widget in entity.Widgets)
            {
                if (oldWidgets.Where(w => w.WidgetId == widget.WidgetId).Count() == 0)
                {
                    Context.Add(widget);
                }
                else
                {
                    Context.Entry(widget).State = EntityState.Modified;
                }
            }

            // SharedDashboards
            //
            var sharedDashboardsToDelete = Context.Set<SharedDashboard>()
                .Where(x => x.DashboardId == entity.Id &&
                            !entity.SharedDashboards.Select(w => w.Id).Contains(x.Id));
            foreach (var sharedDashboard in sharedDashboardsToDelete)
            {
                Context.Remove(sharedDashboard);
            }

            foreach (var sharedDashboard in entity.SharedDashboards)
            {
                if (sharedDashboard.Id <= 0)
                {
                    Context.Add(sharedDashboard);
                }
                else
                {
                    Context.Entry(sharedDashboard).State = EntityState.Modified;
                }
            }

            Context.Entry(entity).State = EntityState.Modified;
        }

        public override void Create(Dashboard entity)
        {
            Context.Add(entity);

            foreach (var localization in entity.DashboardLocalizations)
            {
                Context.Add(localization);
            }

            // DashboardWidget :
            if (entity.Widgets != null && entity.Widgets.Any())
            {
                foreach (var widget in entity.Widgets)
                {
                    Context.Add(widget);
                }
            }
        }

        public override async Task CreateAsync(Dashboard entity)
        {
            await Context.AddAsync(entity);
            foreach (var localization in entity.DashboardLocalizations)
            {
                await Context.AddAsync(localization);
            }

            if (entity.Widgets != null && entity.Widgets.Any())
            {
                foreach (var widget in entity.Widgets)
                {
                    await Context.AddAsync(widget);
                }
            }
        }

        public override void CreateRange(IEnumerable<Dashboard> entities)
        {
            foreach (var entity in entities)
            {
                Create(entity);
            }
        }

        public override async Task CreateRangeAsync(IEnumerable<Dashboard> entities)
        {
            foreach (var entity in entities)
            {
                await CreateAsync(entity);
            }
        }
    }
}
