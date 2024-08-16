using Microsoft.AspNetCore.Mvc;
using PriceService.Services;

namespace PriceService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PricesController : ControllerBase
    {
        private readonly ILogger<PricesController> _logger;
        private readonly IPriceRepository _priceRepository;

        public PricesController(ILogger<PricesController> logger, 
                                IPriceRepository priceRepository)
        {
            _logger = logger;
            _priceRepository = priceRepository;
        }

        [HttpGet("getCurrentPrice")]
        public ActionResult<decimal> GetCurrentPrice(string instrument)
        {
            instrument = instrument.ToLower();
            _logger.LogInformation($"{nameof(GetCurrentPrice)} requested for {instrument}");

            var currentInstrumentPrice = _priceRepository.GetPrice(instrument);
            if(!currentInstrumentPrice.HasValue)
            {
                var message = $"Latest price for {instrument} instrument not found!";
                _logger.LogWarning(message);
                return NotFound(message);
            }

            return Ok(currentInstrumentPrice.Value);
        }
    }
}
