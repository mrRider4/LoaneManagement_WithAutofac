namespace LoanManagement.Services.RequestedLoans.Contracts;

public class GetClosedRequestedLoanDto
{
    public int  Id { get; set; }
    public decimal LoanAmount { get; set; }
    public int InstallmentsCount { get; set; }
    public decimal TotalLatePayedFees { get; set; }
}