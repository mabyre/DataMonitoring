using System;
using System.Collections.Generic;
using Sodevlog.CoreServices;

namespace DataMonitoring
{
    public static class EnumExtension
    {

        public static List<EnumValue> GetValues<T>(ILocalizationService localizationService = null) where T : struct
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("GetValues<" + typeof(T).Name + "> can only be called for types derived from System.Enum");
            }

            var list = new List<EnumValue>();

            foreach (var item in Enum.GetValues(typeof(T)))
            {
                list.Add(new EnumValue
                {
                    Value = (int)item,
                    Name = localizationService != null 
                        ? localizationService.GetLocalizedHtmlString(Enum.GetName(typeof(T), item))
                        : Enum.GetName(typeof(T), item)

                });
            }

            return list;
        }
    }

    public class EnumValue
    {
        public int Value { get; set; }
        public string Name { get; set; }
    }
}
