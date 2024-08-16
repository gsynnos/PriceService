using Microsoft.Extensions.Options;
using PriceService.Options;

namespace PriceService.Services
{
    public class PriceProviderService : IPriceProviderService
    {
        private readonly IReadOnlyCollection<IWebSocketPriceProvider> _priceProviders;
        private readonly IOptions<InstrumentsOptions> _instrumentsOptions;
        private readonly ILogger<PriceProviderService> _logger;
        private List<Task> _priceFetchTasks;

        public PriceProviderService(IEnumerable<IWebSocketPriceProvider> priceProviders,
                                    IOptions<InstrumentsOptions> instrumentsOptions,
                                    ILogger<PriceProviderService> logger)
        {
            _priceFetchTasks = new();
            _priceProviders = priceProviders.ToArray();
            _instrumentsOptions = instrumentsOptions;
            _logger = logger;
        }

        public async Task SubscribeToInstruments()
        {
            var instrumentsGrouppedByAssetClass = _instrumentsOptions.Value.SupportedInstruments.GroupBy(x => x.AssetClass)
                                                                                                .ToDictionary(x => x.Key, x => x.ToArray());

            foreach (var group in instrumentsGrouppedByAssetClass)
            {
                var assetClassPriceProvider = _priceProviders.FirstOrDefault(p => p.SupportedAssetClass == group.Key);
                if (assetClassPriceProvider == null)
                {
                    _logger.LogWarning("Cannot subscribe to the following instruments because a price provider for the asset class {AssetClass} was not found: {Instruments}", group.Key, group.Value);
                    continue;
                }

                _priceFetchTasks.Add(assetClassPriceProvider.ConnectAsync(group.Value.Select(v => v.InstrumentCode.ToLower()).ToArray()));
            }

            await Task.WhenAll(_priceFetchTasks);
        }
    }
}
