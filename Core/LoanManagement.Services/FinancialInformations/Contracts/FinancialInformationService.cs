namespace LoanManagement.Services.FinancialInformations.Contracts;

public interface FinancialInformationService
{
    Task<int> CreateByCustomerId(int customerId,
        AddFinancialInformationDto dto);

    Task<int> UpdateByCustomerId(int customerId,
        UpdateFinancialInformationDto dto);
}