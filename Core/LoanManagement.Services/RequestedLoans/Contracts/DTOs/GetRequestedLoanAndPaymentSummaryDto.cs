namespace LoanManagement.Services.RequestedLoans.Contracts;

public class GetRequestedLoanAndPaymentSummaryDto
{
    public int Id { get; set; }
    public int LoanId { get; set; }
    public int FinancialInformationId { get; set; }
    public System.DateOnly RequestedDate { get; set; }
    public int CreditRate { get; set; }
    public RequestedLoanStatus RequestedLoanStatus { get; set; }
    public decimal LoanAmount { get; set; }
    public int InstallmentCount { get; set; }
    public decimal PayedAmount { get; set; }
    public HashSet<GetInstallmentDto> RemainingInstallments { get; set; } = [];
}