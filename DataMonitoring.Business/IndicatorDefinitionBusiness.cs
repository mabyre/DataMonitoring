using System;
using System.Linq;
using System.Threading.Tasks;
using DataMonitoring.DAL;
using DataMonitoring.Model;
using Microsoft.Extensions.Logging;
using Sodevlog.Tools;
using Newtonsoft.Json.Linq;


namespace DataMonitoring.Business
{
    public class IndicatorDefinitionBusiness : UnitOfWork, IIndicatorDefinitionBusiness
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<IndicatorDefinitionBusiness>();

        public IndicatorDefinitionBusiness() : base()
        {
        }

        public IndicatorDefinitionBusiness( DataMonitoringDbContext dataMonitoringContext ) : base( dataMonitoringContext )
        {
        }

        #region Connector
        public void CreateOrUpdateConnector( Connector connector )
        {
            if ( connector.Id == 0 )
            {
                Logger.LogInformation( "Create new connector" );

                Repository<Connector>().Create( connector );
                Save();
            }
            else
            {
                Logger.LogInformation( $"Update connector id {connector.Id}" );

                Repository<Connector>().Update( connector );
                Save();
            }
        }

        public void DeleteConnector( long id )
        {
            Logger.LogInformation( $"Delete connector id {id}" );

            var indicators = Repository<IndicatorQuery>().Find( x => x.ConnectorId == id ).ToList();
            if ( indicators.Any() )
            {
                throw new InvalidOperationException( $"delete invalid, query exist with this connector {id}" );
            }

            Repository<Connector>().Delete( id );
            Save();
        }

        public bool TestConnection( Connector connector )
        {
            if ( connector is SqlServerConnector sqlConnector )
            {
                try
                {
                    var connectorSql = new Sodevlog.Connector.SqlServerConnector( sqlConnector.ConnectionString );
                    return connectorSql.TestConnection();
                }
                catch ( Exception ex )
                {
                    Logger.LogError( ex, "Error during Test Connection SQL" );
                    return false;
                }
            }

            return false;
        }

        #endregion

        #region IndicatorDefinition

