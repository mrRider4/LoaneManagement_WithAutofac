namespace LoanManagement.Services.DateOnly;

public class DateOnlyAppService : DateOnlyService
{
    public System.DateOnly NowUtc
    {
        get { return System.DateOnly.FromDateTime(DateTime.UtcNow); }
    }
}