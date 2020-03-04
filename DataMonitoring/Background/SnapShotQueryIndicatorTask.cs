using DataMonitoring.Business;
using Sodevlog.Tools;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DataMonitoring.Background
{
    public class SnapShotQueryIndicatorTask : BackgroundService
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<SnapShotQueryIndicatorTask>();

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly int _waitInterval;

        public SnapShotQueryIndicatorTask( IServiceScopeFactory scopeFactory, IOptions<MonitorSettings> settings )
        {
            _scopeFactory = scopeFactory;

            _waitInterval = settings.Value.WaitIntervalQueryBackgroundTask >= 0 ? settings.Value.WaitIntervalQueryBackgroundTask : 30;
        }

        protected override async Task ExecuteAsync( CancellationToken stoppingToken )
        {
            Logger.LogInformation( "Is starting." );

            stoppingToken.Register( () => Logger.LogInformation( "Is stopping." ) );

            await Task.Delay( TimeSpan.FromSeconds( 30 ), stoppingToken );

            while ( !stoppingToken.IsCancellationRequested )
            {
                Logger.LogTrace( "Doing background work." );

                using ( var scope = _scopeFactory.CreateScope() )
                {
                    var indicatorQueryBusiness = scope.ServiceProvider.GetRequiredService<IIndicatorQueryBusiness>();

                    try
                    {
                        await indicatorQueryBusiness.ExecuteSnapShotIndicatorQueryAsync( stoppingToken );
                    }
                    catch ( Exception ex )
                    {
                        Logger.LogError( ex, "Error during ExecuteSnapShotIndicatorQueryAsync" );
                    }
                }

                await Task.Delay( TimeSpan.FromSeconds( _waitInterval ), stoppingToken );
            }

            Logger.LogInformation( "Background task is stopping." );
        }
    }
}
