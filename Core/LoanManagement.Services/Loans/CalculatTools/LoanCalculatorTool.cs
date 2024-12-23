namespace LoanManagement.Services.Loans.Contracts.CalculatTools;

public class LoanCalculatorTool : LoanCalculator
{
    public decimal AnnualInterestPercentage(int installmentCount)
    {
        return installmentCount > 12 ? 0.20m : 0.15m;
    }

    public decimal InstallmentAmount(
        decimal amount,
        int installmentCount,
        decimal annualInterestPercentage)
    {
       
        return Math.Round(((((annualInterestPercentage/12)+1) * amount) /
                           installmentCount), 2);
    }

    public decimal LatePaymentFee(decimal installmentAmount)
    {
        return Math.Round((installmentAmount * 0.02m), 2);
    }

    public decimal InstallmentInterestAmount(
        decimal amount,
        int installmentCount,
        decimal annualInterestPercentage)
    {
        return Math.Round((((annualInterestPercentage/12) * amount) /
                           installmentCount), 2);
    }
}