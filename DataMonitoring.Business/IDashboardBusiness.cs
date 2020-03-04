using DataMonitoring.Model;

namespace DataMonitoring.Business
{
    public interface IDashboardBusiness : IUnitOfWork
    {
        long CreateOrUpdateDashboard( Dashboard dashboard );
        void RemoveDashboard( int id );
        string GetWidgetTitleList( long id );
    }
}
