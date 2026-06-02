namespace SpeedyShop.Api.Services;

public interface IReportWorkshopService
{
    Task<string> BuildAllocationHeavyReportAsync();
}