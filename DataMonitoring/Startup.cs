using DataMonitoring.Background;
using DataMonitoring.Business;
using DataMonitoring.DAL;
using DataMonitoring.Resources;
using IdentityServer4.AccessTokenValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Newtonsoft.Json;
using Serilog;
using Sodevlog.Common.Http;
using Sodevlog.CoreServices;
using Sodevlog.Tools;
using System;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace DataMonitoring
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();

            string _baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string _logDir = Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData );

            Environment.SetEnvironmentVariable( "BASEDIR", _baseDir );
            Environment.SetEnvironmentVariable( "LOGDIR", _logDir );

            //
            // SeriLog's configuration by appsettings files
            //
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration( Configuration )
                .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var settingsSection = Configuration.GetSection("ApplicationSettings");

            var settings = settingsSection.Get<MonitorSettings>();

            services.Configure<MonitorSettings>(settingsSection);

            services.AddDbContext<DataMonitoringDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.UseRowNumberForPaging()));

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IConfigurationBusiness, ConfigurationBusiness>();
            services.AddTransient<IMonitorBusiness, MonitorBusiness>();
            services.AddTransient<IIndicatorQueryBusiness, IndicatorQueryBusiness>();
            services.AddTransient<IIndicatorDefinitionBusiness, IndicatorDefinitionBusiness>();
            services.AddTransient<IWidgetBusiness, WidgetBusiness>();
            services.AddTransient<IDashboardBusiness, DashboardBusiness>();
            services.AddTransient<ITimeManagementBusiness, TimeManagementBusiness>();

            services.AddSingleton(Configuration);

            // Personally Identifiable Information (PII)
            IdentityModelEventSource.ShowPII = true;

            /* 
             * Defines the policy for Cross - Origin requests based on the CORS specifications.
             */
            services.AddCors();

            CorsPolicy policy = new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy();

            policy.Headers.Add("*");
            policy.Methods.Add("*");
            policy.Origins.Add("*");
            //policy.SupportsCredentials = true;

            services.AddCors(x => x.AddPolicy("corsGlobalPolicy", policy));

            services.ConfigureQueryLocalizationService(settings, typeof(SharedResource));

            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationSodevlogPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, HasPermissionHandler>();

            services.AddSingleton<ILocalizationService, LocalizationService>();

            if (settings.AuthoritySettings.AuthorityServerActif)
            {
                IdentityModelEventSource.ShowPII = true;
                
                services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                    .AddIdentityServerAuthentication(options =>
                    {
                        options.Authority = settings.AuthoritySettings.AuthorityServerUrl;
                        options.ApiName = settings.AuthoritySettings.ApiName;
                        options.ApiSecret = settings.AuthoritySettings.ApiSecret;
                    });


                var guestPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .RequireClaim("ApplicationAccess", settings.ApplicationScope)
                    .Build();

                services.AddLocalization(options => options.ResourcesPath = "Resources");

                // TODO : AddLocalization
                services.AddMvc()
                    .AddViewLocalization()
                    .AddDataAnnotationsLocalization();

                services.AddMvc().AddJsonOptions(options =>
                    {
                        options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                    });

                //services.AddLocalization(options => options.ResourcesPath = "SharedResource");

                //services.AddMvc(options =>
                //    {
                //        options.Filters.Add(new AuthorizeFilter(guestPolicy));
                //        options.AllowCombiningAuthorizeFilters = false;
                //    })
                //    .AddLocalization(typeof(SharedResource)).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                //    .AddJsonOptions(options =>
                //    {
                //        options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                //        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                //    });
            }
            else
            {
                services.AddLocalization(options => options.ResourcesPath = "Resources");

                // TODO : AddLocalization
                services.AddMvc()
                    .AddViewLocalization()
                    .AddDataAnnotationsLocalization();

                services.AddMvc().AddJsonOptions(options =>
                {
                    options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                });

                //services.AddMvc(opts => { opts.Filters.Add(new AllowAnonymousFilter()); })
                //    .AddLocalization(typeof(SharedResource)).SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                //    .AddJsonOptions(options =>
                //    {
                //        options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                //        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                //    });
            }


            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddSingleton<IHostedService, SnapShotQueryIndicatorTask>();
            services.AddSingleton<IHostedService, FlowQueryIndicatorTask>();
            services.AddSingleton<IHostedService, RatioQueryIndicatorTask>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            // SeriLog
            loggerFactory.AddSerilog(dispose: true);

            // Microsoft.Extensions.Logging
            ApplicationLogging.LoggerFactory = loggerFactory;

            TestProviderLogging.TestLogs( false );

            InitializeDatabase(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            var locOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(locOptions.Value);

            app.UseCors("corsGlobalPolicy");

            var settings = app.ApplicationServices.GetService<IOptions<ApplicationSettings>>().Value;

            if (settings.UseHttpRedirection)
            {
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    //spa.UseAngularCliServer(npmScript: "start");
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:4200"); // Execute "ng serve" in VSCode
                }
            });
        }

        

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DataMonitoringDbContext>();
                context.Database.Migrate();

                DbInitializer.Initialize(context);
            }
        }
    }
}
