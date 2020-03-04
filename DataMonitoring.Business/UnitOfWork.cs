//
// https://docs.microsoft.com/fr-fr/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/infrastructure-persistence-layer-implemenation-entity-framework-core
//
using DataMonitoring.DAL;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DataMonitoring.Business
{
    public class UnitOfWork : IUnitOfWork , IDisposable
    {
        protected readonly DbContext Context;
        private IWidgetRepository _widgetRepository;
        private ITimeManagementRepository _timeManagementRepository;
        private IDashboardRepository _dashboardRepository;

        private MonitorBusiness _monitorBusiness;
        private IndicatorQueryBusiness _indicatorQueryBusiness;
        private IndicatorDefinitionBusiness _indicatorDefinitionBusiness;
        private ConfigurationBusiness _configurationBusiness;
        private WidgetBusiness _widgetBusiness;
        private DashboardBusiness _dashboardBusiness;
        private TimeManagementBusiness _timeManagementBusiness;


        public UnitOfWork() : this(new DataMonitoringDbContext())
        {}
        
        public UnitOfWork(DataMonitoringDbContext dataMonitorContext)
        {
            Context = dataMonitorContext;
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            return new Repository<TEntity>(Context);
        }

        public IDashboardRepository DashboardRepository
        {
            get
            {
                return _dashboardRepository = _dashboardRepository ?? new DashboardRepository(Context);
            }
        }

        public IWidgetRepository WidgetRepository
        {
            get
            {
                return _widgetRepository = _widgetRepository ?? new WidgetRepository(Context);
            }
        }

        public ITimeManagementRepository TimeManagementRepository
        {
            get
            {
                return _timeManagementRepository = _timeManagementRepository ?? new TimeManagementRepository( Context );
            }
        }

        public MonitorBusiness MonitorBusiness
        {
            get
            {
                return _monitorBusiness = _monitorBusiness ?? new MonitorBusiness((DataMonitoringDbContext)Context);
            }
        }

        public IndicatorQueryBusiness IndicatorQueryBusiness
        {
            get
            {
                return _indicatorQueryBusiness = _indicatorQueryBusiness ?? new IndicatorQueryBusiness((DataMonitoringDbContext)Context);
            }
        }

        public IndicatorDefinitionBusiness IndicatorDefinitionBusiness
        {
            get
            {
                return _indicatorDefinitionBusiness = _indicatorDefinitionBusiness ?? new IndicatorDefinitionBusiness((DataMonitoringDbContext)Context);
            }
        }

        public WidgetBusiness WidgetBusiness
        {
            get
            {
                return _widgetBusiness = _widgetBusiness ?? new WidgetBusiness((DataMonitoringDbContext)Context);
            }
        }

        public DashboardBusiness DashboardBusiness
        {
            get
            {
                return _dashboardBusiness = _dashboardBusiness ?? new DashboardBusiness((DataMonitoringDbContext)Context);
            }
        }

        public TimeManagementBusiness TimeManagementBusiness
        {
            get
            {
                return _timeManagementBusiness = _timeManagementBusiness ?? new TimeManagementBusiness( (DataMonitoringDbContext)Context );
            }
        }

        public ConfigurationBusiness ConfigurationBusiness
        {
            get
            {
                return _configurationBusiness = _configurationBusiness ?? new ConfigurationBusiness( (DataMonitoringDbContext)Context );
            }
        }

        public int Save()
        {
            return Context.SaveChanges();
        }

        public async Task<int> SaveAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Context.Dispose();
        }

        public IDatabaseTransaction BeginTransaction()
        {
            return new EntityDatabaseTransaction(Context);
        }
    }
}
