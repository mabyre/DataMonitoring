//
// https://docs.microsoft.com/fr-fr/aspnet/core/fundamentals/logging/?tabs=aspnetcore2x&view=aspnetcore-3.1
// https://stackify.com/net-core-loggerfactory-use-correctly/
//
// Exemples d'utilisation de Serilog :
// https://github.com/serilog/serilog-extensions-logging/blob/dev/samples/Sample/Program.cs
// 
// La doc :
// https://github.com/serilog/serilog/wiki
//
// Level Usage
// 1. Verbose : is the noisiest level, rarely(if ever) enabled for a production app.
// 2. Debug  : is used for internal system events that are not necessarily observable from the outside, but useful when determining how something happened.
// 3. Information  : events describe things happening in the system that correspond to its responsibilities and functions.Generally these are the observable actions the system can perform.
// 4. Warning : When service is degraded, endangered, or may be behaving outside of its expected parameters, Warning level events are used.
// 5. Error : When functionality is unavailable or expectations broken, an Error event is used.
// 6. Fatal : The most critical level, Fatal events demand immediate attention.
//
// https://docs.microsoft.com/fr-fr/dotnet/api/microsoft.extensions.logging.loglevel?view=dotnet-plat-ext-3.1
//
// Pour Microsoft
// 0 Trace : Journaux contenant les messages les plus détaillés.Ces messages peuvent contenir des données d’application sensibles.Ils sont désactivés par défaut et ne doivent jamais être activés dans un environnement de production.
// 1 Debug : Journaux utilisés pour l’investigation interactive durant le développement.Ils doivent principalement contenir des informations utiles pour le débogage et n’ont pas d’intérêt à long terme.
// 2 Information : Journaux utilisés pour suivre le flux général de l’application.Ils ont généralement une utilité à long terme.
// 3 Warning : Journaux qui mettent en évidence un événement anormal ou inattendu dans le flux de l’application, mais qui ne provoque pas l’arrêt de l’exécution de l’application.
// 4 Error : Journaux qui indiquent quand le flux actuel de l’exécution s’est arrêté en raison d’une erreur. Ils doivent indiquer une erreur dans l’activité en cours, et non une défaillance de l’application.
// 5 Critical : Journaux qui décrivent un plantage irrécupérable de l’application ou du système, ou une défaillance catastrophique nécessitant une attention immédiate.
// 6 None : pour supprimer tous les messages

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Sodevlog.Tools
{
    public class ApplicationLogging
    {
        private static ILoggerFactory _factory = null;

        // Faire quelque chose pour logger ... en plus ...
        public static void ConfigureLogger(ILoggerFactory factory)
        {
            // Ici pour ajouter au pipeline de logging
            // si nécessaire
        }

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_factory == null)
                {
                    _factory = new LoggerFactory();
                    ConfigureLogger(_factory);
                }
                return _factory;
            }
            set { _factory = value; }
        }
        public static ILogger CreateLogger() => LoggerFactory.CreateLogger("MyLogger");
    }

    public class SdlLog
    {
        private static Microsoft.Extensions.Logging.ILogger _logger = null;

        public static ILogger Logger
        {
            get 
            {
                if ( _logger == null )
                {
                    _logger = ApplicationLogging.LoggerFactory.CreateLogger<SdlLog>();
                }
                return _logger; 

            }
            set { _logger = value; }
        }

    }

    public class TestProviderLogging
    {
        private static readonly Microsoft.Extensions.Logging.ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<TestProviderLogging>();

        public static void TestLogs( bool doMoreTests )
        {
            // https://github.com/serilog/serilog/wiki/Writing-Log-Events
            // Verbose - tracing information and debugging minutiae; generally only switched on in unusual situations
            // Debug - internal control flow and diagnostic state dumps to facilitate pinpointing of recognised problems
            // Information - events of interest or that have relevance to outside observers; the default enabled minimum logging level
            // Warning - indicators of possible issues or service/functionality degradation
            // Error - indicating a failure within the application or connected system
            // Fatal - critical errors causing complete failure of the application

            #region LEVEL DETECTION

            // C'est une bonne idee de tester le niveau des logs configurés 
            // au départ de l'application, ça évite les mauvaises surprises
            //

            if ( Log.IsEnabled( Serilog.Events.LogEventLevel.Verbose ) )
            {
                Log.Fatal( "1. Verbose enable ..." );
            }

            if ( Log.IsEnabled( Serilog.Events.LogEventLevel.Debug ) )
            {
                Log.Fatal( "2. Debug enable ..." );
            }

            if ( Log.IsEnabled( Serilog.Events.LogEventLevel.Information ) )
            {
                Log.Fatal( "3. Information enable ..." );
            }

            if ( Log.IsEnabled( Serilog.Events.LogEventLevel.Warning ) )
            {
                Log.Fatal( "4. Warning enable ..." );
            }

            if ( Log.IsEnabled( Serilog.Events.LogEventLevel.Error ) )
            {
                Log.Fatal( "5. Error enable ..." );
            }

            if ( Log.IsEnabled( Serilog.Events.LogEventLevel.Fatal ) )
            {
                Log.Fatal( "6. Fatal enable ..." );
            }

            #endregion

            if ( doMoreTests )
            {
                //
                // Log de Serilog 
                //
                Log.Fatal( "-------------------------------- SeriLog Level" );
                Log.Verbose( "1. Log Verbose" );
                Log.Debug( "2. Log Debug" );
                Log.Information( "3. Log Information" );
                Log.Warning( "4. Log Warning" );
                Log.Error( "5. Log Error" );
                Log.Fatal( "6. Log Fatal" );

                Log.Fatal( "-------------------------------- SeriLog other tests" );
                var myLog = Log.ForContext<TestProviderLogging>();
                myLog.Information( "Hello!" );
                
                Logger.LogCritical( "-------------------------------- Logger " );

                // LogVerbose n'existe pas !?
                Logger.LogTrace( "2. This is a Trace message" );
                Logger.LogDebug( "2. This is a Debug message" );
                Logger.LogInformation( "3. This is an Information" );
                Logger.LogWarning( "4. This is a Warnig" );
                Logger.LogError( "5. This is an Error" );
                Logger.LogCritical( "6. This is a Critical" );

                SdlLog.Logger.LogCritical( "-------------------------------- SodevlogLog.Logger " );

                SdlLog.Logger.LogTrace( "2. This is Trace with Sodevlog Logger" );
                SdlLog.Logger.LogDebug( "2. This is a Debug with Sodevlog Logger" );
                SdlLog.Logger.LogInformation( "3. This is an Information with Sodevlog Logger" );
                SdlLog.Logger.LogWarning( "4. This is a Warnig with Sodevlog Logger" );
                SdlLog.Logger.LogError( "5. This is an Error with Sodevlog Logger" );
                SdlLog.Logger.LogCritical( "6. This is a Critical with Sodevlog Logger" );

                SdlLog.Logger.LogCritical( "-------------------------------- Other Tests " );
                SdlLog.Logger.Log( LogLevel.Trace, "messsage de trace" );

                int to_make_albreakpoint = 0;
            }
        }
    }
}
