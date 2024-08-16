using System.Collections.Concurrent;

namespace PriceService.Services
{
    public class PriceRepository : IPriceRepository
    {
        private readonly ConcurrentDictionary<string, decimal> _prices = new();

        public void UpdatePrice(string instrument, decimal price)
        {

            instrument = instrument.ToLower();
            _prices.AddOrUpdate(instrument, price, (key, oldValue) => price);
        }

        public decimal? GetPrice(string instrument)
        {
            instrument = instrument.ToLower();
            if (_prices.TryGetValue(instrument, out decimal price))
            {
                return price;
            }
            return null;
        }
    }
}
