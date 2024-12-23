namespace LoanManagement.Services.RequestedLoans.Contracts.DTOs;

public class GetCustomerInformationDto
{
    public bool IsVerified { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required  string NationalCode { get; set; }
    public int? FinancialInformationId { get; set; }
    public JobType? JobType { get; set; }
    public decimal? MonthlyIncome { get; set; }
    public decimal? FinancialAssets { get; set; }

}