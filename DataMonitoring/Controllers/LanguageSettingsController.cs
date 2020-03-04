using DataMonitoring.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DataMonitoring.Controllers
{
    [Route("api/[controller]")]
    [AllowAnonymous]
    [SecurityHeaders]
    public class LanguageSettingsController : Controller
    {
        private readonly MonitorSettings _settings;

        public LanguageSettingsController(IOptions<MonitorSettings> settings)
        {
            _settings = settings.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(ViewModelConverter.GetLanguageSettings(_settings));
        }
    }
}
