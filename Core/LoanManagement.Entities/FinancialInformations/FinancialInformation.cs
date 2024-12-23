using LoanManagement.Entities.FinancialInformations.Enums;

namespace LoanManagement.Entities.FinancialInformations;

public class FinancialInformation
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public JobType JobType { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal FinancialAssets { get; set; }
    public bool IsDeleted { get; set; }
}