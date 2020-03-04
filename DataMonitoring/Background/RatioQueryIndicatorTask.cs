using DataMonitoring.Business;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sodevlog.Tools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataMonitoring.Background
{
    public class RatioQueryIndicatorTask : BackgroundService
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<RatioQueryIndicatorTask>();

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly int _waitInterval;

        public RatioQueryIndicatorTask(IServiceScopeFactory scopeFactory, IOptions<MonitorSettings> settings)
        {
            _scopeFactory = scopeFactory;

            _waitInterval = settings.Value.WaitIntervalQueryBackgroundTask != 0 ? settings.Value.WaitIntervalQueryBackgroundTask : 30;
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
                        await indicatorQueryBusiness.ExecuteRatioIndicatorQueryAsync( stoppingToken );
                    }
                    catch ( Exception ex )
                    {
                        Logger.LogError( ex, "Error during ExecuteRatioIndicatorQueryAsync" );
                    }
                }

                await Task.Delay( TimeSpan.FromSeconds( _waitInterval ), stoppingToken );
            }

            Logger.LogInformation( "Background task is stopping." );
        }

    }
}
