using System.Threading.Tasks;
using DataMonitoring.Model;

namespace DataMonitoring.Business
{
    public interface IIndicatorDefinitionBusiness : IUnitOfWork
    {
        void CreateOrUpdateConnector(Connector connector);
        void DeleteConnector(long id);
        bool TestConnection(Connector connector);


        void CreateOrUpdateIndicatorDefinition(IndicatorDefinition indicator);
        void DeleteIndicatorDefinition(int id);
        string GetConnectorTitleList(long id);
        Task CheckFlowIndicatorQueriesColumns(IndicatorDefinition indicator);
    }
}
