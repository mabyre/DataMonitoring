using System;
using Microsoft.Extensions.DependencyInjection;

namespace DataMonitoring
{
    public static class DependencyInjectionExtension
    {
        public static void ConfigureQueryLocalizationService(
            this IServiceCollection source,
            MonitorSettings settings,
            Type type)
        {

        }
    }
}
