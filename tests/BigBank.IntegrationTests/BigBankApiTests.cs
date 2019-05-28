using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BigBank.IntegrationTests.Core;
using BigBank.IntegrationTests.Data;
using BigBank.OLAP.Models;
using FluentAssertions;
using Xunit;

namespace BigBank.IntegrationTests
{
    [Collection(nameof(TestCollection))]
    public class BigBankApiTests
    {
        private readonly HttpClient _httpClient;

        public BigBankApiTests(TestFixture testFixture)
        {
            _httpClient = testFixture.HttpClient;
        }

        //|=============================================================================|
        //|  DataSet: Data/test_data.csv                                                |
        //|=============================================================================|
        //|  Portfolio          | Owner     | Instrument | Date                | Price  |
        //|=============================================================================|
        const string TestData = @"
          | Barclays            | Ford      | Swap       | 01/01/2018 01:07:21 | 712.54 |
          | State Bank of India | Samsung   | Swap       | 01/01/2018 01:16:57 | 342.66 |
          | Fannie Mae          | Amazon    | Loan       | 01/01/2018 01:36:06 | 117.85 |
          | BNP Paribas         | Google    | Loan       | 01/01/2018 02:07:13 | 117.38 |
          | Deutsche Bank       | Microsoft | Options    | 01/01/2018 02:14:23 | 501.6  |
          | BNP Paribas         | Toyota    | Loan       | 01/01/2018 02:19:01 | 30.39  |
          | Fannie Mae          | Walmart   | Bond       | 01/01/2018 02:19:13 | 32.08  |
          | Deutsche Bank       | Walmart   | Equity     | 01/01/2018 02:26:45 | 244.01 |
          | UOB                 | Samsung   | Options    | 01/01/2018 02:27:56 | 80.44  |
          | Deutsche Bank       | SimCorp   | CDS        | 01/01/2018 02:33:08 | 662.16 |
          | State Bank of India	| Google	| Swap	     | 01/01/2018 02:55:16 | 80.12  |
          | Fannie Mae	        | Apple	    | Bond	     | 01/01/2018 03:01:14 | 68.18  |
          | Barclays            | Amazon    | Futures    | 02/01/2018 04:18:13 | 43.12  |
          | State Bank of India | Apple     | Futures    | 02/01/2018 04:51:05 | 312.1  |
          | Fannie Mae          | Toyota    | Swap       | 02/01/2018 04:58:51 | 262.39 |
          | UOB                 | Google    | Options    | 02/01/2018 05:07:04 | 43.17  |
          | State Bank of India | Ford      | Deposit    | 03/01/2018 03:33:45 | 398.98 |
          | State Bank of India | Google    | Options    | 03/01/2018 03:35:55 | 244.76 |
          | State Bank of India | Walmart   | Equity     | 03/01/2018 03:39:57 | 9.16   |
          | Deutsche Bank       | Google    | Deposit    | 03/01/2018 03:44:03 | 566.89 |
          | Bank of America     | Toyota    | Options    | 03/01/2018 04:01:57 | 175.31 |
          | UOB                 | Apple     | Options    | 04/01/2018 02:28:07 | 604.17 |
          | Finanz Informatik   | Apple     | Loan       | 04/01/2018 02:32:05 | 71.33  |
          | Bank of America     | Amazon    | Equity     | 04/01/2018 02:32:35 | 145.36 |
          | Finanz Informatik   | Walmart   | Bond       | 04/01/2018 02:59:10 | 76.18  |
          | Fannie Mae          | Apple     | Equity     | 04/01/2018 03:05:54 | 80.56  |
          | BNP Paribas         | Amazon    | Futures    | 04/01/2018 03:11:08 | 621.42 |
          | Barclays            | Samsung   | CDS        | 04/01/2018 03:13:30 | 143.76 |
          | Barclays            | Microsoft | Swap       | 04/01/2018 03:13:30 | 768.63 |
          | Deutsche Bank       | Amazon    | Futures    | 04/01/2018 03:21:02 | 146.69 |";

