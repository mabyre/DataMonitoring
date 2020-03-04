using DataMonitoring.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DataMonitoring.DAL
{
    public interface ITimeManagementRepository : IRepository<TimeManagement>
    {
    }
}