using LoanManagement.Services.Loans;

namespace LoanManagement.TestTools.Loans;

public static class LoanServiceFactory
{
    public static LoanService Create(
        EfDataContext context,
        LoanRepository? repository = null,
        LoanCalculator? loanCalculator = null)
    {
        repository ??= new EfLoanRepository(context);
        var unitOfWoork = new EfUnitOfWork(context);
        loanCalculator ??= new LoanCalculatorTool();
        return new LoanAppService(
            unitOfWoork,
            repository,
            loanCalculator
        );
    }
}