using System;
using System.IO;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace BigBank.IntegrationTests.Core
{
    public class TestFixture : IDisposable
    {
        public HttpClient HttpClient { get; private set; }

        public TestFixture()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables()
                .Build();

            HttpClient = new HttpClient() { BaseAddress = new Uri(config["BIG_BANK_URL"]) };
        }

        public void Dispose()
        {
            HttpClient?.Dispose();
            HttpClient = null;
        }
    }
}