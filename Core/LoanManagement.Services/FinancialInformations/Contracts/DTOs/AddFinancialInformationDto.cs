namespace LoanManagement.Services.FinancialInformations.Contracts;

public class AddFinancialInformationDto
{
    public JobType JobType { get; set; }

    [Range(0, (double)decimal.MaxValue)]
    public decimal MonthlyIncome { get; set; }

    [Range(0, (double)decimal.MaxValue)]
    public decimal FinancialAssets { get; set; }
}