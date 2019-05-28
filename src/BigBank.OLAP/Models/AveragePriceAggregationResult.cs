namespace BigBank.OLAP.Models
{
    internal class AveragePriceAggregationResult
    {
        /// <summary>
        /// Aggregated price records count
        /// </summary>
        public long Count { get; set; }

        /// <summary>
        /// Aggregated sum of price records
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Aggregated average price
        /// </summary>
        public decimal AveragePrice { get; set; }
    }
}