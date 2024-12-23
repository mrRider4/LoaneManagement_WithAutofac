namespace LoanManagement.Services.RequestedLoans.Contracts;

public class GetRequestedLoanDto
{
    public int Id { get; set; }
    public int LoanId { get; set; }
    public int FinancialInformationId { get; set; }
    public System.DateOnly RequestedDate { get; set; }
    public int CreditRate { get; set; }
    public RequestedLoanStatus RequestedLoanStatus { get; set; }
}