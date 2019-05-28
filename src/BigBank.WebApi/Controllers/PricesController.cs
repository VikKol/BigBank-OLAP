using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using BigBank.OLAP;
using BigBank.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BigBank.Controllers
{
    /// <summary>
    /// Price calculation API
    /// </summary>
    [Route("api/prices")]
    public class PricesController : ControllerBase
    {
        private readonly IPriceCalculationService _priceCalculationService;

        public PricesController(IPriceCalculationService priceCalculationService)
        {
            _priceCalculationService = priceCalculationService ?? throw new ArgumentNullException(nameof(priceCalculationService));
        }

        /// <summary>
        /// Calculates average price by specified parameters and timeslot of the <paramref name="date"/>.
        /// </summary>
        /// <param name="portfolio">Portfolio name (Optional)</param>
        /// <param name="owner">Owner name (Optional)</param>
        /// <param name="instrument">Instrument name (Optional)</param>
        /// <param name="date">DateTime of the timeslot</param>
        /// <returns>Average price value</returns>
        [HttpGet("average")]
        [ProducesResponseType(200, Type = typeof(AveragePriceResponse))]
        public async Task<IActionResult> GetAveragePrice(
            [FromQuery, MinLength(1)]string portfolio,
            [FromQuery, MinLength(1)]string owner,
            [FromQuery, MinLength(1)]string instrument,
            [FromQuery, Required]DateTime date)
        {
            var result = await _priceCalculationService.CalculateAveragePrice(date, portfolio, owner, instrument);
            if (result != null)
            {
                return Ok(new AveragePriceResponse
                {
                    Date = result.Date,
                    Price = result.Price
                });
            }

            return NotFound();
        }
    }
}