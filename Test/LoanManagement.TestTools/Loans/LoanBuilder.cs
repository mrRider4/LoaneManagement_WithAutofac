namespace LoanManagement.TestTools.Loans;

public class LoanBuilder
{
    private readonly Loan _loan;
    private readonly LoanCalculator _loanCalculator;

    public LoanBuilder()
    {
        _loanCalculator = new LoanCalculatorTool();
        _loan = new Loan()
        {
            Amount = 1000,
            InstallmentCount = 4,
            AnnualInterestPercentage = -1,
            InstallmentAmount = -1,
            LatePaymentFee = -1
        };
    }

    public LoanBuilder WithAmount(decimal amount)
    {
        _loan.Amount = amount;
        return this;
    }

    public LoanBuilder WithInstallmentCount(int installmentCount)
    {
        _loan.InstallmentCount = installmentCount;
        return this;
    }

    public Loan Build()
    {
        if (_loan.AnnualInterestPercentage == -1)
        {
            _loan.AnnualInterestPercentage =
                _loanCalculator.AnnualInterestPercentage(
                    _loan.InstallmentCount);
        }

        if (_loan.InstallmentAmount == -1)
        {
            _loan.InstallmentAmount = _loanCalculator.InstallmentAmount(
                _loan.Amount, _loan.InstallmentCount,
                _loan.AnnualInterestPercentage);
        }

        if (_loan.LatePaymentFee == -1)
        {
            _loan.LatePaymentFee =
                _loanCalculator.LatePaymentFee(_loan.InstallmentAmount);
        }

        return _loan;
    }
}