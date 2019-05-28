using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using BigBank.OLAP.Constants;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BigBank.WebApi.Helpers
{
    public class BigBankDataSourceHealthChecker
    {
        private static readonly TimeSpan _defaultDelay = TimeSpan.FromSeconds(10);

        private readonly Uri _druidCoordinatorUri;
        private readonly ILogger _logger;

        private int _isHealthy = 0;

        public BigBankDataSourceHealthChecker(Uri druidCoordinatorUri, ILoggerFactory loggerFactory)
        {
            _druidCoordinatorUri = druidCoordinatorUri;
            _logger = loggerFactory.CreateLogger<BigBankDataSourceHealthChecker>();
        }

        public bool IsHealthy() => _isHealthy == 1;

        public async Task WaitUntilDataSourceIsAvailableAsync()
        {
            _logger.LogInformation($"Waiting until '{BigBankCube.DataSource}' data are loaded.");

            using (var httpClient = new HttpClient())
            {
                bool mustRepeat = false;
                double loadPercentage = 0.0;
                do
                {
                    var dataSources = await GetDataSources(httpClient);
                    if (dataSources.TryGetValue(BigBankCube.DataSource, out var value))
                    {
                        loadPercentage = double.Parse(value, NumberStyles.Any, CultureInfo.InvariantCulture);

                        _logger.LogInformation($"'{BigBankCube.DataSource}' loading progress: {loadPercentage}%");
                    }

                    mustRepeat = loadPercentage < 100;
                    if (mustRepeat)
                    {
                        await Task.Delay(_defaultDelay);
                    }
                    else
                    {
                        Interlocked.Exchange(ref _isHealthy, 1);
                    }
                }
                while (mustRepeat);
            }
        }

        private async Task<IReadOnlyDictionary<string, string>> GetDataSources(HttpClient httpClient, int attempts = 10)
        {
            try
            {
                var response = await httpClient.GetAsync(new Uri(_druidCoordinatorUri, "/druid/coordinator/v1/loadstatus"));
                var responseContent = await response.Content.ReadAsStringAsync();

                return JsonConvert.DeserializeObject<Dictionary<string, string>>(responseContent);
            }
            catch(HttpRequestException ex) when(ex.GetBaseException() is SocketException)
            {
                if (attempts > 0)
                {
                    _logger.LogWarning($"Request to Druid Coordinator failed with SocketException. Retry in {_defaultDelay.TotalSeconds} seconds...");

                    await Task.Delay(_defaultDelay);

                    return await GetDataSources(httpClient, attempts - 1);
                }

                throw;
            }
        }
    }
}