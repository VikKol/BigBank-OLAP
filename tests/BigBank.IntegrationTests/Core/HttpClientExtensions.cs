using System;
using System.Net.Http;
using System.Threading.Tasks;
using BigBank.IntegrationTests.Data;
using BigBank.OLAP.Extensions;
using BigBank.WebApi.Helpers;
using BigBank.WebApi.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BigBank.IntegrationTests.Core
{
    internal static class HttpClientExtensions
    {
        private static readonly JsonSerializerSettings _jsonSerializerSettings = JsonConfigurator
            .ApplySerializerSettings(new JsonSerializerSettings());

        public static async Task<AveragePriceResponse> GetAveragePrice(this HttpClient httpClient, PriceRecordDimensions parameters)
        {
            var request = BuildAveragePriceRequest(parameters);
            var response = await httpClient.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            response.IsSuccessStatusCode.Should().BeTrue();

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AveragePriceResponse>(responseContent, _jsonSerializerSettings);
            return result;
        }

        public static HttpRequestMessage BuildAveragePriceRequest(this PriceRecordDimensions parameters)
        {
            var query = BuildQuery(parameters.Date, parameters.Portfolio, parameters.Owner, parameters.Instrument);
            var uri = new Uri($"/api/prices/average{query}", UriKind.Relative);
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            return request;
        }

        private static QueryString BuildQuery(DateTime dateTime, string portfolioName, string instrumentOwnerName, string instrumentName)
        {
            var query = new QueryString()
                .Add("date", dateTime.ToIsoDateTimeString());

            if (portfolioName != null)
            {
                query = query.Add("portfolio", portfolioName);
            }

            if (instrumentOwnerName != null)
            {
                query = query.Add("owner", instrumentOwnerName);
            }

            if (instrumentName != null)
            {
                query = query.Add("instrument", instrumentName);
            }

            return query;
        }
    }
}