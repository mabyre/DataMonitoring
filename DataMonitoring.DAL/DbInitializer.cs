//
// Vérifier si des données minimum nécessaires à l'exécution de l'application sont dans la base 
// Et sinon Remplir la base avec des données minimum ...
//
// C'est une bonne idée, en plus avec le ClearDatase ça fait un système assez complet
// mais je n'arriverai jamais à écrire tout ce code C# pour initialiser tous ces objets.
//
// Ja vais plutôt essayer de faire un backup de la base à restaurer en cas de besoin ...
//
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using DataMonitoring.Model;
using Microsoft.Extensions.Logging;
using Sodevlog.Tools;
using Serilog;

namespace DataMonitoring.DAL
{
    public static class DbInitializer
    {
        public static void Initialize( DataMonitoringDbContext context )
        {
            bool saveChanges = false;

            initializeConnectors( context, ref saveChanges );

            initializeColors( context, ref saveChanges );

            initializeTimeManagements( context, ref saveChanges );

            initializeIndicators( context, ref saveChanges );

            if ( saveChanges )
            {
                try
                {
                    context.SaveChanges();
                }
                catch ( Exception e )
                {
                    SdlLog.Logger.LogError( e.InnerException.Message );
                }
            }
        }

#region INIT_OBJECTS

        private static void initializeIndicators( DataMonitoringDbContext context, ref bool changed )
        {
            if ( context.IndicatorDefinitions.Any() )
            {
                SdlLog.Logger.LogInformation( "IndicatorDefinitions in database" );
                return;
            }

            //
            // Recuperer l'ID d'un TimeRange
            //
            long idTimeRange = 0;
            List<TimeRange> timeManagements2 = context.TimeRanges.ToList<TimeRange>();
            if ( timeManagements2.Count != 0 )
            {
                idTimeRange = timeManagements2[0].TimeManagementId;
            }
            else
            {
                changed = false;
                return;
            }

            var indicatordefinitions = new IndicatorDefinition[]
            {
                new IndicatorDefinition
                {
                   Type = IndicatorType.Snapshot,
                   Title = "MyMeasureSnapshot1",
                   RefreshTime = 5, // minutes
                   TimeManagementId = idTimeRange,
                },
                new IndicatorDefinition
                {
                    Type = IndicatorType.Flow,
                    Title = "MyMeasureFlow1",
                    RefreshTime = 5, // minutes
                    DelayForDelete = 5, // en jours ?
                    TimeManagementId = idTimeRange,
                },

                // TODO Type = IndicatorType.Ratio
            };

            foreach ( IndicatorDefinition o in indicatordefinitions )
            {
                context.IndicatorDefinitions.Add( o );
            }

            changed = true;
        }

        private static void initializeTimeManagements( DataMonitoringDbContext context, ref bool changed )
        {
            if ( context.TimeManagements.Any() )
            {
                SdlLog.Logger.LogInformation( "TimeManagement in database" );
                return; 
            }

            var timeManagements = new TimeManagement[]
            {
                new TimeManagement
                {
                   //Id = 1,
                   Name = "MySlipperyTime1"
                },
                new TimeManagement
                {
                   //Id = 2,
                   Name = "MyTimeRange1"
                }
            };

            foreach ( TimeManagement o in timeManagements )
            {
                context.TimeManagements.Add( o );
            }

            // Pour recuperer les Id
            context.SaveChanges();

            List<TimeManagement> timeManagements2 = context.TimeManagements.ToList<TimeManagement>();

            var slippery = new SlipperyTime
            {
                TimeBack = 1,
                UnitOfTime = 0,
                TimeManagementId = timeManagements2[0].Id
            };

            context.SlipperyTimes.Add( slippery );

            var range = new TimeRange
            {
                Name = "MyRange1", // TODO make this field not [Required]
                StartTimeUtc = DateTime.Parse("08:00"),
                EndTimeUtc = DateTime.Parse( "18:00" ),
                TimeManagementId = timeManagements2[1].Id
            };

            context.TimeRanges.Add( range );

            changed = true;
        }

