using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Raygun.Druid4Net;

namespace BigBank.OLAP.Serializers
{
    internal class NewtonsoftSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter> { new StringEnumConverter() }
        };

        public string Serialize<TRequest>(TRequest request)
        {
            return JsonConvert.SerializeObject(request, _settings);
        }

        public TResponse Deserialize<TResponse>(string responseData)
        {
            return JsonConvert.DeserializeObject<TResponse>(responseData, _settings);
        }
    }
}