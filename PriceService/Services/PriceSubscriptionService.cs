namespace PriceService.Services
{
    public class PriceSubscriptionService : BackgroundService
    {
        private readonly IPriceProviderService _priceProviderService;

        public PriceSubscriptionService(IPriceProviderService priceProviderService)
        {
            _priceProviderService = priceProviderService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _priceProviderService.SubscribeToInstruments();
        }
    }

}
