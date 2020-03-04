using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataMonitoring.Business;
using DataMonitoring.Model;
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
    public class StyleController : Controller
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<StyleController>();
        private readonly IConfigurationBusiness _configurationBusiness;
        private readonly ILocalizationService _localizationService;

        public StyleController(IConfigurationBusiness configurationBusiness, ILocalizationService localizationService)
        {
            _configurationBusiness = configurationBusiness;
            _localizationService = localizationService;
        }

        // GET: api/Style
        [HttpGet]
        public async Task<IEnumerable<StyleViewModel>> Get()
        {
            try
            {
                var styles = await _configurationBusiness.Repository<Style>().GetAllAsync();
                return styles.Select(ViewModelConverter.GetStyle);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during get all Style");
                return null;
            }
        }

        // GET: api/Style/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ConnectorViewModel>> Get(long id)
        {
            try
            {
                var style = await _configurationBusiness.Repository<Style>().GetAsync(id);
                if (style == null)
                {
                    Logger.LogError($"Style id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }
                return Ok(ViewModelConverter.GetStyle(style));
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during get Style id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("GetError");
                return StatusCode(500, messageResult);
            }
        }

        // POST: api/Style
        [HttpPost]
        public ActionResult Post([FromBody] StyleViewModel value)
        {
            try
            {
                var style = BusinessConverter.GetStyle(value);

                _configurationBusiness.CreateOrUpdateStyle(style);

                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during Post Operation Style");
                var messageResult = _localizationService.GetLocalizedHtmlString("CreateOrUpdateError");
                return StatusCode(500, messageResult);
            }
        }

        // PUT: api/Style/5
        [HttpPut("{id}")]
        public ActionResult Put(long id, [FromBody] StyleViewModel value)
        {
            try
            {
                var style = BusinessConverter.GetStyle(value);

                _configurationBusiness.CreateOrUpdateStyle(style);

                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during Put Operation Style");
                var messageResult = _localizationService.GetLocalizedHtmlString("CreateOrUpdateError");
                return StatusCode(500, messageResult);
            }
        }

        // DELETE: api/Style/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var style = await _configurationBusiness.Repository<Style>().GetAsync(id);
                if (style == null)
                {
                    Logger.LogError($"Style id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }

                _configurationBusiness.DeleteStyle(id);
                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during delete Style id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("DeleteError");
                return StatusCode(500, messageResult);
            }
        }
    }
}