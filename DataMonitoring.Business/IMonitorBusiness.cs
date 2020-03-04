using System;
using System.Threading.Tasks;
using DataMonitoring.Model;

namespace DataMonitoring.Business
{
    public interface IMonitorBusiness : IUnitOfWork
    {
        Task<SharedDashboard> GetMonitorAsync(string key);
        Task<Dashboard> GetDashboardAsync(long id, string timeZone, string codeLangue = "");
        Task<string> CreateHtmlContentWidgetAsync(long id, TimeZoneInfo timeZoneInfo, string position = "");
        Task<string> CreateHtmlContentWidgetForTestAsync(long id, TimeZoneInfo timeZoneInfo, string position = "");
        Task<Widget> GetWidgetAsync(long id, TimeZoneInfo timeZoneInfo);
    }
}