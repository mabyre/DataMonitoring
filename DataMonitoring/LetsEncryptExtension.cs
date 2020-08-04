using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace DataMonitoring
{
    public static class BuilderExtension
    {
        /// <summary>
        /// Adds a file provider for '.well-known' folder that allows lets encrypt to verify domain requests.
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        /// <remarks>See SO answer: http://stackoverflow.com/a/38406699 </remarks>
        public static IApplicationBuilder UseLetsEncryptFolder( this IApplicationBuilder app, IHostingEnvironment env )
        {
            var wellKnownDirectory = new DirectoryInfo( Path.Combine( env.ContentRootPath, ".well-known" ) );
            if ( !wellKnownDirectory.Exists )
            {
                wellKnownDirectory.Create();
            }

            app.UseStaticFiles( new StaticFileOptions
            {
                ServeUnknownFileTypes = true,
                FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider( wellKnownDirectory.FullName ),
                RequestPath = new Microsoft.AspNetCore.Http.PathString( "/.well-known" ),
            } );
            return app;
        }
    }
}

