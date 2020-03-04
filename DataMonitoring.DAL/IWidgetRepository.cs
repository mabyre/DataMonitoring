using DataMonitoring.Model;
using System;
using System.Threading.Tasks;

namespace DataMonitoring.DAL
{
    public interface IWidgetRepository : IRepository<Widget>
    {
        Task<DateTime?> QueryingLastUpdateUtcIndicatorAsync(long widgetId);
    }
}