        private static readonly IReadOnlyList<PriceRecord> _testDataRows = TestData
            .Split(new string[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(r =>
            {
                var values = r.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).Select(v => v.Trim(' ')).Where(v => !string.IsNullOrEmpty(v)).ToList();
                var date = DateTime.ParseExact(values[3], "dd/MM/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
                var timeSlot = TimeSlot.DateTimeToTimeSlot(date);
                var price = decimal.Parse(values[4], NumberStyles.Any, CultureInfo.InvariantCulture);
                return new PriceRecord { Portfolio = values[0], Owner = values[1], Instrument = values[2], Date = date, TimeSlot = timeSlot, Price = price };
            })
            .ToList();

        [Fact]
        public async Task GetAveragePriceForOrigin_ShouldBeSuccessful()
        {
            var origin = new DateTime(2018, 1, 1);
            var testRecord = new PriceRecordDimensions { Date = origin };

            var result = await _httpClient.GetAveragePrice(testRecord);

            result.Should().NotBeNull();
            origin.Should().Be(result.Date);
            result.Price.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetAverageBySingleRecord_ReturnsExactPrice()
        {
            var testRecord = _testDataRows.Single(r => r.Portfolio == "Barclays" && r.Owner == "Ford" && r.Instrument == "Swap");
            var expectedDate = TimeSlot.DateTimeToTimeSlot(testRecord.Date).Start;
            var expectedPrice = 712.54M;

            var result = await _httpClient.GetAveragePrice(testRecord);

            result.Should().NotBeNull();
            result.Date.Should().Be(expectedDate);
            result.Price.Should().Be(expectedPrice);
        }

        [Fact]
        public async Task GetAveragePrice_CanCalculateBy_DateOnly()
        {
            var expectedDate = new DateTime(2018, 1, 1);
            var expectedTimeSlot = TimeSlot.DateTimeToTimeSlot(expectedDate);
            var testRecords = _testDataRows.Where(r => r.TimeSlot == expectedTimeSlot).ToList();
            var expectedPrice = testRecords.Select(r => r.Price).Average().RoundTo(decimals: 3);

            var result = await _httpClient.GetAveragePrice(new PriceRecordDimensions { Date = expectedDate });

            result.Should().NotBeNull();
            result.Date.Should().Be(expectedDate);
            result.Price.RoundTo(decimals: 3).Should().Be(expectedPrice);
        }

        [Fact]
        public async Task GetAveragePrice_CanCalculateBy_Portfolio()
        {
            var testRecords = _testDataRows
                .Where(r => r.Portfolio == "State Bank of India")
                .GroupBy(r => r.TimeSlot)
                .OrderByDescending(gr => gr.Count())
                .First();
            var expectedDate = testRecords.Key.Start;
            var expectedPrice = testRecords.Select(r => r.Price).Average().RoundTo(decimals: 3);

            var result = await _httpClient.GetAveragePrice(
                new PriceRecordDimensions
                {
                    Portfolio = testRecords.First().Portfolio,
                    Date = testRecords.First().Date
                });

            result.Should().NotBeNull();
            result.Date.Should().Be(expectedDate);
            result.Price.RoundTo(decimals: 3).Should().Be(expectedPrice);
        }

        [Fact]
        public async Task GetAveragePrice_CanCalculateBy_Owner()
        {
            var testRecords = _testDataRows
                .Where(r => r.Owner == "Google")
                .GroupBy(r => r.TimeSlot)
                .OrderByDescending(gr => gr.Count())
                .First();
            var expectedDate = testRecords.Key.Start;
            var expectedPrice = testRecords.Select(r => r.Price).Average().RoundTo(decimals: 3);

            var result = await _httpClient.GetAveragePrice(
                new PriceRecordDimensions
                {
                    Owner = testRecords.First().Owner,
                    Date = testRecords.First().Date
                });

            result.Should().NotBeNull();
            result.Date.Should().Be(expectedDate);
            result.Price.RoundTo(decimals: 3).Should().Be(expectedPrice);
        }

        [Fact]
        public async Task GetAveragePrice_CanCalculateBy_Instrument()
        {
            var testRecords = _testDataRows
                .Where(r => r.Instrument == "Futures")
                .GroupBy(r => r.TimeSlot)
                .OrderByDescending(gr => gr.Count())
                .First();
            var expectedDate = testRecords.Key.Start;
            var expectedPrice = testRecords.Select(r => r.Price).Average().RoundTo(decimals: 3);

            var result = await _httpClient.GetAveragePrice(
                new PriceRecordDimensions
                {
                    Instrument = testRecords.First().Instrument,
                    Date = testRecords.First().Date
                });

            result.Should().NotBeNull();
            result.Date.Should().Be(expectedDate);
            result.Price.RoundTo(decimals: 3).Should().Be(expectedPrice);
        }

        [Fact]
        public async Task GetAveragePrice_CanCalculateBy_PortfolioAndOwner()
        {
            var testRecords = _testDataRows
                .Where(r => r.Portfolio == "State Bank of India" && r.Owner == "Google")
                .GroupBy(r => r.TimeSlot)
                .OrderByDescending(gr => gr.Count())
                .First();
            var expectedDate = testRecords.Key.Start;
            var expectedPrice = testRecords.Select(r => r.Price).Average().RoundTo(decimals: 3);

            var result = await _httpClient.GetAveragePrice(
                new PriceRecordDimensions
                {
                    Portfolio = testRecords.First().Portfolio,
                    Owner = testRecords.First().Owner,
                    Date = testRecords.First().Date
                });

            result.Should().NotBeNull();
            result.Date.Should().Be(expectedDate);
            result.Price.RoundTo(decimals: 3).Should().Be(expectedPrice);
        }

        [Fact]
        public async Task GetAveragePrice_CanCalculateBy_PortfolioAndInstrument()
        {
            var testRecords = _testDataRows
                .Where(r => r.Portfolio == "BNP Paribas" && r.Instrument == "Loan")
                .GroupBy(r => r.TimeSlot)
                .OrderByDescending(gr => gr.Count())
                .First();
            var expectedDate = testRecords.Key.Start;
            var expectedPrice = testRecords.Select(r => r.Price).Average().RoundTo(decimals: 3);

            var result = await _httpClient.GetAveragePrice(
                 new PriceRecordDimensions
                 {
                     Portfolio = testRecords.First().Portfolio,
                     Instrument = testRecords.First().Instrument,
                     Date = testRecords.First().Date
                 });

            result.Should().NotBeNull();
            result.Date.Should().Be(expectedDate);
            result.Price.RoundTo(decimals: 3).Should().Be(expectedPrice);
        }

        [Fact]
        public async Task GetAveragePrice_CanCalculateBy_OwnerAndInstrument()
        {
            var testRecords = _testDataRows
                .Where(r => r.Owner == "Amazon" && r.Instrument == "Futures")
                .GroupBy(r => r.TimeSlot)
                .OrderByDescending(gr => gr.Count())
                .First();
            var expectedDate = testRecords.Key.Start;
            var expectedPrice = testRecords.Select(r => r.Price).Average().RoundTo(decimals: 3);

            var result = await _httpClient.GetAveragePrice(
                new PriceRecordDimensions
                {
                    Owner = testRecords.First().Owner,
                    Instrument = testRecords.First().Instrument,
                    Date = testRecords.First().Date
                });

            result.Should().NotBeNull();
            result.Date.Should().Be(expectedDate);
            result.Price.RoundTo(decimals: 3).Should().Be(expectedPrice);
        }

        [Fact]
        public async Task GetAveragePrice_CanCalculateBy_PortfolioAndOwnerAndInstrument()
        {
            var testRecords = _testDataRows
                .Where(r => r.Portfolio == "Deutsche Bank" && r.Owner == "Amazon" && r.Instrument == "Futures")
                .GroupBy(r => r.TimeSlot)
                .OrderByDescending(gr => gr.Count())
                .First();
            var expectedDate = testRecords.Key.Start;
            var expectedPrice = testRecords.Select(r => r.Price).Average().RoundTo(decimals: 3);

            var result = await _httpClient.GetAveragePrice(
                new PriceRecordDimensions
                {
                    Portfolio = testRecords.First().Portfolio,
                    Owner = testRecords.First().Owner,
                    Instrument = testRecords.First().Instrument,
                    Date = testRecords.First().Date
                });

            result.Should().NotBeNull();
            result.Date.Should().Be(expectedDate);
            result.Price.RoundTo(decimals: 3).Should().Be(expectedPrice);
        }

        [Fact]
        public async Task GetAveragePrice_ReturnsNotFound_ForInvalidDimensions()
        {
            var testRecord = new PriceRecordDimensions
            {
                Owner = "NotExisting",
                Date = new DateTime(2018, 1, 1)
            };

            var request = testRecord.BuildAveragePriceRequest();
            var response = await _httpClient.SendAsync(request);

            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetAveragePrice_ReturnsNotFound_ForDateEarlierThanOrigin()
        {
            var testRecord = new PriceRecordDimensions
            {
                Date = new DateTime(2017, 1, 1)
            };

            var request = testRecord.BuildAveragePriceRequest();
            var response = await _httpClient.SendAsync(request);

            response.IsSuccessStatusCode.Should().BeFalse();
            response.StatusCode.Should().Be((int)HttpStatusCode.NotFound);
        }
    }
}