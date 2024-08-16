using PriceService.Models;

namespace PriceService.Services
{
    public class TiingoCryptoWebSocketClient : TiingoWebSocketClient
    {
        public TiingoCryptoWebSocketClient(IPriceRepository priceRepository,
                                           ILogger<TiingoWebSocketClient> logger,
                                           IWebSocketConnectionManager connectionManager) : base(priceRepository, logger, connectionManager)
        {
        }

        public override AssetClass SupportedAssetClass => AssetClass.Crypto;

        protected override int ClosingPriceMessageIndex => 5;

        protected override Uri GetUri()
        {
            return new Uri("wss://api.tiingo.com/crypto");
        }
    }
}
