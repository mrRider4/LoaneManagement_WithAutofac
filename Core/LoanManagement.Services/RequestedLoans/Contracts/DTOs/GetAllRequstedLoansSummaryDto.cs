namespace LoanManagement.Services.RequestedLoans.Contracts;

public class GetAllRequstedLoansSummaryDto
{
    public int TotalAllOnTimeClosedLoansCount { get; set; }
    public int TotalDelayedInstallmentsCount { get; set; }
}