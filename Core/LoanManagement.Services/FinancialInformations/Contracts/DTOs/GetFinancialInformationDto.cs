namespace LoanManagement.Services.Customers.Contracts;

public class GetFinancialInformationDto
{
    public JobType JobType { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal FinancialAssets { get; set; }
}