namespace LoanManagement.Services.Loans.Contracts;

public interface LoanCalculator
{
    decimal AnnualInterestPercentage(int installmentCount);

    decimal InstallmentAmount(
        decimal amount,
        int installmentCount,
        decimal annualInterestPercentage);

    decimal LatePaymentFee(decimal installmentAmount);
    
    decimal InstallmentInterestAmount(
        decimal amount,
        int installmentCount,
        decimal annualInterestPercentage);
}