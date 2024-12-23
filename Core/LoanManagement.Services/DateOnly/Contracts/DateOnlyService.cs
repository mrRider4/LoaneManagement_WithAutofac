namespace LoanManagement.Services;

public interface DateOnlyService
{
    System.DateOnly NowUtc { get; }
}