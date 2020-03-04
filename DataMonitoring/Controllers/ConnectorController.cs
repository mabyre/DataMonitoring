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
    public class ConnectorController : ControllerBase
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<ConnectorController>();

        private readonly IIndicatorDefinitionBusiness _indicatorDefinitionBusiness;
        private readonly ILocalizationService _localizationService;

        public ConnectorController(IIndicatorDefinitionBusiness indicatorDefinitionBusiness, ILocalizationService localizationService)
        {
            _indicatorDefinitionBusiness = indicatorDefinitionBusiness;
            _localizationService = localizationService;
        }

        // GET: api/Connector
        [HttpGet]
        /*[Authorize]*/
        public async Task<IEnumerable<ConnectorViewModel>> Get()
        {
            try
            {
                var connectors = await _indicatorDefinitionBusiness.Repository<Connector>().GetAllAsync();
                return connectors.Select(ViewModelConverter.GetConnector);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during get all connector");
                return null;
            }
        }

        // GET: api/Connector/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<ConnectorViewModel>> Get(long id)
        {
            try
            {
                var connector = await _indicatorDefinitionBusiness.Repository<Connector>().GetAsync(id);
                if (connector == null)
                {
                    Logger.LogError($"Connector id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }
                return Ok(ViewModelConverter.GetConnector(connector));
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during get connector id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("GetError");
                return StatusCode(500, messageResult);
            }
        }

        // POST: api/Connector
        [HttpPost]
        public ActionResult Post([FromBody] ConnectorViewModel value)
        {
            try
            {
                var connector = BusinessConverter.GetConnector(value);

                _indicatorDefinitionBusiness.CreateOrUpdateConnector(connector);
               
                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during Post Operation connector");
                var messageResult = _localizationService.GetLocalizedHtmlString("CreateOrUpdateError");
                return StatusCode(500, messageResult);
            }
        }

        // PUT: api/Connector/5
        [HttpPut("{id}")]
        public ActionResult Put(long id, [FromBody] ConnectorViewModel value)
        {
            try
            {
                var connector = _indicatorDefinitionBusiness.Repository<Connector>().Get(id);
                if (connector == null)
                {
                    var message = $"Dashboard {id} not found";
                    Logger.LogError(message);
                    return NotFound(message);
                }

                _indicatorDefinitionBusiness.CreateOrUpdateConnector(BusinessConverter.GetConnector(value,connector));

                return Ok();
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during Post Operation connector");
                var messageResult = _localizationService.GetLocalizedHtmlString("CreateOrUpdateError");
                return StatusCode(500, messageResult);
            }
        }

        // POST: api/Connector/TestConnection
        [HttpPost("TestConnection", Name = "TestConnection")]
        public ActionResult TestConnection([FromBody] ConnectorViewModel value)
        {
            try
            {
                var connector = BusinessConverter.GetConnector(value);

                var result = _indicatorDefinitionBusiness.TestConnection(connector);

                return Ok(result);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error during test of connection");
                var messageResult = _localizationService.GetLocalizedHtmlString("TestConnectionError");
                return StatusCode(500, messageResult);
            }
        }

        // DELETE: api/Connector/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var connector = await _indicatorDefinitionBusiness.Repository<Connector>().GetAsync(id);
                if (connector == null)
                {
                    Logger.LogError($"Connector id {id} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }

                _indicatorDefinitionBusiness.DeleteConnector(id);

                return Ok();
            }
            catch (InvalidOperationException)
            {
                Logger.LogError($"Connector id {id} Delete impossible due to relationship");
                var messageResult = _localizationService.GetLocalizedHtmlString("DeleteImpossibleBecauseRelationship");
                return StatusCode(500, messageResult);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"Error during delete connector id {id}");
                var messageResult = _localizationService.GetLocalizedHtmlString("DeleteError");
                return StatusCode(500, messageResult);
            }
        }

        // GET: api/Connector/connectorTypes
        [HttpGet("connectorTypes")]
        public List<EnumValue> GetConnectorTypes()
        {
            return EnumExtension.GetValues<ConnectorType>();
        }

        // GET: api/Connector/autorisationTypes
        [HttpGet("autorisationTypes")]
        public List<EnumValue> GetAutorisationTypes()
        {
            return EnumExtension.GetValues<AutorisationType>(_localizationService);
        }

        // GET: api/Connector/grantTypes
        [HttpGet("grantTypes")]
        public List<EnumValue> GetGrantTypes()
        {
            return EnumExtension.GetValues<GrantType>();
        }
    }
}
