using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BigBank.OLAP.Constants;
using BigBank.OLAP.Models;
using Raygun.Druid4Net;

namespace BigBank.OLAP
{
    internal class PriceCalculationService : IPriceCalculationService
    {
        private readonly IDruidClient _druidClient;

        public PriceCalculationService(IDruidClient druidClient)
        {
            _druidClient = druidClient ?? throw new ArgumentNullException(nameof(druidClient));
        }

        /// <summary>
        /// Calculates average price by specified dimensions and datetime of the timeslot.
        /// </summary>
        /// <returns>
        /// <see cref="AveragePriceResult"/> in case Price records found by specified params.
        /// Otherwise <code>null<code/>.
        /// </returns>
        public async Task<AveragePriceResult> CalculateAveragePrice(
            DateTime dateTime,
            string portfolioName = null,
            string instrumentOwnerName = null,
            string instrumentName = null)
        {
            if (dateTime < TimeSlot.Origin)
                return null;

            var timeslot = TimeSlot.DateTimeToTimeSlot(dateTime);
            var filter = BuildFilter(portfolioName, instrumentOwnerName, instrumentName);
            var dimensions = SelectDimensions(portfolioName, instrumentOwnerName, instrumentName);

            var response = await _druidClient.GroupByAsync<AveragePriceAggregationResult>(
                aggr => ConfigureAggregationSpec(aggr, timeslot, filter, dimensions));

            if (response.Data.Any())
            {
                var aggregationResult = response.Data.Single();
                return new AveragePriceResult(aggregationResult.Timestamp, aggregationResult.Event.AveragePrice);
            }

            return null;
        }

        private static IGroupByQueryDescriptor ConfigureAggregationSpec(
            IGroupByQueryDescriptor aggregation,
            TimeSlot timeslot,
            IFilterSpec filter,
            IReadOnlyList<string> dimensions)
        {
            if (filter != null)
            {
                aggregation = aggregation.Filter(filter);
            }

            if (dimensions.Any())
            {
                aggregation = aggregation.Dimensions(dimensions);
            }

            return aggregation
                .DataSource(BigBankCube.DataSource)
                .Granularity(Granularities.All)
                .Interval(timeslot.Start, timeslot.End)
                .Aggregations(
                    new DoubleSumAggregator(BigBankCube.Metrics.Price, BigBankCube.Metrics.Price),
                    new LongSumAggregator(BigBankCube.Metrics.Count, BigBankCube.Metrics.Count))
                .PostAggregations(new ArithmeticPostAggregator(
                    BigBankCube.Metrics.AveragePrice,
                    ArithmeticFunction.Divide,
                    fields: new IPostAggregationSpec[]
                    {
                        new FieldAccessPostAggregator(BigBankCube.Metrics.Price, BigBankCube.Metrics.Price),
                        new FieldAccessPostAggregator(BigBankCube.Metrics.Count, BigBankCube.Metrics.Count)
                    }));
        }

        private IReadOnlyList<string> SelectDimensions(string portfolioName, string instrumentOwnerName, string instrumentName)
        {
            var dimensions = new List<string>(3);

            if (portfolioName != null)
            {
                dimensions.Add(BigBankCube.Dimensions.Portfolio);
            }

            if (instrumentOwnerName != null)
            {
                dimensions.Add(BigBankCube.Dimensions.Owner);
            }

            if (instrumentName != null)
            {
                dimensions.Add(BigBankCube.Dimensions.Instrument);
            }

            return dimensions;
        }

        private IFilterSpec BuildFilter(string portfolioName, string instrumentOwnerName, string instrumentName)
        {
            var filters = new List<IFilterSpec>(3);

            if (portfolioName != null)
            {
                filters.Add(new SelectorFilter(BigBankCube.Dimensions.Portfolio, portfolioName));
            }

            if (instrumentOwnerName != null)
            {
                filters.Add(new SelectorFilter(BigBankCube.Dimensions.Owner, instrumentOwnerName));
            }

            if (instrumentName != null)
            {
                filters.Add(new SelectorFilter(BigBankCube.Dimensions.Instrument, instrumentName));
            }

            if (filters.Count == 0)
                return null;

            if (filters.Count == 1)
                return filters.Single();

            return new AndFilter(filters);
        }
    }
}