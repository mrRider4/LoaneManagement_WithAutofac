namespace LoanManagement.Services.Customers.Contracts;

public interface CustomerQuery: Repository
{
    Task<HashSet<GetCustomerAndFinancialInfoDto>>
        GetCustomersWithOptionalVerificationFilter(bool? verificationFilter=null);
}