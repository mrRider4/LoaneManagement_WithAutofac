namespace LoanManagement.Services;

public interface DateOnlyService : Service
{
    System.DateOnly NowUtc { get; }
}