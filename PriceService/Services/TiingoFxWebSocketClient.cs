using PriceService.Models;

namespace PriceService.Services
{
    public class TiingoFxWebSocketClient : TiingoWebSocketClient
    {
        public TiingoFxWebSocketClient(IPriceRepository priceRepository, 
                                       ILogger<TiingoWebSocketClient> logger,
                                       IWebSocketConnectionManager connectionManager) : base(priceRepository, logger, connectionManager)
        {
        }

        public override AssetClass SupportedAssetClass => AssetClass.FX;

        protected override int ClosingPriceMessageIndex => 4;

        protected override Uri GetUri()
        {
            return new Uri("wss://api.tiingo.com/fx");
        }
    }
}
