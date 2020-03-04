using DataMonitoring.Business;
using DataMonitoring.Model;
using DataMonitoring.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sodevlog.CoreServices;
using Sodevlog.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [SecurityHeaders]
    public class ColorController : Controller
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<ColorController>();


        private readonly IConfigurationBusiness _configurationBusiness;
        private readonly ILocalizationService _localizationService;

        public ColorController(IConfigurationBusiness configurationBusiness, ILocalizationService localizationService)
        {
            _configurationBusiness = configurationBusiness;
            _localizationService = localizationService;
        }
        
        // GET: api/Color
        [HttpGet]
        public async Task<IEnumerable<ColorViewModel>> Get()
        {
            try
            {
                var colors = await _configurationBusiness.Repository<ColorHtml>().GetAllAsync();
                return colors.Select(ViewModelConverter.GetColor);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during get all color");
                return null;
            }
        }

        // GET: api/Color/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ColorViewModel>> Get(long id)
        {
            try
            {
                var color = await _configurationBusiness.Repository<ColorHtml>().GetAsync(id);
                if (color == null)
                {
                    Logger.LogError($"Color id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }
                return Ok(ViewModelConverter.GetColor(color));
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during get Color id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("GetError");
                return StatusCode(500, messageResult);
            }
        }

        // POST: api/Color
        [HttpPost]
        public ActionResult Post([FromBody] ColorViewModel value)
        {
            try
            {
                var color = BusinessConverter.GetColor(value);

                _configurationBusiness.CreateOrUpdateColor(color);

                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during Post Operation Color");
                var messageResult = _localizationService.GetLocalizedHtmlString("CreateOrUpdateError");
                return StatusCode(500, messageResult);
            }
        }

        // PUT: api/Color/5
        [HttpPut("{id}")]
        public ActionResult Put(long id, [FromBody] ColorViewModel value)
        {
            try
            {
                var color = BusinessConverter.GetColor(value);

                _configurationBusiness.CreateOrUpdateColor(color);

                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during Put Operation Color");
                var messageResult = _localizationService.GetLocalizedHtmlString("CreateOrUpdateError");
                return StatusCode(500, messageResult);
            }
        }

        // DELETE: api/Color/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var color = await _configurationBusiness.Repository<ColorHtml>().GetAsync(id);
                if (color == null)
                {
                    Logger.LogError($"Color id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }

                _configurationBusiness.DeleteColor(id);
                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during delete Color id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("DeleteError");
                return StatusCode(500, messageResult);
            }
        }
    }
}
