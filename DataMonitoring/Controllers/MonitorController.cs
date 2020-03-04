using DataMonitoring.Business;
using DataMonitoring.Model;
using DataMonitoring.ViewModel;
using Sodevlog.CoreServices;
using Sodevlog.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DataMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [SecurityHeaders]
    public class MonitorController : ControllerBase
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<MonitorController>();

        private readonly IMonitorBusiness _monitorBusiness;
        private readonly ILocalizationService _localizationService;

        private const string KeyPreviewToProd = "9AB3B0C9-6E01-43C8-A7B2-0EF548E2B5DF";

        public MonitorController(IMonitorBusiness monitorBusiness, ILocalizationService localizationService)
        {
            _monitorBusiness = monitorBusiness;
            _localizationService = localizationService;
        }

        [HttpGet("{key}")]
        [AllowAnonymous]
        public async Task<ActionResult<MonitorViewModel>> Get(string key)
        {
            try
            {
                var shareDashboard = await _monitorBusiness.GetMonitorAsync(key);
                if (shareDashboard == null)
                {
                    Logger.LogError($"Monitor Key {key} NotFound");
                    var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                    return NotFound(messageResult);
                }

                var dashboard = await _monitorBusiness.GetDashboardAsync(shareDashboard.DashboardId, shareDashboard.TimeZone, shareDashboard.CodeLangue);
                if (dashboard == null)
                {
                    throw new Exception("Dashboard notFound");
                }

                var colors = await _monitorBusiness.Repository<ColorHtml>().GetAllAsync();

                var monitorViewModel = ViewModelConverter.GetMonitor(shareDashboard, dashboard, colors.ToList());
                return Ok(monitorViewModel);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during get monitor key: {key}");
                var messageResult = _localizationService.GetLocalizedHtmlString("GetError");
                return StatusCode(500, messageResult);
            }
        }

        [HttpGet("widget/test/{id}")]
        public async Task<ActionResult<WidgetContentViewModel>> GetWidgetTest(long id)
        {
            try
            {
                return await GetWidget(id, true);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during Get widget id {id} mode Test");

                var messageResult = _localizationService.GetLocalizedHtmlString("GetError");
                return StatusCode(500, messageResult);
            }
        }

        [HttpGet("widget/{id}/{key}")]
        [AllowAnonymous]
        public async Task<ActionResult<WidgetContentViewModel>> GetWidget(long id,string key)
        {
            try
            {
                if (key != KeyPreviewToProd)
                {
                    var shareDashboard = await _monitorBusiness.GetMonitorAsync(key);
                    if (shareDashboard == null)
                    {
                        Logger.LogError($"Monitor Key {key} NotFound");
                        var messageResult = _localizationService.GetLocalizedHtmlString("NotFoundError");
                        return NotFound(messageResult);
                    }
                }
                
                return await GetWidget(id, false);
            }
            catch (Exception e)
            {
                Logger.LogError(e, $"Error during Get widget id {id}");

                var messageResult = _localizationService.GetLocalizedHtmlString("GetError");
                return StatusCode(500, $"{messageResult} - {e.Message}");
            }
        }

        private async Task<WidgetContentViewModel> GetWidget(long id, bool testMode)
        {
            TimeZoneInfo timeZoneInfo = null;
            
            var query = HttpContext.Request.Query;
            var tzExists = query.TryGetValue("tz", out var timeZone);
            if (tzExists)
            {
                timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            }

            query.TryGetValue("position", out var position);
            
            var widget = await _monitorBusiness.GetWidgetAsync(id, timeZoneInfo);

            var colorTitleName = string.IsNullOrEmpty(widget.TitleColorName) ? "Black" : widget.TitleColorName;
            var colorHtml = await _monitorBusiness.Repository<ColorHtml>().SingleOrDefaultAsync(x => x.Name == colorTitleName);

            var lastUpdate = widget.LastUpdateUtc != null
                ? timeZoneInfo != null
                    ? TimeZoneInfo.ConvertTimeFromUtc(widget.LastUpdateUtc.Value, timeZoneInfo)
                    : widget.LastUpdateUtc.Value.ToLocalTime()
                : (DateTime?) null;

            WidgetContentViewModel widgetContentVM = new WidgetContentViewModel()
            {
                Title = widget.TitleToDisplay,
                TitleFontSize = widget.TitleFontSize,
                LastUpdateToDisplay = lastUpdate.HasValue ? lastUpdate.Value.ToString( "HH:mm" ) : string.Empty,
                TitleClassColor = colorHtml.TxtClassName,
                RefreshTime = widget.RefreshTime,
                WidgetType = widget.Type,
                Content = testMode
                    ? await _monitorBusiness.CreateHtmlContentWidgetForTestAsync( id, timeZoneInfo, position )
                    : await _monitorBusiness.CreateHtmlContentWidgetAsync( id, timeZoneInfo, position )
            };

            return widgetContentVM;
        }
    }
}