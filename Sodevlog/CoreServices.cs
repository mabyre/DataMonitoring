using System;
using System.Collections.Generic;
using System.Text;

namespace Sodevlog.CoreServices
{
    public interface ILocalizationService
    {
        string GetLocalizedHtmlString(string text);
    }

    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    public class LocalizationService : ILocalizationService
    {
        string ILocalizationService.GetLocalizedHtmlString(string text)
        {
            // TODO : return something else but text
            return text;
        }
    }
}
