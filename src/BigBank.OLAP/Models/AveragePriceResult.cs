using System;

namespace BigBank.OLAP.Models
{
    public class AveragePriceResult
    {
        /// <summary>
        /// Start of the price timeslot
        /// </summary>
        public DateTime Date { get; }
        
        /// <summary>
        /// Aggregated average price
        /// </summary>
        public decimal Price { get; }

        public AveragePriceResult(DateTime date, decimal price)
        {
            Date = date;
            Price = price;
        }
    }
}