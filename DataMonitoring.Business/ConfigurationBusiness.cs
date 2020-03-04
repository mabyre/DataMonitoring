using DataMonitoring.DAL;
using DataMonitoring.Model;
using Sodevlog.Tools;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataMonitoring.Business
{
    public class ConfigurationBusiness : UnitOfWork, IConfigurationBusiness
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<ConfigurationBusiness>();

        public ConfigurationBusiness() : base()
        {
        }

        public ConfigurationBusiness( DataMonitoringDbContext dataMonitoringContext ) : base( dataMonitoringContext )
        {
        }

        #region Style
        public void CreateOrUpdateStyle( Style style )
        {
            if ( style.Id == 0 )
            {
                Logger.LogInformation( $"Create new Style" );

                Repository<Style>().Create( style );
                Save();
            }
            else
            {
                Logger.LogInformation( $"Update Style id {style.Id}" );
                Repository<Style>().Update( style );
                Save();
            }
        }

        public void DeleteStyle( long id )
        {
            Logger.LogInformation( $"Delete Style id {id}" );

            Repository<Style>().Delete( id );
            Save();
        }

        public void CreateOrUpdateColor( ColorHtml color )
        {
            if ( color.Id == 0 )
            {
                Logger.LogInformation( $"Create new color" );

                Repository<ColorHtml>().Create( color );
                Save();
            }
            else
            {
                Logger.LogInformation( $"Update color id {color.Id}" );
                Repository<ColorHtml>().Update( color );
                Save();
            }
        }

        public void DeleteColor( long id )
        {
            Logger.LogInformation( $"Delete Color id {id}" );

            Repository<ColorHtml>().Delete( id );
            Save();
        }

        #endregion
    }
}
