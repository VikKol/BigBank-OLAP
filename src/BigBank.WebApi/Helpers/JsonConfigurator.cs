using System.Globalization;
using BigBank.OLAP.Constants;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BigBank.WebApi.Helpers
{
    public static class JsonConfigurator
    {
        public static JsonSerializerSettings ApplySerializerSettings(JsonSerializerSettings settings)
        {
            settings.Culture = CultureInfo.InvariantCulture;
            settings.DateFormatString = BigBankFormats.DateTime;
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            return settings;
        }
    }
}