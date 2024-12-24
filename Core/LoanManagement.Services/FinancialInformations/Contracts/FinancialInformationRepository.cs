namespace LoanManagement.Services.FinancialInformations.Contracts;

public interface FinancialInformationRepository : Repository
{
    Task<bool> IsExistByCustomerIdAndIsDeletedFilter(int customerId,
        bool isDeleted = false);

    Task Add(FinancialInformation financialInformation);
    Task<FinancialInformation?> FindLastOneByCustomerId(int customerId);
}