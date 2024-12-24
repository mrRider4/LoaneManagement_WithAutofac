namespace LoanManagement.Services.Loans.Contracts;

public interface LoanRepository : Repository
{
    Task Add(Loan loan);
    Task<bool> IsExistByAmountAndInstallmentCount(decimal amount,
        int installmentCount);

    Task<GetLoanInfoDto?> GetInfoById(int id);
}