        public void CreateOrUpdateIndicatorDefinition( IndicatorDefinition indicator )
        {
            if ( indicator.Id == 0 )
            {
                Logger.LogInformation( "Create new IndicatorDefinition" );

                Repository<IndicatorDefinition>().Create( indicator );
                Save();
            }
            else
            {
                Logger.LogInformation( $"Update IndicatorDefinition id {indicator.Id}" );

                using ( var transaction = BeginTransaction() )
                {
                    try
                    {
                        var indicatorsQueries = Repository<IndicatorQuery>()
                            .Find( x => x.IndicatorDefinitionId == indicator.Id ).ToList();

                        if ( indicatorsQueries.Any() )
                        {
                            Repository<IndicatorQuery>().DeleteRange( indicatorsQueries );
                        }

                        if ( indicator.Queries != null )
                        {
                            foreach ( var indicatorQuery in indicator.Queries )
                            {
                                Repository<IndicatorQuery>().Create( indicatorQuery );
                            }
                        }

                        var indicatorsLocalizations = Repository<IndicatorLocalization>()
                            .Find( x => x.IndicatorDefinitionId == indicator.Id ).ToList();

                        if ( indicatorsLocalizations.Any() )
                        {
                            Repository<IndicatorLocalization>().DeleteRange( indicatorsLocalizations );
                        }

                        if ( indicator.IndicatorLocalizations != null )
                        {
                            foreach ( var indicatorLocalization in indicator.IndicatorLocalizations )
                            {
                                Repository<IndicatorLocalization>().Create( indicatorLocalization );
                            }
                        }

                        Repository<IndicatorDefinition>().Update( indicator );
                        Save();

                        transaction.Commit();
                    }
                    catch ( Exception e )
                    {
                        Logger.LogError( e, $"Error during Update IndicatorDefinition id {indicator.Id}." );
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void DeleteIndicatorDefinition( int id )
        {
            Logger.LogInformation( $"Delete IndicatorDefinition id {id}" );

            using ( var transaction = BeginTransaction() )
            {
                try
                {
                    var indicators = Repository<IndicatorWidget>().Find( x => x.IndicatorDefinitionId == id ).ToList();
                    if ( indicators.Any() )
                    {
                        throw new InvalidOperationException( $"Delete invalid, indicator widget exist with this indicator {id}" );
                    }

                    var indicatorsQueries = Repository<IndicatorQuery>().Find( x => x.IndicatorDefinitionId == id ).ToList();
                    if ( indicatorsQueries.Any() )
                    {
                        Repository<IndicatorQuery>().DeleteRange( indicatorsQueries );
                    }

                    var indicatorLocalizations = Repository<IndicatorLocalization>().Find( x => x.IndicatorDefinitionId == id ).ToList();
                    if ( indicatorLocalizations.Any() )
                    {
                        Repository<IndicatorLocalization>().DeleteRange( indicatorLocalizations );
                    }

                    var indicatorValues = Repository<IndicatorValue>().Find( x => x.IndicatorDefinitionId == id ).ToList();
                    if ( indicatorValues.Any() )
                    {
                        Repository<IndicatorValue>().DeleteRange( indicatorValues );
                    }

                    Repository<IndicatorDefinition>().Delete( id );
                    Save();

                    transaction.Commit();
                }
                catch ( Exception e )
                {
                    Logger.LogError( e, $"Error during Delete indicatorDefinition id {id}" );
                    transaction.Rollback();
                    throw;
                }

            }
        }

        public string GetConnectorTitleList( long id )
        {
            var connectorTitleListResult = string.Empty;
            var indicatorQueryList = Repository<IndicatorQuery>().Find( x => x.IndicatorDefinitionId == id ).ToList();
            foreach ( var indicatorQuery in indicatorQueryList )
            {
                var connector = Repository<Connector>().Find( x => x.Id == indicatorQuery.ConnectorId ).FirstOrDefault();
                if ( connector != null )
                {
                    connectorTitleListResult += $"{connector.Name} ; ";
                }
            }

            return connectorTitleListResult;
        }

        public async Task CheckFlowIndicatorQueriesColumns( IndicatorDefinition indicator )
        {
            if ( indicator.Queries != null && indicator.Queries.Any() )
            {
                foreach ( var indicatorConnector in indicator.Queries )
                {
                    string query = IndicatorQueryBusiness.FormatQueryWithFakeDate( indicatorConnector.Query );
                    var result = await IndicatorQueryBusiness.ExecuteQueryResultPreviewAsyncToJson( indicatorConnector.ConnectorId, query, 1 );
                    var jsonResult = new JArray();
                    jsonResult.Merge( JArray.Parse( result ) );
                    if ( jsonResult.Count > 0 )
                    {
                        var first = jsonResult.First;
                        var group1Exist = false;
                        var valueExist = false;
                        var anyColumnInError = false;
                        foreach ( JProperty child in first.Children() )
                        {
                            if ( child.Name == "GROUP1" )
                            {
                                group1Exist = true;
                            }
                            else if ( child.Name == "VALUE" )
                            {
                                valueExist = true;
                            }
                            else if ( child.Name != "GROUP2" && child.Name != "GROUP3" && child.Name != "GROUP4" &&
                                     child.Name != "GROUP5" )
                            {
                                anyColumnInError = true;
                            }
                        }

                        if ( !group1Exist )
                        {
                            Logger.LogError( $"Error during CheckFlowIndicatorQueriesColumns, ConnectorId:[{indicatorConnector.ConnectorId}]. " +
                                            $"GROUP1 is missing." );
                            throw new InvalidOperationException();
                        }
                        else if ( !valueExist )
                        {
                            Logger.LogError( $"Error during CheckFlowIndicatorQueriesColumns, ConnectorId:[{indicatorConnector.ConnectorId}]. " +
                                            $"VALUE is missing." );
                            throw new InvalidOperationException();
                        }
                        else if ( anyColumnInError )
                        {
                            Logger.LogError( $"Error during CheckFlowIndicatorQueriesColumns, ConnectorId:[{indicatorConnector.ConnectorId}]. " +
                                            $"Wrong group column name." );
                            throw new InvalidOperationException();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