        private static void initializeColors( DataMonitoringDbContext context, ref bool contextChanged )
        {
            if ( context.ColorHtml.Any() )
            {
                SdlLog.Logger.LogInformation( "Colors in database" );
                return; // DB has been seeded
            }
            else
            {
                //
                // Ajouter des coleurs par defaut
                //
                var colors = new ColorHtml[]
                {
                    // Elle est déjà "en dur" dans le code
                    new ColorHtml
                    {
                        Name = "Black",
                        TxtClassName = "txt-color-black",
                        BgClassName = "bg-color-black",
                        HexColorCode = "#000000"
                    },
                    new ColorHtml
                    {
                        Name = "Red",
                        TxtClassName = "txt-color-red",
                        BgClassName = "bg-color-red",
                        HexColorCode = "#FF0000"
                    },
                    new ColorHtml
                    {
                        Name = "Green",
                        TxtClassName = "txt-color-green",
                        BgClassName = "bg-color-green",
                        HexColorCode = "#00FF00"
                    },
                    new ColorHtml
                    {
                        Name = "Blue",
                        TxtClassName = "txt-color-blue",
                        BgClassName = "bg-color-blue",
                        HexColorCode = "#0000FF"
                    },
                };

                foreach ( ColorHtml o in colors )
                {
                    context.ColorHtml.Add( o );
                }

                contextChanged = true;
            }

        }

        private static void initializeConnectors( DataMonitoringDbContext context, ref bool contextChanged )
        {
            if ( context.SqlServerConnectors.Any() )
            {
                SdlLog.Logger.LogInformation( "SqlServerConnectors in database" );
                return; // DB has been seeded
            }
            else
            {
                //
                // Ajouter un connecteur par defaut
                // sous-entend que la base MyDatabase1 existe
                //
                var sqlConnectorsList = new SqlServerConnector[]
                {
                    new SqlServerConnector
                    {
                        Name = "MySqlServerConnector1",
                        TimeZone = TimeZoneInfo.Local.ToString(),
                        // Discriminator
                        // BaseUrl
                        HostName = "localhost",
                        DatabaseName = "MyDatabase1",
                        UseIntegratedSecurity = true,
                    },
                };

                var apiConnectorsList = new ApiConnector[]
                {
                        new ApiConnector
                    {
                        Name = "MyAPIConnecteur1",
                        TimeZone = "E. South America Standard Time",
                        // Discriminator
                        BaseUrl = "www.test1.com",
                        //IndicatorDefinitions = ConnectorType.SqlServer
                    },
                };

                var sqlLiteConnectorsList =  new SqLiteConnector[]
                {
                    new SqLiteConnector
                    {
                        Name = "MySQLite1Connecteur1",
                        TimeZone = TimeZoneInfo.Local.ToString(),
                        // Discriminator
                        // BaseUrl
                        HostName = "localhost",
                        DatabaseName = "MyDatabase1",
                        UseIntegratedSecurity = true,
                        //IndicatorDefinitions = ConnectorType.SqlServer
                    },
                };

                foreach ( SqlServerConnector s in sqlConnectorsList )
                {
                    context.Connectors.Add( s );
                }

                foreach ( ApiConnector s in apiConnectorsList )
                {
                    context.Connectors.Add( s );
                }
                
                foreach ( SqLiteConnector s in sqlLiteConnectorsList )
                {
                    context.Connectors.Add( s );
                }

                contextChanged = true;
            }        

        }

#endregion

        public static void ClearDatabase( DataMonitoringDbContext context )
        {
            // Clear Database
            if ( context.SqlServerConnectors.Any() )
            {

            }

            try
            {
                context.SaveChanges();
            }
            catch ( Exception e )
            {
                SdlLog.Logger.LogError( e.InnerException.Message );
            }
        }
    }
}
