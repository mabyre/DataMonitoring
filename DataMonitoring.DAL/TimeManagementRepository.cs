using DataMonitoring.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMonitoring.DAL
{
    public class TimeManagementRepository : Repository<TimeManagement>, ITimeManagementRepository
    {
        public TimeManagementRepository( DbContext context ) : base( context )
        {
        }

        // BRY_WORK
        //public override async Task<TimeManagement> GetAsync( long id )
        //{
        //    IQueryable<TimeManagement> dbQuery = Context.Set<TimeManagement>()
        //        .Include( x => x.SlipperyTime )
        //        .Include( x => x.TimeRanges );

        //    return await dbQuery.SingleOrDefaultAsync( x => x.Id == id );
        //}
    }
}
