using System;

namespace BigBank.IntegrationTests.Data
{
    internal class PriceRecordDimensions
    {
        public string Portfolio { get; set; }
        public string Owner { get; set; }
        public string Instrument { get; set; }
        public DateTime Date { get; set; }
    }
}