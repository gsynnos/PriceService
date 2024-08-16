using PriceService.Models;

namespace PriceService.Options
{
    public class InstrumentsOptions
    {
        public const string Position = "Instruments";

        public Instrument[] SupportedInstruments { get; set; } = Array.Empty<Instrument>();
    }
}
