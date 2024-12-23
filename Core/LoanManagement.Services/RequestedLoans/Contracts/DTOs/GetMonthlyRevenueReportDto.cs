namespace LoanManagement.Services.RequestedLoans.Contracts;

public class GetMonthlyRevenueReportDto
{
    public System.DateOnly FromDate { get; set; }
    public System.DateOnly ToDate { get; set; }
    public decimal TotalInterestRevenue { get; set; }
    public decimal TotalLatePaymentFeeRevenue { get; set; }
}