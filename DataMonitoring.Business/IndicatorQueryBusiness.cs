using DataMonitoring.DAL;
using DataMonitoring.Model;
using Sodevlog.Connector;
using Sodevlog.Tools;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IOSqlConnector = Sodevlog.Connector.SqlServerConnector;
using SqlServerConnector = DataMonitoring.Model.SqlServerConnector;

namespace DataMonitoring.Business
{
    public class IndicatorQueryBusiness : UnitOfWork, IIndicatorQueryBusiness
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<IndicatorQueryBusiness>();

        private const string LocalDate = "%%localDate%%";
        private const string UtcDate = "%%utcDate%%";

        public IndicatorQueryBusiness() : base()
        {
        }

        public IndicatorQueryBusiness(DataMonitoringDbContext dataMonitoringContext ) : base( dataMonitoringContext )
        {
        }

        public async Task ExecuteSnapShotIndicatorQueryAsync(CancellationToken stoppingToken)
        {
            var indicatorList = Repository<IndicatorDefinition>().Find(x => x.Type == IndicatorType.Snapshot).ToList();

            var dateUtc = DateTime.UtcNow;

            foreach (var indicator in indicatorList)
            {
                using (var transaction = BeginTransaction())
                {
                    try
                    {
                        if ( CheckExecuteIndicatorQuery( indicator ) )
                        {
                            await ExecuteIndicatorQueryAsync(indicator, dateUtc);
                        }

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        Logger.LogError(e, $"Error during execute query indicator {indicator.Title}");
                    }
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        public async Task ExecuteFlowIndicatorQueryAsync(CancellationToken stoppingToken)
        {
            var indicatorList = Repository<IndicatorDefinition>().Find(x => x.Type == IndicatorType.Flow).ToList();

            var dateUtc = DateTime.UtcNow;

            foreach (var indicator in indicatorList)
            {
                using (var transaction = BeginTransaction())
                {
                    try
                    {
                        var indicatorRationExist = Repository<IndicatorCalculated>().Find(x =>
                            x.IndicatorDefinitionId1 == indicator.Id || x.IndicatorDefinitionId2 == indicator.Id).Any();

                        if (indicatorRationExist == false)
                        {
                            if ( CheckExecuteIndicatorQuery( indicator ) )
                            {
                                await ExecuteIndicatorQueryAsync( indicator, dateUtc );
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        Logger.LogError(e, $"Error during execute query indicator {indicator.Title}");
                    }
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        public async Task ExecuteRatioIndicatorQueryAsync(CancellationToken stoppingToken)
        {
            var indicatorList = await Repository<IndicatorCalculated>().GetAllAsync();

            var dateUtc = DateTime.UtcNow;

            foreach (var indicator in indicatorList)
            {
                using (var transaction = BeginTransaction())
                {
                    try
                    {
                        if ( CheckExecuteIndicatorQuery( indicator ) )
                        {
                            var indicator1 = await Repository<IndicatorDefinition>()
                                    .SingleOrDefaultAsync( x => x.Id == indicator.IndicatorDefinitionId1 );

                            await ExecuteIndicatorQueryAsync( indicator1, dateUtc );

                            var indicator2 = await Repository<IndicatorDefinition>()
                                .SingleOrDefaultAsync( x => x.Id == indicator.IndicatorDefinitionId2 );

                            await ExecuteIndicatorQueryAsync( indicator2, dateUtc );

                            indicator.LastRefreshUtc = DateTime.UtcNow;
                            Repository<IndicatorCalculated>().Update( indicator );
                            await SaveAsync();
                        }
                        
                        transaction.Commit();
                    }
                    catch (Exception e)
                    {
                        transaction.Rollback();
                        Logger.LogError(e, $"Error during execute query indicator {indicator.Title}");
                    }
                }

                if (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        public async Task<string> ExecuteQueryResultAsyncToJson( long connectorId, string query )
        {
            var connector = Repository<Connector>().Find( x => x.Id == connectorId ).Single();

            Logger.LogDebug( $"ExecuteQueryResultAsyncToJson - Connector: {connector.Name} Query: {query}" );

            if ( connector is SqlServerConnector sqlConnector )
            {
                IOSqlConnector connectorSql = null;
                try
                {
                    connectorSql = new IOSqlConnector( sqlConnector.ConnectionString )
                    {
                        SqlQueryString = query
                    };

                    connectorSql.Initialize( AccessMode.Input );
                    connectorSql.Open();

                    var result = await connectorSql.ExecuteReaderAsyncToJson();

                    connectorSql.Close();

                    return result;
                }
                catch ( Exception ex )
                {
                    Logger.LogError( ex, "Error during ExecuteReaderAsyncToJson" );
                    if ( connectorSql != null && connectorSql.IsOpen )
                    {
                        connectorSql.Close();
                    }
                }

                return null;
            }

            return null;
        }

        public async Task<string> ExecuteQueryResultPreviewAsyncToJson( long connectorId, string query, int topRowNumber)
        {
            var connector = Repository<Connector>().Find(x => x.Id == connectorId).Single();

            Logger.LogDebug($"Executing with connector {connector.Name} query {query}");

            if (connector is SqlServerConnector sqlConnector)
            {
                IOSqlConnector connectorSql = null;
                try
                {
                    connectorSql = new IOSqlConnector(sqlConnector.ConnectionString)
                    {
                        SqlQueryString = query
                    };

                    connectorSql.Initialize(AccessMode.Input);
                    connectorSql.Open();

                    var result = await connectorSql.ExecuteReaderPreviewAsyncToJson(topRowNumber);

                    connectorSql.Close();

                    return result;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error during ExecuteQueryResultPreviewAsyncToJson" );
                    if (connectorSql != null && connectorSql.IsOpen)
                    {
                        connectorSql.Close();
                    }
                }

                return null;
            }

            return null;
        }

        public string FormatQueryWithFakeDate(string valueQuery)
        {
            return FormatQueryWithDate(valueQuery, null, DateTime.Now.AddDays(-1));
        }

        public async Task<string> ExecuteIndicatorQueryColumnsAsync( long indicatorDefinitionId)
        {
            var indicatorQuery = Repository<IndicatorQuery>()
                .Find(x => x.IndicatorDefinitionId == indicatorDefinitionId, x => x.Connector).FirstOrDefault();

            if (indicatorQuery == null)
            {
                return string.Empty;
            }

            var connector = Repository<Connector>().Find(x => x.Id == indicatorQuery.ConnectorId).Single();

            if (connector is SqlServerConnector sqlConnector)
            {
                indicatorQuery.Query = FormatQueryWithFakeDate(indicatorQuery.Query);

                IOSqlConnector connectorSql = null;
                try
                {
                    connectorSql = new IOSqlConnector(sqlConnector.ConnectionString)
                    {
                        SqlQueryString = indicatorQuery.Query
                    };
                    connectorSql.Initialize(AccessMode.Input);
                    connectorSql.Open();

                    var result = await connectorSql.ExecuteReaderPreviewAsyncToJson(1);

                    connectorSql.Close();

                    return result;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error during GetIndicatorQueryColumnsAsync");
                    if (connectorSql != null && connectorSql.IsOpen)
                    {
                        connectorSql.Close();
                    }
                }

                return null;
            }

            return null;
        }

        private bool IsValidQuery(string query)
        {
            if (query.ToUpper().Contains("DELETE") || query.ToUpper().Contains("UPDATE"))
            {
                Logger.LogError($"DELETE or UPDATE not allowed in this kind of query  : {query}.");
                return false;
            }

            return true;
        }

        private bool IsValidFlowQuery(string query)
        {
            if (query.Contains("GROUP1") && query.Contains("VALUE"))
            {
                return true;
            }

            Logger.LogError($"name GROUP1 and VALUE not found in query : {query}.");
            return false;
        }

        private async Task ExecuteIndicatorQueryAsync(IndicatorDefinition indicator, DateTime dateUtc)
        {
           var timeRange = indicator.TimeManagementId != null
                ? await TimeManagementBusiness.GetTimeRangeAsync(indicator.TimeManagementId.Value)
                : null;

            if (timeRange == null)
            {
                return;
            }

            var dateExtractionUtc = timeRange.StartTimeUtc;

            var jsonResult = new JArray();

            var listQuery = Repository<IndicatorQuery>().Find(x => x.IndicatorDefinitionId == indicator.Id, x => x.Connector);

            foreach (var query in listQuery)
            {
                if (indicator.Type == IndicatorType.Flow && IsValidFlowQuery(query.Query) == false)
                {
                    Logger.LogError( "ExecuteIndicatorQueryAsync - Query not valid valid" );
                    continue;
                }

                var result = await ExecuteQueryAsync(query, dateExtractionUtc);
                if (result != null)
                {
                    Logger.LogTrace( $"ExecuteIndicatorQueryAsync - Result: {result}" );
                    jsonResult.Merge(JArray.Parse(result));
                }
            }

            if ( jsonResult.Count > 0 )
            {
                await AddExecuteQueryAsync( indicator, jsonResult, dateUtc );
                indicator.LastRefreshUtc = DateTime.UtcNow;
                Repository<IndicatorDefinition>().Update( indicator );
                await SaveAsync();
            }
        }

        private bool CheckExecuteIndicatorQuery(IndicatorDefinition indicator)
        {
            Logger.LogDebug( $"CheckExecuteIndicatorQuery - Indicator: {indicator.Title} - RefreshTime: {indicator.RefreshTime}" );

            if ( indicator.RefreshTime == 0 )
            {
                Logger.LogDebug( $"CheckExecuteIndicatorQuery - RefreshTime == 0" );
                return false;
            }

            if ( indicator.LastRefreshUtc == null )
            {
                Logger.LogDebug( $"CheckExecuteIndicatorQuery - LastRefreshUtc == null" );
                return true;
            }

            var lastRefresh = indicator.LastRefreshUtc.Value.AddMinutes( indicator.RefreshTime );
            bool itsTimeToExecute = lastRefresh < DateTime.UtcNow;

            Logger.LogDebug( $"CheckExecuteIndicatorQuery - itsTimeToExecute: {itsTimeToExecute}" );
            return itsTimeToExecute;
        }

        private async Task<string> ExecuteQueryAsync(IndicatorQuery query, DateTime? dateUtc)
        {
            Logger.LogInformation($"Executing query id: {query.Id}");

            TimeZoneInfo timeZoneInfo = null;
            if (query.Connector.TimeZone != null)
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(query.Connector.TimeZone);
            }
            
            var querySql = dateUtc.HasValue ? FormatQueryWithDate(query.Query, timeZoneInfo, dateUtc.Value) : query.Query;

            if (IsValidQuery(querySql) == false)
            {
                Logger.LogError( $"Query error, the query is not valid : {querySql}" );
                throw new InvalidOperationException("Query error, the query is not valid");
            }

            return await ExecuteQueryResultAsyncToJson(query.ConnectorId, querySql);
        }

        // TODO : le format de la date "yyyy-MM-dd HH:mm:ss" est dependant de la culture
        // ce n'est pas bien
        private string FormatQueryWithDate(string query, TimeZoneInfo timeZoneInfo, DateTime dateUtc)
        {
            var dateformat = "set dateformat ymd;";

            if (query.Contains(LocalDate))
            {
                var localDate = dateUtc.ToLocalTime();
                if (timeZoneInfo != null)
                {
                    localDate = TimeZoneInfo.ConvertTimeFromUtc(dateUtc, timeZoneInfo);
                }
                
                return dateformat + query.Replace(LocalDate, $"'{localDate.ToString("yyyy-MM-dd HH:mm:ss")}'");
            }

            if (query.Contains(UtcDate))
            {
                return dateformat + query.Replace(UtcDate, $"'{dateUtc.ToString("yyyy-MM-dd HH:mm:ss")}'");
            }
            
            return query;
        }

        private async Task AddExecuteQueryAsync(IndicatorDefinition indicator, JArray data, DateTime dateUtc)
        {
            switch (indicator.Type)
            {
                case IndicatorType.Snapshot:
                    await AddSnapshotValue(indicator.Id, data, dateUtc);
                    break;

                case IndicatorType.Flow:
                    await AddFlowValue(indicator.Id, indicator.DelayForDelete, data, dateUtc);
                    break;

                case IndicatorType.Ratio:
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private async Task AddSnapshotValue(long indicatorDefinitionId, JArray data, DateTime dateUtc)
        {
            try
            {
                var value = new SnapshotIndicatorValue
                {
                    DateUtc = dateUtc,
                    IndicatorDefinitionId = indicatorDefinitionId,
                    TableValue = data.ToString()
                };

                await Repository<SnapshotIndicatorValue>().CreateAsync(value);
                await SaveAsync();

                var dataToDelete = Repository<SnapshotIndicatorValue>().Find(x => x.DateUtc < dateUtc && x.IndicatorDefinitionId == indicatorDefinitionId);
                Repository<SnapshotIndicatorValue>().DeleteRange(dataToDelete);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error during adding SnapShotValue : {ex.Message}");
                throw;
            }
        }

        private async Task AddFlowValue(long indicatorDefinitionId, int DelayForDelete, JArray data, DateTime dateUtc)
        {
            try
            {
                foreach (var json in data)
                {
                    var value = new FlowIndicatorValue
                    {
                        DateUtc = dateUtc,
                        IndicatorDefinitionId = indicatorDefinitionId,
                        Group1 = GetValue<string>(json, "GROUP1"),
                        ChartValue = GetValue<decimal>(json, "VALUE")
                    };

                    var group2 = GetValue<string>(json, "GROUP2");
                    if (!string.IsNullOrEmpty(group2))
                    {
                        value.Group2 = group2;
                    }

                    var group3 = GetValue<string>(json, "GROUP3");
                    if (!string.IsNullOrEmpty(group3))
                    {
                        value.Group3 = group3;
                    }

                    var group4 = GetValue<string>(json, "GROUP4");
                    if (!string.IsNullOrEmpty(group4))
                    {
                        value.Group4 = group4;
                    }

                    var group5 = GetValue<string>(json, "GROUP5");
                    if (!string.IsNullOrEmpty(group5))
                    {
                        value.Group5 = group5;
                    }

                    await Repository<FlowIndicatorValue>().CreateAsync(value);
                    await SaveAsync();

                    if (DelayForDelete != 0)
                    {
                        var delayUtc = dateUtc.AddDays(-1 * DelayForDelete);

                        var dataToDelete = Repository<FlowIndicatorValue>().Find(x => x.DateUtc < delayUtc && x.IndicatorDefinitionId == indicatorDefinitionId);
                        Repository<FlowIndicatorValue>().DeleteRange(dataToDelete);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error during adding FlowValue : {ex.Message}");
                throw;
            }
        }

        private T GetValue<T>(JToken token, string column)
        {
            var value = token.SelectToken(column);

            return value != null ? value.Value<T>() : default(T);
        }
    }
}