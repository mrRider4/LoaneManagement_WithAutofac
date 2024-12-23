namespace LoanManagement.Services.Loans.Contracts;

public interface LoanRepository
{
    Task Add(Loan loan);
    Task<bool> IsExistByAmountAndInstallmentCount(decimal amount,
        int installmentCount);

    Task<GetLoanInfoDto?> GetInfoById(int id);
}