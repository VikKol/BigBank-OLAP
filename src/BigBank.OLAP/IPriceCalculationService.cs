using System;
using System.Threading.Tasks;
using BigBank.OLAP.Models;

namespace BigBank.OLAP
{
    public interface IPriceCalculationService
    {
        /// <summary>
        /// Calculates average price by specified dimensions and datetime of the timeslot.
        /// </summary>
        /// <returns>
        /// <see cref="AveragePriceResult"/> in case Price records found by specified params.
        /// Otherwise <code>null<code/>.
        /// </returns>
        Task<AveragePriceResult> CalculateAveragePrice(
            DateTime dateTime,
            string portfolioName = null,
            string instrumentOwnerName = null,
            string instrumentName = null);
    }
}