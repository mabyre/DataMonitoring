using System;
using Sodevlog.Tools;
using DataMonitoring.DAL;
using DataMonitoring.Model;
using Microsoft.Extensions.Logging;
using System.Linq;


namespace DataMonitoring.Business
{
    public class DashboardBusiness : UnitOfWork, IDashboardBusiness
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<DashboardBusiness>();
      
        public DashboardBusiness() : base()
        {
        }

        public DashboardBusiness(DataMonitoringDbContext dataMonitoringContext) : base(dataMonitoringContext)
        {
        }

        public long CreateOrUpdateDashboard(Dashboard dashboard)
        {
            if (dashboard.Id == 0)
            {
                Logger.LogInformation($"Create new dashboard");

                DashboardRepository.Create(dashboard);
            }
            else
            {
                Logger.LogInformation($"Update dashboard id {dashboard.Id}");

                foreach (var sharedDashboard in dashboard.SharedDashboards)
                {
                    if (sharedDashboard.Id == 0)
                    {
                        var passwordHasher = new PasswordHasher();
                        var securityKey = passwordHasher.HashPassword(sharedDashboard.Key);
                        sharedDashboard.SecurityStamp = securityKey;
                    }
                }

                dashboard.Version = DateTime.UtcNow.Ticks.ToString();

                DashboardRepository.Update(dashboard);
            }

            Save();
            return dashboard.Id;
        }

        public void RemoveDashboard(int id)
        {
            DashboardRepository.Delete(id);
            Save();
        }

        public string GetWidgetTitleList(long id)
        {
            var widgetTitleListResult = string.Empty;
            var dashboardWidgetList = Repository<DashboardWidget>().Find(x => x.DashboardId == id).ToList();
            foreach (var dashboardWidget in dashboardWidgetList)
            {
                var widget = Repository<Widget>().Find(x => x.Id == dashboardWidget.WidgetId).FirstOrDefault();
                if (widget != null)
                {
                    widgetTitleListResult += $"{widget.Title} ; ";
                }
            }

            return widgetTitleListResult;
        }
    }
}
