using System.Threading;
using System.Threading.Tasks;
using DataMonitoring.Model;

namespace DataMonitoring.Business
{
    public interface IIndicatorQueryBusiness : IUnitOfWork
    {
        Task ExecuteSnapShotIndicatorQueryAsync(CancellationToken stoppingToken);
        Task ExecuteFlowIndicatorQueryAsync(CancellationToken stoppingToken);
        Task ExecuteRatioIndicatorQueryAsync(CancellationToken stoppingToken);

        Task<string> ExecuteQueryResultAsyncToJson(long connectorId, string query);
        Task<string> ExecuteQueryResultPreviewAsyncToJson( long connectorId, string query, int topRowNumber);
        Task<string> ExecuteIndicatorQueryColumnsAsync( long indicatorDefinitionId);
        string FormatQueryWithFakeDate(string valueQuery);
    }
}
