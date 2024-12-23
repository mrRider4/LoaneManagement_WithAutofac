using LoanManagement.Services.Loans.Contracts;
using LoanManagement.Services.Loans.Contracts.CalculatTools;

namespace LoanManagement.Tool.Unit.Tests.LoanTools;

public class LoanCalculatorTest : BusinessIntegrationTest
{
    private readonly LoanCalculator _tut;

    public LoanCalculatorTest()
    {
        _tut = new LoanCalculatorTool();
    }

    [Theory]
    [InlineData(12)]
    [InlineData(13)]
    public void
        AnnualInterestPercentage_return_annual_interest_percentage_properly(
            int installmentCount)
    {
        var actual = _tut.AnnualInterestPercentage(installmentCount);

        var excepted = installmentCount > 12 ? 0.20m : 0.15m;
        actual.Should().Be(excepted);
    }

    [Fact]
    public void InstallmentAmount_return_installment_amount_properly()
    {
        var amount = 100;
        var installmentCount = 5;
        var annualInterestPercentage = (decimal)0.2;

        var actual = _tut.InstallmentAmount(amount, installmentCount,
            annualInterestPercentage);


        var excepted = Math.Round(((((annualInterestPercentage/12) + 1) * amount) /
                                   installmentCount), 2);
        actual.Should().Be(excepted);
    }

    [Fact]
    public void LatePaymentFee_return_late_payment_fee_properly()
    {
        var installmentAmount = 100;

        var actual = _tut.LatePaymentFee(installmentAmount);

        var excepted = Math.Round((installmentAmount * 0.02m), 2);
        actual.Should().Be(excepted);
    }

    [Fact]
    public void
        InstallmentInterestAmount_return_installment_interests_amount_properly()
    {
        var annualInterestPercentage = (decimal)0.15;
        var amount = 1000000;
        var installmentCount = 5;

        var actual = _tut.InstallmentInterestAmount(
            annualInterestPercentage,
            amount,
            installmentCount);

        var excepted = Math.Round((((annualInterestPercentage/12) * amount) /
                                   installmentCount), 2);
    }
}