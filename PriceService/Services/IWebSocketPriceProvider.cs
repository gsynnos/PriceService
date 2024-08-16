using PriceService.Models;

namespace PriceService.Services
{
    public interface IWebSocketPriceProvider
    {
        AssetClass SupportedAssetClass { get; }

        Task ConnectAsync(string[] instruments);
    }
}
