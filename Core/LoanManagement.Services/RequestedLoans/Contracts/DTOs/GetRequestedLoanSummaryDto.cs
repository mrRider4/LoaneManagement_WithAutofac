namespace LoanManagement.Services.RequestedLoans.Contracts;

public class GetRequestedLoanSummaryDto
{
    public int LoanId { get; set; }
    public int FinancialInformationId { get; set; }
    public RequestedLoanStatus RequestedLoanStatus { get; set; }
}