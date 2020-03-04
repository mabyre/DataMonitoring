using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMonitoring.Business;
using DataMonitoring.Model;
using DataMonitoring.ViewModel;
using Sodevlog.Tools;
using Sodevlog.CoreServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DataMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [SecurityHeaders]
    public class TimeManagementController : ControllerBase
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<TimeManagementController>();

        private readonly ITimeManagementBusiness _timeManagementBusiness;
        private readonly ILocalizationService _localizationService;

        public TimeManagementController(ITimeManagementBusiness timeManagementBusiness, ILocalizationService localizationService)
        {
            _timeManagementBusiness = timeManagementBusiness;
            _localizationService = localizationService;
        }

        // GET: api/TimeManagement
        [HttpGet]
        public async Task<IEnumerable<TimeManagementViewModel>> Get()
        {
            try
            {
                var timeManagements = await _timeManagementBusiness.GetAllTimeManagementsAsync();
                return timeManagements.Select(ViewModelConverter.GetTimeManagement);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during get all TimeManagement");
                return null;
            }
        }

        // GET: api/TimeManagement/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TimeManagementViewModel>> Get(long id)
        {
            try
            {
                var timeManagement = await _timeManagementBusiness.GetTimeManagementAsync(id);
                if (timeManagement == null)
                {
                    Logger.LogError($"TimeManagement id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }
                return Ok(ViewModelConverter.GetTimeManagement(timeManagement));
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, $"Error during get TimeManagement id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("GetError");
                return StatusCode(500, messageResult);
            }
        }

        // POST: api/TimeManagement
        [HttpPost]
        public ActionResult Post([FromBody] TimeManagementViewModel value)
        {
            try
            {
                var timeManagement = BusinessConverter.GetTimeManagement(value);
                _timeManagementBusiness.CreateOrUpdateTimeManagement(timeManagement);
                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, "Error during Post Operation TimeManagement");
                var messageResult = _localizationService.GetLocalizedHtmlString("CreateOrUpdateError");
                return StatusCode(500, messageResult);
            }
        }

        // DELETE: api/TimeManagement/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var timeManagement = await _timeManagementBusiness.Repository<TimeManagement>().GetAsync(id);
                if (timeManagement == null)
                {
                    Logger.LogError($"TimeManagement id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }

                _timeManagementBusiness.DeleteTimeManagement(id);

                return Ok();
            }
            catch (InvalidOperationException)
            {
                Logger.LogError($"TimeManagement id {id} Delete impossible due to relationship");
                var messageResult = _localizationService.GetLocalizedHtmlString("DeleteImpossibleBecauseRelationship");
                return StatusCode(500, messageResult);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during delete TimeManagement id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("DeleteError");
                return StatusCode(500, messageResult);
            }
        }

        // GET: api/TimeManagement/timeManagementTypes
        [HttpGet("timeManagementTypes")]
        public List<EnumValue> GetTimeManagementTypes()
        {
            List<EnumValue> result = EnumExtension.GetValues<TimeManagementType>( _localizationService );
            return result;
        }

        // GET: api/TimeManagement/unitOfTimes
        [HttpGet("unitOfTimes")]
        public List<EnumValue> GetUnitOfTimes()
        {
            List<EnumValue> result = EnumExtension.GetValues<UnitOfTime>( _localizationService );
            return result;
        }
    }
}
