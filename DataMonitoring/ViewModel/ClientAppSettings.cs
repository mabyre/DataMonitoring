using System.Collections.Generic;
using System.Security.Policy;

namespace DataMonitoring.ViewModel
{
    public class ClientAppSettings
    {
        public string ApplicationName { get; set; }
        public string ApplicationScope { get; set; }
        public string DefaultLocale { get; set; }
        public string DefaultSkin { get; set; }
        public string ApiServerUrl { get; set; }
        public int ApiRetry { get; set; }
        public int ApiTimeout { get; set; }

        public int WaitIntervalMonitor { get; set; }
        public int WaitIntervalQueryBackgroundTask { get; set; }

        public AuthenticationSettings AuthenticationSettings { get; set; }

        public List<SkinSetting> Skins { get; set; }
    }

    public class AuthenticationSettings
    {
        public bool AuthorityServerActif { get; set; }
        public string AutorityServer { get; set; }
        public string RedirectUrl { get; set; }
        public string ClientId { get; set; }
        public string ResponseType { get; set; }
        public string Scope { get; set; }
        public string PostLogoutRedirectUri { get; set; }
        public bool StartCheckSession { get; set; }
        public bool SilentRenew { get; set; }
        public string StartupRoute { get; set; }
        public string ForbiddenRoute { get; set; }
        public string UnauthorizedRoute { get; set; }
        public bool LogConsoleWarningActive { get; set; }
        public bool LogConsoleDebugActive { get; set; }
        public string MaxIdTokenIatOffsetAllowedInSeconds { get; set; }
    }

    public class SkinSetting
    {
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Label { get; set; }
    }

}