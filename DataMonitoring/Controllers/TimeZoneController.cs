using DataMonitoring.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Sodevlog.Tools;
using System;
using System.Linq;

namespace DataMonitoring.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    [SecurityHeaders]
    public class TimeZoneController : ControllerBase
    {
        private static readonly ILogger Logger = ApplicationLogging.LoggerFactory.CreateLogger<TimeZoneController>();

        [HttpGet]
        public ActionResult Get()
        {
            var list = TimeZoneInfo.GetSystemTimeZones();
            return Ok(list.Select(ViewModelConverter.GetTimeZone));
        }

        [HttpGet("current")]
        public ActionResult GetCurrentTimeZone()
        {
            var timeZone = TimeZoneInfo.Local;
            return Ok(ViewModelConverter.GetTimeZone(timeZone));
        }
    }
}