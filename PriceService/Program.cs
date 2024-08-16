using PriceService.Middleware;
using PriceService.Options;
using PriceService.Services;
using System.Text.Json.Serialization;

namespace PriceService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<InstrumentsOptions>(builder.Configuration.GetSection(InstrumentsOptions.Position));
            builder.Services.AddSingleton<IPriceRepository, PriceRepository>();
            builder.Services.AddSingleton<IPriceProviderService, PriceProviderService>();
            builder.Services.AddSingleton<IWebSocketPriceProvider, TiingoFxWebSocketClient>();
            builder.Services.AddSingleton<IWebSocketPriceProvider, TiingoCryptoWebSocketClient>();
            builder.Services.AddSingleton<IWebSocketConnectionManager, WebSocketConnectionManager>(); 
            builder.Services.AddHostedService<PriceSubscriptionService>();

            builder.Services.AddControllers()
                 .AddJsonOptions(options =>
                 {
                     options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                 });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseWebSockets();
            app.UseMiddleware<WebSocketMiddleware>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
