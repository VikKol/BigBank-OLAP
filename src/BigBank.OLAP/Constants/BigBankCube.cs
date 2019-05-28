namespace BigBank.OLAP.Constants
{
    public static class BigBankCube
    {
        public static string DataSource = "BigBank";

        public static class Dimensions
        {
            public static string Portfolio = nameof(Portfolio);
            public static string Owner = nameof(Owner);
            public static string Instrument = nameof(Instrument);
            public static string Date = nameof(Date);
        }

        public static class Metrics
        {
            public static string Count = nameof(Count);
            public static string Price = nameof(Price);
            public static string AveragePrice = nameof(AveragePrice);
        }
    }
}