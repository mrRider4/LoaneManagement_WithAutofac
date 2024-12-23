using LoanManagement.Services.Loans.Contracts.DTOs;

namespace LoanManagement.Services.Loans;

public class LoanAppService(
    UnitOfWork unitOfWork,
    LoanRepository repository,
    LoanCalculator loanCalculator) : LoanService
{
    public async Task<int> Create(AddLoanDto dto)
    {
        if (dto.Amount<=0)
        {
            throw new InvalidLoanAmountException();
        }

        if (dto.InstallmentCount <= 0)
        {
            throw new InvalidLoanInstallmentCountException();
        }
        

        var annualInterestPercentage =
            loanCalculator.AnnualInterestPercentage(dto.InstallmentCount);
        var installmentAmount = loanCalculator.InstallmentAmount(
            dto.Amount, dto.InstallmentCount, annualInterestPercentage);
        var latePaymentFee = loanCalculator.LatePaymentFee(installmentAmount);
        var loan = new Loan()
        {
            Amount = dto.Amount,
            AnnualInterestPercentage = annualInterestPercentage,
            InstallmentCount = dto.InstallmentCount,
            InstallmentAmount = installmentAmount,
            LatePaymentFee = latePaymentFee
        };
        await repository.Add(loan);
        await unitOfWork.Save();
        return loan.Id;
    }
}