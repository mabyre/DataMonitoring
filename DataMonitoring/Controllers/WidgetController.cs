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
    public class WidgetController : Controller
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<WidgetController>();

        private readonly IWidgetBusiness _widgetBusiness;
        private readonly ILocalizationService _localizationService;

        public WidgetController(IWidgetBusiness widgetBusiness, ILocalizationService localizationService)
        {
            _widgetBusiness = widgetBusiness;
            _localizationService = localizationService;
        }

        // GET: api/widget
        [HttpGet]
        public async Task<IEnumerable<WidgetViewModel>> Get()
        {
            try
            {
                var widgets = await _widgetBusiness.WidgetRepository.GetAllAsync();
                var widgetViewModelList = widgets.Select(ViewModelConverter.GetWidget).ToList();

                foreach (var widgetViewModel in widgetViewModelList)
                {
                    widgetViewModel.IndicatorTitleListToDisplayed = _widgetBusiness.GetIndicatorTitleList(widgetViewModel.Id);
                }

                return widgetViewModelList;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during get all widget");
                return null;
            }
        }

        // GET: api/widget/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WidgetViewModel>> Get(long id)
        {
            try
            {
                var widget = await _widgetBusiness.GetWidgetAsync(id);
                if (widget == null)
                {
                    Logger.LogError($"Widget id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }
                return Ok(ViewModelConverter.GetWidget(widget));
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during get widget id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("GetError");
                return StatusCode(500, messageResult);
            }
        }

        // POST: api/widget
        [HttpPost]
        public ActionResult Post([FromBody] WidgetViewModel value)
        {
            try
            {
                var widget = BusinessConverter.GetWidget(value);

                var id = _widgetBusiness.CreateOrUpdateWidget(widget);

                return Ok(id);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during Post Operation widget");
                var messageResult = _localizationService.GetLocalizedHtmlString("CreateOrUpdateError");
                return StatusCode(500, messageResult);
            }
        }

        // POST: api/widget/5
        [HttpPut("{id}")]
        public ActionResult Put(long id, [FromBody] WidgetViewModel value)
        {
            try
            {
                var widget = BusinessConverter.GetWidget(value);

                _widgetBusiness.CreateOrUpdateWidget(widget);

                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during Put Operation widget");
                var messageResult = _localizationService.GetLocalizedHtmlString("CreateOrUpdateError");
                return StatusCode(500, messageResult);
            }
        }

        // DELETE: api/Widget/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var widget = await _widgetBusiness.Repository<Widget>().GetAsync(id);
                if (widget == null)
                {
                    Logger.LogError($"widget id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }

                _widgetBusiness.DeleteWidget(id);

                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                Logger.LogError(ex, $"widget id {id} Delete impossible due to relationship");
                var messageResult = _localizationService.GetLocalizedHtmlString("DeleteImpossibleBecauseRelationship");
                return StatusCode(500, messageResult);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during delete widget id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("DeleteError");
                return StatusCode(500, messageResult);
            }
        }

        // POST: api/Widget/duplicateWidget/5
        [HttpPost("duplicateWidget/{id}")]
        public async Task<ActionResult> DuplicateWidget(long id)
        {
            try
            {
                var widgetModel = await _widgetBusiness.GetWidgetAsync(id);
                if (widgetModel == null)
                {
                    Logger.LogError($"widget id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }

                _widgetBusiness.DuplicateWidget(
                    BusinessConverter.GetWidget(ViewModelConverter.GetWidget(widgetModel)), // HACK : transformation de l'objet Model en ViewModel puis de nouveau en Model pour avoir un new objet ! 
                    _localizationService.GetLocalizedHtmlString("Copy"));
                
                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during duplicate widget id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("DuplicateError");
                return StatusCode(500, messageResult);
            }
        }

        // GET: api/widget/type
        [HttpGet("type")]
        public List<EnumValue> GetWidgetType()
        {
            return EnumExtension.GetValues<WidgetType>(_localizationService);
        }

        // GET: api/widget/columnType
        [HttpGet("columnType")]
        public List<EnumValue> GetColumnType()
        {
            return EnumExtension.GetValues<ColumnType>(_localizationService);
        }

        // GET: api/widget/columnStyle
        [HttpGet("columnStyle")]
        public List<EnumValue> GetColumnStyle()
        {
            return EnumExtension.GetValues<ColumnStyle>(_localizationService);
        }

        // GET: api/widget/alignStyle
        [HttpGet("alignStyle")]
        public List<EnumValue> GetAlignStyle()
        {
            return EnumExtension.GetValues<AlignStyle>(_localizationService);
        }
    }
}