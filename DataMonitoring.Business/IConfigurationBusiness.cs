using System.Collections.Generic;
using System.Threading.Tasks;
using DataMonitoring.Model;
using Microsoft.AspNetCore.Mvc;

namespace DataMonitoring.Business
{
    public interface IConfigurationBusiness : IUnitOfWork
    {
        void CreateOrUpdateStyle(Style style);
        void DeleteStyle(long id);
        void CreateOrUpdateColor(ColorHtml color);
        void DeleteColor(long id);
    }
}
