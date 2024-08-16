using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using PriceService.Controllers;
using PriceService.Models;
using PriceService.Options;
using PriceService.Services;
using Xunit;

namespace PriceService.Tests
{
    public class InstrumentsControllerTests
    {
        private InstrumentsController _controller;
        private InstrumentsOptions _options;

        public InstrumentsControllerTests()
        {
            _options = new InstrumentsOptions()
            {
                SupportedInstruments = new[]
                {
                    new Instrument
                    {
                        AssetClass = AssetClass.FX,
                        InstrumentCode = "EURUSD"
                    },
                    new Instrument
                    {
                        AssetClass = AssetClass.Crypto,
                        InstrumentCode = "BTCUSD"
                    }
                }
            };

            var options = Substitute.For<IOptions<InstrumentsOptions>>();
            options.Value.Returns(_options);

            _controller = new(Substitute.For<ILogger<InstrumentsController>>(), options);
        }

        [Fact]
        public void GetInstruments_NoInstruments_ReturnsNotFound()
        {
            _options = new InstrumentsOptions();
            var options = Substitute.For<IOptions<InstrumentsOptions>>();
            options.Value.Returns(_options);

            _controller = new(Substitute.For<ILogger<InstrumentsController>>(), options);

            var actual = _controller.GetInstruments();
            actual.Result.Should().BeOfType<NotFoundObjectResult>();
        }


        [Fact]
        public void GetInstruments_WithInstruments_ReturnsOkAndInstruments()
        {
            var actual = _controller.GetInstruments();
            actual.Result.Should().BeOfType<OkObjectResult>();
            (actual.Result as OkObjectResult)?.Value.Should().BeEquivalentTo(new[] { "EURUSD", "BTCUSD" });
        }
    }
}
