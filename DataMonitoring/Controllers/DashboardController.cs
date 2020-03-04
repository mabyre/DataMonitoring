using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMonitoring.Business;
using DataMonitoring.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sodevlog.CoreServices;
using Sodevlog.Tools;

namespace DataMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [SecurityHeaders]
    public class DashboardController : Controller
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<DashboardController>();

        private readonly IDashboardBusiness _dashboardBusiness;
        private readonly ILocalizationService _localizationService;

        public DashboardController(IDashboardBusiness dashboardBusiness, ILocalizationService localizationService)
        {
            _dashboardBusiness = dashboardBusiness;
            _localizationService = localizationService;
        }

        // GET: api/Dashboard
        [HttpGet]
        public async Task<IEnumerable<DashboardViewModel>> Get()
        {
            try
            {
                var dashboards = await _dashboardBusiness.DashboardRepository.GetAllAsync();
                var dashboardViewModelList = dashboards.Select(ViewModelConverter.GetDashboard).ToList();

                foreach (var dashboardViewModel in dashboardViewModelList)
                {
                    dashboardViewModel.WidgetTitleListToDisplayed = _dashboardBusiness.GetWidgetTitleList(dashboardViewModel.Id);
                }

                return dashboardViewModelList;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during get all Dashboard");
                return null;
            }
        }

        // GET: api/Dashboard/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DashboardViewModel>> Get(int id)
        {
            try
            {
                var dashboard = await _dashboardBusiness.DashboardRepository.GetAsync(id);
                if (dashboard == null)
                {
                    Logger.LogError($"Dashboard id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }
                return Ok(ViewModelConverter.GetDashboard(dashboard));
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during get Dashboard id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("GetError");
                return StatusCode(500, messageResult);
            }
        }

        // POST: api/Dashboard
        [HttpPost]
        public ActionResult Post([FromBody] DashboardViewModel value)
        {
            try
            {
                var dashboard = BusinessConverter.GetDashboard(value);

                var id = _dashboardBusiness.CreateOrUpdateDashboard(dashboard);

                return Ok(id);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during Post Operation Dashboard");
                var messageResult = _localizationService.GetLocalizedHtmlString("CreateOrUpdateError");
                return StatusCode(500, messageResult);
            }
        }

        // PUT: api/Dashboard/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] DashboardViewModel value)
        {
            try
            {
                var dashboard = _dashboardBusiness.DashboardRepository.Get(id);
                if (dashboard== null)
                {
                    Logger.LogError($"Dashboard {id} not found");
                    return NotFound($"Dashboard {id} not found");
                }

                _dashboardBusiness.CreateOrUpdateDashboard(BusinessConverter.GetDashboard(value,dashboard));

                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during Put Operation Dashboard");
                var messageResult = _localizationService.GetLocalizedHtmlString("CreateOrUpdateError");
                return StatusCode(500, messageResult);
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var dashboard = await _dashboardBusiness.DashboardRepository.GetAsync(id);
                if (dashboard == null)
                {
                    Logger.LogError($"Dashboard id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }

                _dashboardBusiness.RemoveDashboard(id);
                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during delete Dashboard id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("DeleteError");
                return StatusCode(500, messageResult);
            }
        }
    }
}
