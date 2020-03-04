using DataMonitoring.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DataMonitoring.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [SecurityHeaders]
    public class ClientAppSettingsController : Controller
    {
        private readonly MonitorSettings _settings;

        public ClientAppSettingsController(IOptions<MonitorSettings> settings)
        {
            _settings = settings.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(ViewModelConverter.GetAppSettings(_settings));
        }
    }
}
