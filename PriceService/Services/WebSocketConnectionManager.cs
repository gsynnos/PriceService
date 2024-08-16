using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace PriceService.Services
{
    public class WebSocketConnectionManager : IWebSocketConnectionManager
    {
        private readonly ConcurrentDictionary<string, List<WebSocket>> _subscriptions = new();

        public void Subscribe(string instrument, WebSocket webSocket)
        {
            instrument = instrument.ToLower();
            if (!_subscriptions.ContainsKey(instrument))
            {
                _subscriptions[instrument] = new List<WebSocket>();
            }
            _subscriptions[instrument].Add(webSocket);
        }

        public async Task BroadcastPriceUpdate(string instrument, string priceUpdate)
        {
            instrument = instrument.ToLower();
            if (_subscriptions.ContainsKey(instrument))
            {
                var connections = _subscriptions[instrument];
                foreach (var socket in connections.ToList()) 
                {
                    if (socket.State == WebSocketState.Open)
                    {
                        var buffer = Encoding.UTF8.GetBytes(priceUpdate);
                        await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else
                    {
                        connections.Remove(socket); 
                    }
                }
            }
        }
    }

}
