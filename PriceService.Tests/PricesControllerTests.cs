using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using PriceService.Controllers;
using PriceService.Services;
using Xunit;

namespace PriceService.Tests
{
    public class PricesControllerTests
    {
        private PricesController _controller;
        private IPriceRepository _prices;

        public PricesControllerTests()
        {
            _prices = Substitute.For<IPriceRepository>();
            _controller = new(Substitute.For<ILogger<PricesController>>(), _prices);
        }

        [Fact]
        public void GetCurrentPrice_NotAvailableInstruments_ReturnsNotFound()
        {
            var actual = _controller.GetCurrentPrice("TEST");
            actual.Result.Should().BeOfType<NotFoundObjectResult>();
        }


        [Fact]
        public void GetCurrentPrice_AvailableInstruments_ReturnsPrice()
        {
            _prices.GetPrice("test").Returns(100.5M);

            var actual = _controller.GetCurrentPrice("TEST");
            actual.Result.Should().BeOfType<OkObjectResult>();
            (actual.Result as OkObjectResult)?.Value.Should().Be(100.5M);
        }
    }
}
