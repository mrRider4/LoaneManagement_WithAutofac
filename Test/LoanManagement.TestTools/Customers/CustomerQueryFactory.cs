namespace LoanManagement.TestTools.Customers;

public static class CustomerQueryFactory
{
    public static CustomerQuery Create(EfDataContext context)
    {
        return new EfCustomerQuery(context);
    }
}