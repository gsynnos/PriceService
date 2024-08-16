using System.Net.WebSockets;

namespace PriceService.Services
{
    public interface IWebSocketConnectionManager
    {
        Task BroadcastPriceUpdate(string instrument, string priceUpdate);
        void Subscribe(string instrument, WebSocket webSocket);
    }
}
