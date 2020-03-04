using DataMonitoring.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace DataMonitoring.DAL
{
    public class DataMonitoringDbContext : DbContext
    {
        public DataMonitoringDbContext()
        {
        }

        public DataMonitoringDbContext(DbContextOptions<DataMonitoringDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<SqLiteConnector> SqLiteConnectors { get; set; }
        public virtual DbSet<SqlServerConnector> SqlServerConnectors { get; set; }
        public virtual DbSet<ApiConnector> ApiConnectors { get; set; }
        public virtual DbSet<Connector> Connectors { get; set; }

        public virtual DbSet<IndicatorQuery> IndicatorQueries { get; set; }

        public virtual DbSet<IndicatorCalculated> IndicatorCalculated { get; set; }
        public virtual DbSet<IndicatorDefinition> IndicatorDefinitions { get; set; }
        public virtual DbSet<IndicatorLocalization> IndicatorLocalizations { get; set; }

        public virtual DbSet<FlowIndicatorValue> FlowIndicatorValues { get; set; }
        public virtual DbSet<SnapshotIndicatorValue> SnapshotIndicatorValues { get; set; }
        public virtual DbSet<IndicatorValue> IndicatorValues { get; set; }

        public virtual DbSet<IndicatorTableWidget> IndicatorTableWidgets { get; set; }
        public virtual DbSet<IndicatorTableWidgetColumn> IndicatorTableWidgetColumns { get; set; }
        public virtual DbSet<TitleIndicatorTableWidgetColumn> TitleIndicatorTableWidgetColumns { get; set; }
        public virtual DbSet<TranspositionColumnTableWidgetColumn> TranspositionColumnTableWidgetColumns { get; set; }
        public virtual DbSet<CalculatedTableWidgetColumn> CalculatedTableWidgetColumns { get; set; }
        public virtual DbSet<MaskTableWidgetColumn> MaskTableWidgetColumns { get; set; }
        public virtual DbSet<TargetTableWidgetColumn> TargetTableWidgetColumns { get; set; }


        public virtual DbSet<TableWidgetColumn> TableWidgetColumns { get; set; }
        public virtual DbSet<TableWidgetColumnLocalization> TableWidgetColumnLocalizations { get; set; }


        public virtual DbSet<IndicatorBarWidget> IndicatorBarWidget { get; set; }
        public virtual DbSet<IndicatorBarWidgetColumn> IndicatorBarWidgetColumn { get; set; }
        public virtual DbSet<BarLabelWidget> BarLabelWidget { get; set; }
        public virtual DbSet<BarLabelWidgetLocalization> BarLabelWidgetLocalization { get; set; }

        public virtual DbSet<IndicatorGaugeWidget> IndicatorGaugeWidgets { get; set; }

        public virtual DbSet<IndicatorChartWidget> IndicatorChartWidgets { get; set; }
        public virtual DbSet<TargetIndicatorChartWidget> TargetIndicatorChartWidgets { get; set; }


        public virtual DbSet<IndicatorWidget> IndicatorWidgets { get; set; }

        public virtual DbSet<Widget> Widgets { get; set; }
        public virtual DbSet<WidgetLocalization> WidgetLocalizations { get; set; }

        public virtual DbSet<DashboardWidget> DashboardWidgets { get; set; }

        public virtual DbSet<Dashboard> Dashboards { get; set; }
        public virtual DbSet<DashboardLocalization> DashboardLocalizations { get; set; }
        public virtual DbSet<SharedDashboard> SharedDashboards { get; set; }

        public virtual DbSet<TimeManagement> TimeManagements { get; set; }
        public virtual DbSet<TimeRange> TimeRanges { get; set; }
        public virtual DbSet<SlipperyTime> SlipperyTimes { get; set; }

        public virtual DbSet<ColorHtml> ColorHtml { get; set; }
        public virtual DbSet<Style> Styles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                base.OnConfiguring(optionsBuilder);
                return;
            }

            var pathToContentRoot = Directory.GetCurrentDirectory();
            var json = Path.Combine(pathToContentRoot, "appsettings.json");

            if (!File.Exists(json))
            {
                string pathToExe = Process.GetCurrentProcess().MainModule.FileName;
                pathToContentRoot = Path.GetDirectoryName(pathToExe);
            }

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(pathToContentRoot)
                .AddJsonFile("appsettings.json");

            IConfiguration configuration = configurationBuilder.Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DashboardWidget>().HasKey(x => new { x.DashboardId, x.WidgetId });

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().Where(e => !e.IsOwned()).SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            modelBuilder.Entity<IndicatorValue>().HasIndex(x => new { x.IndicatorDefinitionId, x.DateUtc });
        }
    }
}
