using System;

namespace BigBank.WebApi.Models
{
    public class AveragePriceResponse
    {
        /// <summary>
        /// Beginning of the timeslot
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Calculated average price
        /// </summary>
        public decimal Price { get; set; }
    }
}