namespace LoanManagement.Applications.Customers.Handler.UpdateCustomerFinancialInformation.Contracts.Commands;

public class UpdateFinancialInformationCommand
{
    public int CustomerId { get; set; }
    public JobType JobType { get; set; }
    public decimal MonthlyIncome { get; set; }
    public decimal FinancialAssets { get; set; }
}