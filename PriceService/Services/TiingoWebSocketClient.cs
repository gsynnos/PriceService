using Newtonsoft.Json;
using PriceService.Models;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace PriceService.Services
{
    public abstract class TiingoWebSocketClient : IWebSocketPriceProvider
    {
        private readonly ILogger<TiingoWebSocketClient> _logger;
        private readonly IPriceRepository _priceRepository;
        private readonly ClientWebSocket _webSocket = new ClientWebSocket();
        private readonly IWebSocketConnectionManager _connectionManager;
        private readonly string ApiToken;

        protected TiingoWebSocketClient(IPriceRepository priceRepository,
                                        ILogger<TiingoWebSocketClient> logger,
                                        IWebSocketConnectionManager connectionManager)
        {
            ApiToken = Environment.GetEnvironmentVariable("TIINGO_API_KEY") ?? string.Empty;
            _priceRepository = priceRepository;
            _logger = logger;
            _connectionManager = connectionManager;
        }

        protected abstract Uri GetUri();

        protected abstract int ClosingPriceMessageIndex { get; }

        public abstract AssetClass SupportedAssetClass { get; }

        public async Task ConnectAsync(string[] instruments)
        {
            var uri = GetUri();

            try
            {
                _logger.LogInformation("Connecting to {URI}", uri);
                _webSocket.Options.SetRequestHeader("Authorization", $"Token {ApiToken}");
                await _webSocket.ConnectAsync(GetUri(), CancellationToken.None);

                await SubscribeToInstruments(instruments);
                await ReceiveMessages();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection to {URI} failed!", uri);
            }
        }

        private async Task SubscribeToInstruments(string[] instruments)
        {
            _logger.LogInformation("Subscribing to {Instruments}", instruments);
            var message = new
            {
                eventName = "subscribe",
                authorization = ApiToken,
                eventData = new
                {
                    tickers = instruments
                }
            };

            string subscribeMessage = JsonConvert.SerializeObject(message);
            var bytesToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(subscribeMessage));
            await _webSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        private async Task ReceiveMessages()
        {
            var buffer = new byte[1024 * 4];

            while (_webSocket.State == WebSocketState.Open)
            {
                var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                _logger.LogInformation("Received: {Message}", message);

                var jsonDocument = JsonDocument.Parse(message);
                var root = jsonDocument.RootElement;

                // Check if the messageType is "A" (price update)
                // Ideally we should also handle the status messages and the heartbeats, but for the
                // scope of this exercise they can be ignored
                if (root.GetProperty("messageType").GetString() == "A")
                {
                    // Extract the instrument name and price
                    var data = root.GetProperty("data").EnumerateArray().ToArray();
                    var instrument = data[1].GetString();
                    var closingPrice = data[ClosingPriceMessageIndex].GetDecimal();

                    _logger.LogInformation("Instrument: {Instrument}, Closing Price: {ClosingPrice}", instrument, closingPrice);

                    if (!string.IsNullOrEmpty(instrument))
                    {
                        _priceRepository.UpdatePrice(instrument, closingPrice);

                        string priceUpdate = $"Instrument: {instrument}, Closing Price: {closingPrice}";
                        await _connectionManager.BroadcastPriceUpdate(instrument, priceUpdate);
                    }
                }
            }
        }

        public async Task DisconnectAsync()
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
            _webSocket.Dispose();
        }
    }
}
