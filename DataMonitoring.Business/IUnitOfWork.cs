using System.Threading.Tasks;
using DataMonitoring.DAL;

namespace DataMonitoring.Business
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;

        IWidgetRepository WidgetRepository { get; }

        IDashboardRepository DashboardRepository { get; }

        int Save();

        Task<int> SaveAsync();

        IDatabaseTransaction BeginTransaction();
    }
}