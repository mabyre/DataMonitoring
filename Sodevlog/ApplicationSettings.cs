using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sodevlog.Common.Http
{
    public class ApplicationSettings
    {
        public string ApplicationName { get; set; }

        public string ApplicationScope { get; set; }

        public string DefaultLocale { get; set; }

        public string ApiServerUrl { get; set; }

        public int ApiRetry { get; set; }

        public int ApiTimeout { get; set; }

        public AuthoritySettings AuthoritySettings { get; set; }

        public IEnumerable<CultureSupported> CultureSupported  { get; set; }
        public bool UseHttpRedirection { get; set; }
    }

    public class CultureSupported
    {
        public string Key { get; set; }

        public string Alt { get; set; }

        public string Title { get; set; }

        public string Culture { get; set; }
    }

    public class AuthoritySettings
    {
        public string ClientId { get; set; }

        public string AuthorityServerUrl { get; set; }

        public string RedirectUrl { get; set; }

        public string ResponseType { get; set; }

        public string ForbiddenRoute { get; set; }

        public string StartupRoute { get; set; }

        public string UnauthorizedRoute { get; set; }

        public bool AuthorityServerActif { get; set; }

        public string PostLogoutRedirectUri { get; set; }

        public bool SilentRenew { get; set; }

        public bool StartCheckSession { get; set; }

        public bool LogConsoleDebugActive { get; set; }

        public bool LogConsoleWarningActive { get; set; }

        public string MaxIdTokenIatOffsetAllowedInSeconds { get; set; }

        public IEnumerable<string> Scopes { get; set; }
        public string ApiName { get; set; }
        public string ApiSecret { get; set; }
    }
}
