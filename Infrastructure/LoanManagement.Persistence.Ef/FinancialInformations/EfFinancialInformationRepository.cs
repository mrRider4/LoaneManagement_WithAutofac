namespace LoanManagement.Persistence.Ef.FinancialInformations;

public class EfFinancialInformationRepository(EfDataContext context)
    : FinancialInformationRepository
{
    public async Task<bool> IsExistByCustomerIdAndIsDeletedFilter(
        int customerId, bool isDeleted = false)
    {
        return await context.Set<FinancialInformation>()
            .AnyAsync(f => f.CustomerId == customerId &&
                           f.IsDeleted == isDeleted);
    }

    public async Task Add(FinancialInformation financialInformation)
    {
        await context.Set<FinancialInformation>()
            .AddAsync(financialInformation);
    }

    public async Task<FinancialInformation?> FindLastOneByCustomerId(
        int customerId)
    {
        return await context.Set<FinancialInformation>()
            .Where(f => f.CustomerId == customerId && f.IsDeleted == false)
            .FirstAsync();
    }
}