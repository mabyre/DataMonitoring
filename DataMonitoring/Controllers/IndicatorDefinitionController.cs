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
    public class IndicatorDefinitionController : ControllerBase
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<IndicatorDefinitionController>();

        private readonly IIndicatorDefinitionBusiness _indicatorDefinitionBusiness;
        private readonly IIndicatorQueryBusiness _indicatorQueryBusiness;
        private readonly ILocalizationService _localizationService;

        public IndicatorDefinitionController(IIndicatorDefinitionBusiness indicatorDefinitionBusiness, IIndicatorQueryBusiness indicatorQueryBusiness, ILocalizationService localizationService)
        {
            _indicatorDefinitionBusiness = indicatorDefinitionBusiness;
            _indicatorQueryBusiness = indicatorQueryBusiness;
            _localizationService = localizationService;
        }

        // GET: api/IndicatorDefinition
        [HttpGet]
        public async Task<IEnumerable<IndicatorDefinitionViewModel>> Get()
        {
            try
            {
                var indicatorDefinitions = await _indicatorDefinitionBusiness.Repository<IndicatorDefinition>().GetAllAsync();

                foreach (var indicatorDefinition in indicatorDefinitions.ToList())
                {
                    indicatorDefinition.Queries = _indicatorDefinitionBusiness.Repository<IndicatorQuery>()
                        .Find(x => x.IndicatorDefinitionId == indicatorDefinition.Id, x => x.Connector).ToList();
                }

                var indicatorDefinitionViewModelList = indicatorDefinitions.Select(ViewModelConverter.GetIndicatorDefinition).ToList();
                foreach (var indicatorDefinitionViewModel in indicatorDefinitionViewModelList)
                {
                    indicatorDefinitionViewModel.ConnectorTitleListToDisplayed = _indicatorDefinitionBusiness.GetConnectorTitleList(indicatorDefinitionViewModel.Id);
                }

                return indicatorDefinitionViewModelList;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during get all IndicatorDefinition");
                return null;
            }
        }

        // GET: api/IndicatorDefinition/5
        [HttpGet("{id}")]
        public async Task<ActionResult<IndicatorDefinitionViewModel>> Get(long id)
        {
            try
            {
                var indicatorDefinition = await _indicatorDefinitionBusiness.Repository<IndicatorDefinition>()
                    .SingleOrDefaultAsync(x => x.Id == id, x => x.Queries, x=>x.IndicatorLocalizations);

                if (indicatorDefinition == null)
                {
                    Logger.LogError($"IndicatorDefinition id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }

                return Ok(ViewModelConverter.GetIndicatorDefinition(indicatorDefinition));
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during get IndicatorDefinition id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("GetError");
                return StatusCode(500, messageResult);
            }
        }

        // POST: api/IndicatorDefinition
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] IndicatorDefinitionViewModel value)
        {
            try
            {
                var indicator = BusinessConverter.GetIndicatorDefinition(value);

                if (indicator.Type == IndicatorType.Flow)
                {
                    await _indicatorDefinitionBusiness.CheckFlowIndicatorQueriesColumns(indicator);
                }

                _indicatorDefinitionBusiness.CreateOrUpdateIndicatorDefinition(indicator);

                return Ok();
            }
            catch (InvalidOperationException)
            {
                Logger.LogError($"Indicator type Flow : Invalid query.");
                var messageResult = _localizationService.GetLocalizedHtmlString("WrongGroupColumnNameForFlowIndicator");
                return StatusCode(500, messageResult);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during Post Operation IndicatorDefinition");
                var messageResult = _localizationService.GetLocalizedHtmlString("CreateOrUpdateError");
                return StatusCode(500, messageResult);
            }
        }

        // DELETE: api/IndicatorDefinition/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var connector = await _indicatorDefinitionBusiness.Repository<IndicatorDefinition>().GetAsync(id);
                if (connector == null)
                {
                    Logger.LogError($"IndicatorDefinition id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }

                _indicatorDefinitionBusiness.DeleteIndicatorDefinition(id);

                return Ok();
            }
            catch (InvalidOperationException)
            {
                Logger.LogError($"IndicatorDefinition id {id} Delete impossible due to relationship");
                var messageResult = _localizationService.GetLocalizedHtmlString("DeleteImpossibleBecauseRelationship");
                return StatusCode(500, messageResult);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error during delete IndicatorDefinition id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("DeleteError");
                return StatusCode(500, messageResult);
            }
        }

        // POST: api/IndicatorDefinition/GetQueryPreview
        [HttpPost("GetQueryPreview", Name = "GetQueryPreview")]
        public async Task<ActionResult<string>> GetQueryPreview([FromBody] QueryConnectorViewModel value)
        {
            try
            {
                value.Query = _indicatorQueryBusiness.FormatQueryWithFakeDate(value.Query);

                var queryConnector = BusinessConverter.GetIndicatorQuery(value);

                // TODO : c'est quoi ce 20 !!!
                var result = await _indicatorQueryBusiness.ExecuteQueryResultPreviewAsyncToJson( queryConnector.ConnectorId, queryConnector.Query, 20);

                if (result == null)
                {
                    Logger.LogError("QueryPreview : incorrect syntax.");
                    var messageResult = _localizationService.GetLocalizedHtmlString("IncorrectSyntaxError");
                    return BadRequest(messageResult);
                }

                return result;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during QueryPreview");
                return null;
            }
        }

        // POST: api/IndicatorDefinition/GetIndicatorQueryColumns
        [HttpPost("GetIndicatorQueryColumns", Name = "GetIndicatorQueryColumns")]
        public async Task<ActionResult<string>> GetIndicatorQueryColumns([FromBody] long indicatorId)
        {
            try
            {
                var indicatorDefinition = await _indicatorDefinitionBusiness.Repository<IndicatorDefinition>().GetAsync(indicatorId);
                if (indicatorDefinition == null)
                {
                    Logger.LogError($"IndicatorDefinition id {indicatorId} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }

                var result = await _indicatorQueryBusiness.ExecuteIndicatorQueryColumnsAsync( indicatorDefinition.Id);

                if (result == null)
                {
                    Logger.LogError("GetIndicatorQueryColumns : incorrect syntax.");
                    var messageResult = _localizationService.GetLocalizedHtmlString("IncorrectSyntaxError");
                    return BadRequest(messageResult);
                }

                return result;
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during GetIndicatorQueryColumns");
                return null;
            }
        }

        // GET: api/IndicatorDefinition/indicatorTypes
        [HttpGet("indicatorTypes")]
        public List<EnumValue> GetIndicatorTypes()
        {
            return EnumExtension.GetValues<IndicatorType>(_localizationService);
        }
    }
}
