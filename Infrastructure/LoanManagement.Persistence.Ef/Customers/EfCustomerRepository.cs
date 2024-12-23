namespace LoanManagement.Persistence.Ef.Customers;

public class EfCustomerRepository(EfDataContext context) : CustomerRepository
{
    public async Task Add(Customer customer)
    {
        await context.Set<Customer>().AddAsync(customer);
    }

    public async Task<bool> IsExistByNationalCode(string nationalCode)
    {
        return await context.Set<Customer>()
            .AnyAsync(c => c.NationalCode == nationalCode);
    }

    public async Task<Customer?> FindById(int id)
    {
        return await context.Set<Customer>()
            .SingleOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> IsExistById(int id)
    {
        return await context.Set<Customer>().AnyAsync(c => c.Id == id);
    }

    public async Task<GetCustomerInformationDto?> GetInfoById(int id)
    {
        var query = (
            from customer in context.Set<Customer>()
            where customer.Id == id
            join financialInfo in context.Set<FinancialInformation>()
                on new
                {
                    CustomerId = customer.Id,
                    IsDeleted = false
                } equals
                new
                {
                    CustomerId = financialInfo.CustomerId,
                    IsDeleted = financialInfo.IsDeleted
                }
                into CustomerFinancialInfo
            from cFinancialInfo in CustomerFinancialInfo.DefaultIfEmpty()
            select new GetCustomerInformationDto()
            {
                IsVerified = customer.IsVerified,
                Name = customer.Name,
                LastName = customer.LastName,
                NationalCode = customer.NationalCode,
                FinancialInformationId =
                    cFinancialInfo == null ? null : cFinancialInfo.Id,
                JobType =
                    cFinancialInfo == null ? null : cFinancialInfo.JobType,
                MonthlyIncome = cFinancialInfo == null
                    ? null
                    : cFinancialInfo.MonthlyIncome,
                FinancialAssets = cFinancialInfo == null
                    ? null
                    : cFinancialInfo.FinancialAssets
            }
        );

        return await query.SingleOrDefaultAsync();
    }
}