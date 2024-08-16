using PriceService.Services;
using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;

namespace PriceService.Middleware
{
    [ExcludeFromCodeCoverage]
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebSocketConnectionManager _connectionManager;

        public WebSocketMiddleware(RequestDelegate next, IWebSocketConnectionManager connectionManager)
        {
            _next = next;
            _connectionManager = connectionManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var instrument = context.Request.Query["instrument"];

                _connectionManager.Subscribe(instrument, webSocket);

                while (webSocket.State == WebSocketState.Open)
                {
                    var buffer = new byte[1024 * 4];
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", CancellationToken.None);
                    }
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
