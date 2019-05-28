namespace BigBank.IntegrationTests.Core
{
    internal static class DecimalExtensions
    {
        public static decimal RoundTo(this decimal value, int decimals)
        {
            return System.Math.Round(value, decimals);
        }
    }
}