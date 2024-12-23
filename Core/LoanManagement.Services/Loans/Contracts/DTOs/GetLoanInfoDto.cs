namespace LoanManagement.Services.Loans.Contracts;

public class GetLoanInfoDto
{
    public decimal Amount { get; set; }
    public decimal AnnualInterestPercentage { get; set; }
    public int InstallmentCount { get; set; }
    public decimal InstallmentAmount { get; set; }
    public decimal LatePaymentFee { get; set; }
}