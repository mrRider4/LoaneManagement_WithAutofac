namespace LoanManagement.TestTools.FinancialInformations;

public static class FinancialInformationServiceFactory
{
    public static FinancialInformationService Create(
        EfDataContext context,
        FinancialInformationRepository? repository = null,
        CustomerRepository? customerRepository = null)
    {
        repository ??= new EfFinancialInformationRepository(context);
        customerRepository ??= new EfCustomerRepository(context);
        var unitOfWork = new EfUnitOfWork(context);
        return new FinancialInformationAppService(
            unitOfWork,
            repository,
            customerRepository);
    }
}