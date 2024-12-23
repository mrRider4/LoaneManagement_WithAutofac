namespace LoanManagement.TestTools.Customers;

public static class CustomerServiceFactory
{
    public static CustomerService Create(
        EfDataContext context,
        CustomerRepository? repository = null,
        FinancialInformationRepository? financialInfoRepository = null)
    {
        repository ??= new EfCustomerRepository(context);
        financialInfoRepository ??=
            new EfFinancialInformationRepository(context);
        
        var unitOfWork = new EfUnitOfWork(context);
        return new CustomerAppService(
            unitOfWork,
            repository,
            financialInfoRepository);
    }
}