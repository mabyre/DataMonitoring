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
    public class DashboardLightController : Controller
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<DashboardLightController>();

        private readonly IDashboardBusiness _dashboardBusiness;
        private readonly ILocalizationService _localizationService;

        public DashboardLightController(IDashboardBusiness dashboardBusiness, ILocalizationService localizationService)
        {
            _dashboardBusiness = dashboardBusiness;
            _localizationService = localizationService;
        }

        // GET: api/Dashboard/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DashboardLightViewModel>> Get(int id)
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

        // PUT: api/Dashboard/5
        [HttpPut("{id}")]
        public ActionResult Put(int id, [FromBody] DashboardLightViewModel value)
        {
            try
            {
                var dashboard = _dashboardBusiness.DashboardRepository.Get(id);
                if (dashboard == null)
                {
                    Logger.LogError($"Dashboard {id} not found");
                    return NotFound($"Dashboard {id} not found");
                }

                // Ne fonctionnait pas !
                //_dashboardBusiness.CreateOrUpdateDashboard(dashboard);
                // A cause de la non utilisation de EntityState.Modified
                // sur le Context.Entry(widget).State

                _dashboardBusiness.CreateOrUpdateDashboard(BusinessConverter.GetDashboardLight(value, dashboard));

                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during Put Operation Dashboard");
                var messageResult = _localizationService.GetLocalizedHtmlString("CreateOrUpdateError");
                return StatusCode(500, messageResult);
            }
        }
    }
}
