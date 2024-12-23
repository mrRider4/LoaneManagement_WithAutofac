namespace LoanManagement.Entities.RequestedLoans;

public class RequestedLoan
{
    public int Id { get; set; }
    public int LoanId { get; set; }
    public int FinancialInformationId { get; set; }
    public DateOnly RequestedDate { get; set; }
    [Range(1, 100)] public int CreditRate { get; set; }
    public RequestedLoanStatus RequestedLoanStatus { get; set; }
}