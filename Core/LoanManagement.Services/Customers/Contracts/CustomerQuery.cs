namespace LoanManagement.Services.Customers.Contracts;

public interface CustomerQuery
{
    Task<HashSet<GetCustomerAndFinancialInfoDto>>
        GetCustomersWithOptionalVerificationFilter(bool? verificationFilter=null);
}