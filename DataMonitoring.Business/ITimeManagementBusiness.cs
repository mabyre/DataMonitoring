using System.Collections.Generic;
using System.Threading.Tasks;
using DataMonitoring.Model;

namespace DataMonitoring.Business
{
    public interface ITimeManagementBusiness : IUnitOfWork
    {
        Task<IEnumerable<TimeManagement>> GetAllTimeManagementsAsync();
        Task<TimeManagement> GetTimeManagementAsync( long id );
        void CreateOrUpdateTimeManagement( TimeManagement timeManagement );
        void DeleteTimeManagement( int id );
    }
}
