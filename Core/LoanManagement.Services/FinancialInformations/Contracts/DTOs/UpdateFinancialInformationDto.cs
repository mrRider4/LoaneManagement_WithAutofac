namespace LoanManagement.Services.FinancialInformations.Contracts;

public class UpdateFinancialInformationDto
{
    public JobType? JobType { get; set; } = null;

    [Range(0, (double)decimal.MaxValue)]
    public decimal? MonthlyIncome { get; set; } = null;

    [Range(0, (double)decimal.MaxValue)]
    public decimal? FinancialAssets { get; set; } = null;
}