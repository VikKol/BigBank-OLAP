using BigBank.OLAP.Models;

namespace BigBank.IntegrationTests.Data
{
    internal class PriceRecord : PriceRecordDimensions
    {
        public TimeSlot TimeSlot { get; set; }
        public decimal Price { get; set; }
    }
}