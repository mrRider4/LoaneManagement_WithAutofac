namespace LoanManagement.Services.Loans.Contracts.DTOs;

public class AddLoanDto
{
    [Range(1, (double)decimal.MaxValue)]
    [DefaultValue(0)]
    public decimal Amount { get; set; }

    [Range(1, Int32.MaxValue)]
    [DefaultValue(0)]
    public int InstallmentCount { get; set; }
}