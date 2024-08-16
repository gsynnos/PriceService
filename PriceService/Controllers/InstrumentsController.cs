using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PriceService.Options;

namespace PriceService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InstrumentsController : ControllerBase
    {
        private readonly ILogger<InstrumentsController> _logger;
        private readonly IOptions<InstrumentsOptions> _options;

        public InstrumentsController(ILogger<InstrumentsController> logger, 
                                     IOptions<InstrumentsOptions> options)
        {
            _logger = logger;
            _options = options;
        }

        [HttpGet("getInstruments")]
        public ActionResult<IReadOnlyCollection<string>> GetInstruments()
        {
            /// For the purposes of this exercise the list of instruments is included
            /// in the appsettings.json file.
            /// Ideally in a production environment these would be fetched from a database

            _logger.LogInformation($"{nameof(GetInstruments)} requested");

            var instruments = _options.Value.SupportedInstruments.Select(i=>i.InstrumentCode).ToArray();
            if (instruments == null || instruments.Length == 0)
            {
                _logger.LogWarning("No instruments found in configuration");
                return NotFound("No supported instruments configured.");
            }

            _logger.LogInformation("Returning {Instruments}", instruments);
            return Ok(instruments);
        }
    }
}
