using LoanManagement.Services.Loans.Contracts.DTOs;

namespace LoanManagement.TestTools.Loans;

public static class AddLoanDtoFactory
{
    public static AddLoanDto Create(
        decimal amount = 100,
        int installmentCount = 2)
    {
        return new AddLoanDto()
        {
            Amount = amount,
            InstallmentCount = installmentCount
        };
    }
}