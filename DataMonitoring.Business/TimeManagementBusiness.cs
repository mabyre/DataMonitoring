using DataMonitoring.DAL;
using DataMonitoring.Model;
using Microsoft.Extensions.Logging;
using Sodevlog.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMonitoring.Business
{
    public class TimeManagementBusiness : UnitOfWork, ITimeManagementBusiness
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<ConfigurationBusiness>();

        public TimeManagementBusiness() : base()
        {
        }

        public TimeManagementBusiness( DataMonitoringDbContext dataMonitoringContext ) : base( dataMonitoringContext )
        {
        }

        public Task<IEnumerable<TimeManagement>> GetAllTimeManagementsAsync()
        {
            var timeManagements = Repository<TimeManagement>().GetAllAsync();

            foreach ( var time in timeManagements.Result )
            {
                var slipperyTime = Repository<SlipperyTime>().Find( x => x.TimeManagementId == time.Id ).SingleOrDefault();
                if ( slipperyTime != null )
                {
                    time.SlipperyTime = slipperyTime;
                }
                else
                {
                    var timeRanges = Repository<TimeRange>().Find( x => x.TimeManagementId == time.Id ).ToList();
                    if ( timeRanges.Any() )
                    {
                        time.TimeRanges = new List<TimeRange>();
                        foreach ( var timeRange in timeRanges )
                        {
                            time.TimeRanges.Add( timeRange );
                        }
                    }
                }
            }
            return timeManagements;
        }

        public async Task<TimeManagement> GetTimeManagementAsync( long id )
        {
            var timeManagement = await Repository<TimeManagement>().GetAsync( id );

            var slipperyTime = Repository<SlipperyTime>().Find( x => x.TimeManagementId == timeManagement.Id ).SingleOrDefault();
            if ( slipperyTime != null )
            {
                timeManagement.SlipperyTime = slipperyTime;
            }
            else
            {
                var timeRanges = Repository<TimeRange>().Find( x => x.TimeManagementId == timeManagement.Id ).ToList();
                if ( timeRanges.Any() )
                {
                    timeManagement.TimeRanges = new List<TimeRange>();
                    foreach ( var timeRange in timeRanges )
                    {
                        timeManagement.TimeRanges.Add( timeRange );
                    }
                }
            }

            return timeManagement;
        }

        public void CreateOrUpdateTimeManagement( TimeManagement timeManagement )
        {
            if ( timeManagement.Id == 0 )
            {
                Logger.LogInformation( "Create new TimeManagement" );

                Repository<TimeManagement>().Create( timeManagement );

                Save();
            }
            else
            {
                Logger.LogInformation( $"Update TimeManagement id {timeManagement.Id}" );

                using ( var transaction = BeginTransaction() )
                {
                    try
                    {
                        var slipperyTime = Repository<SlipperyTime>().Find( x => x.TimeManagementId == timeManagement.Id ).SingleOrDefault();

                        if ( slipperyTime != null && timeManagement.SlipperyTime != null )
                        {
                            slipperyTime.TimeBack = timeManagement.SlipperyTime.TimeBack;
                            slipperyTime.UnitOfTime = timeManagement.SlipperyTime.UnitOfTime;

                            Repository<SlipperyTime>().Update( slipperyTime );
                        }

                        var timeRanges = Repository<TimeRange>().Find( x => x.TimeManagementId == timeManagement.Id ).ToList();

                        if ( timeRanges.Any() )
                        {
                            Repository<TimeRange>().DeleteRange( timeRanges );
                        }

                        if ( timeManagement.TimeRanges != null && timeManagement.TimeRanges.Any() )
                        {
                            foreach ( var timeRange in timeManagement.TimeRanges )
                            {
                                Repository<TimeRange>().Create( timeRange );
                            }
                        }

                        Repository<TimeManagement>().Update( timeManagement );
                        Save();

                        transaction.Commit();
                    }
                    catch ( Exception e )
                    {
                        Logger.LogInformation( e.Message, $"Error during Update TimeManagement id {timeManagement.Id}." );
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void DeleteTimeManagement( int id )
        {
            Logger.LogInformation( $"DeleteTimeManagement id {id}" );

            using ( var transaction = BeginTransaction() )
            {
                try
                {
                    var widget = Repository<Widget>().Find( x => x.TimeManagementId == id ).ToList();
                    if ( widget.Any() )
                    {
                        throw new InvalidOperationException( $"Widget exist with this TimeManagement id {id}" );
                    }

                    var indicator = Repository<IndicatorDefinition>().Find( x => x.TimeManagementId == id ).ToList();
                    if ( indicator.Any() )
                    {
                        throw new InvalidOperationException( $"Indicator exist with this TimeManagement id {id}" );
                    }

                    var timeRanges = Repository<TimeRange>().Find( x => x.TimeManagementId == id ).ToList();
                    if ( timeRanges.Any() )
                    {
                        Repository<TimeRange>().DeleteRange( timeRanges );
                    }

                    var slipperyTime = Repository<SlipperyTime>().Find( x => x.TimeManagementId == id ).SingleOrDefault();
                    if ( slipperyTime != null )
                    {
                        Repository<SlipperyTime>().Delete( slipperyTime );
                    }

                    Repository<TimeManagement>().Delete( id );
                    Save();

                    transaction.Commit();
                }
                catch ( Exception e )
                {
                    Logger.LogError( e, $"Error during delete TimeManagement id {id}" );
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public virtual async Task<TimeRange> GetTimeRangeAsync( long idManagement )
        {
            // BRY_WORK
            var timeManagement = await Repository<TimeManagement>().SingleOrDefaultAsync( x => x.Id == idManagement, x => x.SlipperyTime, x => x.TimeRanges );

            if ( timeManagement == null )
            {
                throw new Exception( "No TimeManagement Founded!" );
            }

            if ( timeManagement.SlipperyTime != null )
            {
                var timeRange = new TimeRange();
                switch ( timeManagement.SlipperyTime.UnitOfTime )
                {
                    case UnitOfTime.Hour:
                        timeRange.StartTimeUtc = DateTime.UtcNow.AddHours( timeManagement.SlipperyTime.TimeBack * -1 );
                        break;

                    case UnitOfTime.Day:
                        timeRange.StartTimeUtc = DateTime.UtcNow.AddDays( timeManagement.SlipperyTime.TimeBack * -1 );
                        break;

                    case UnitOfTime.Month:
                        timeRange.StartTimeUtc = DateTime.UtcNow.AddMonths( timeManagement.SlipperyTime.TimeBack * -1 );
                        break;

                    case UnitOfTime.Year:
                        timeRange.StartTimeUtc = DateTime.UtcNow.AddYears( timeManagement.SlipperyTime.TimeBack * -1 );
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return timeRange;
            }

            return GetCurrentTimeRange( timeManagement.TimeRanges );
        }

        private TimeRange GetCurrentTimeRange( IEnumerable<TimeRange> timeRanges )
        {
            var currentDate = DateTime.UtcNow;

            foreach ( var timeRange in timeRanges )
            {
                var dtDeb = new DateTime( currentDate.Year, currentDate.Month, currentDate.Day,
                    timeRange.StartTimeUtc.Hour, timeRange.StartTimeUtc.Minute, 0 );

                DateTime? dtFin = null;

                if ( timeRange.EndTimeUtc.HasValue )
                {
                    if ( timeRange.EndTimeUtc > timeRange.StartTimeUtc )
                    {
                        dtFin = new DateTime( currentDate.Year, currentDate.Month, currentDate.Day,
                            timeRange.EndTimeUtc.Value.Hour, timeRange.EndTimeUtc.Value.Minute, 0 );
                    }

                    if ( timeRange.EndTimeUtc < timeRange.StartTimeUtc )
                    {
                        var newDate = currentDate.AddDays( 1 );
                        dtFin = new DateTime( newDate.Year, newDate.Month, newDate.Day,
                            timeRange.EndTimeUtc.Value.Hour, timeRange.EndTimeUtc.Value.Minute, 0 );
                    }
                }

                if ( currentDate >= dtDeb )
                {
                    if ( dtFin == null )
                    {
                        return new TimeRange
                        {
                            StartTimeUtc = dtDeb,
                            Name = timeRange.Name,
                            EndTimeUtc = null
                        };
                    }

                    if ( currentDate < dtFin )
                    {
                        return new TimeRange
                        {
                            StartTimeUtc = dtDeb,
                            Name = timeRange.Name,
                            EndTimeUtc = dtFin
                        };
                    }
                }
            }

            return null;
        }


    }
}
