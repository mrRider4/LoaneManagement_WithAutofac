namespace LoanManagement.TestTools.RequestedLoans;

public static class RequestedLoanServiceFactory
{
    public static RequestedLoanService Create(
        EfDataContext context,
        RequestedLoanRepository? repository = null,
        DateOnlyService? dateOnlyService = null,
        CustomerRepository? customerRepository = null,
        LoanRepository? loanRepository = null,
        RequestedLoanCalculator? requestedLoanCalculator=null)
    {
        repository ??= new EfRequestedLoanRepository(context);
        dateOnlyService ??= new DateOnlyAppService();
        customerRepository ??= new EfCustomerRepository(context);
        loanRepository ??= new EfLoanRepository(context);
        requestedLoanCalculator ??= new RequestedLoanCalculatorTool();
        var unitOfWork = new EfUnitOfWork(context);

        return new RequestedLoanAppService(
            unitOfWork,
            repository,
            dateOnlyService,
            customerRepository,
            loanRepository,
            requestedLoanCalculator);
    }
}