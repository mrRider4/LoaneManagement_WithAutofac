namespace LoanManagement.Persistence.Ef.Customers;

public class EfCustomerQuery(EfDataContext context) : CustomerQuery
{
    public async Task<HashSet<GetCustomerAndFinancialInfoDto>>
        GetCustomersWithOptionalVerificationFilter(
            bool? verificationFilter = null)
    {
        var query = (
            from customer in context.Set<Customer>()
            join financial in context.Set<FinancialInformation>()
                on new
                {
                    cId = customer.Id, isDeleted = false
                } equals new
                {
                    cId = financial.CustomerId, isDeleted = financial.IsDeleted
                }
                into CustomerFinancialInfo
            from cFInfo in CustomerFinancialInfo.DefaultIfEmpty()
            select new GetCustomerAndFinancialInfoDto()
            {
                Id = customer.Id,
                IsVerified = customer.IsVerified,
                Name = customer.Name,
                LastName = customer.LastName,
                NationalCode = customer.NationalCode,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                FinancialInformation = cFInfo == null
                    ? null
                    : new GetFinancialInformationDto()
                    {
                        JobType = cFInfo.JobType,
                        MonthlyIncome = cFInfo.MonthlyIncome,
                        FinancialAssets = cFInfo.FinancialAssets
                    }
            });
        if (verificationFilter != null)
        {
            query = query.Where(c => c.IsVerified == verificationFilter);
        }

        var result = await query.ToListAsync();
        return result.ToHashSet();
    }
}