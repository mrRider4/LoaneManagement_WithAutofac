namespace LoanManagement.TestTools.Installments;

public static class InstallmentServiceFactory
{
    public static InstallmentService Create(
        EfDataContext context,
        InstallmentRepository? repository = null,
        DateOnlyService? dateOnlyService = null,
        LoanRepository? loanRepository = null,
        RequestedLoanRepository? requestedLoanRepository = null)
    {
        repository ??= new EfInstallmentRepository(context);
        dateOnlyService ??= new DateOnlyAppService();
        loanRepository ??= new EfLoanRepository(context);
        requestedLoanRepository ??= new EfRequestedLoanRepository(context);
        var unitOfWork = new EfUnitOfWork(context);
        return new InstallmentAppService(
            unitOfWork,
            repository,
            dateOnlyService,
            loanRepository,
            requestedLoanRepository);
    }
}