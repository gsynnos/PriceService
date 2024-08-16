namespace PriceService.Services
{
    public interface IPriceRepository
    {
        decimal? GetPrice(string instrument);
        void UpdatePrice(string instrument, decimal price);
    }
}
