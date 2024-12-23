namespace LoanManagement.Service.Unit.Test.Loans;

public class LoanServiceTest : BusinessIntegrationTest
{
    private readonly LoanService _sut;
    private readonly LoanCalculator _loanCalculatorTool;

    public LoanServiceTest()
    {
        _loanCalculatorTool = new LoanCalculatorTool();
        _sut = LoanServiceFactory.Create(SetupContext);
    }

    [Fact]
    public async Task Create_create_and_add_loan_properly()
    {
        var dto = AddLoanDtoFactory.Create(1000, 5);

        var actual = await _sut.Create(dto);

        var excepted = ReadContext.Set<Loan>();
        var exceptedAnnualInterestPercentage =
            _loanCalculatorTool.AnnualInterestPercentage(dto.InstallmentCount);
        var exceptedInstallmentAmount = _loanCalculatorTool.InstallmentAmount(
            dto.Amount, dto.InstallmentCount, exceptedAnnualInterestPercentage);
        var exceptedLatePaymentFee =
            _loanCalculatorTool.LatePaymentFee(exceptedInstallmentAmount);
        excepted.Single().Should().BeEquivalentTo(new Loan()
        {
            Id = actual,
            Amount = dto.Amount,
            AnnualInterestPercentage = exceptedAnnualInterestPercentage,
            InstallmentCount = dto.InstallmentCount,
            InstallmentAmount = exceptedInstallmentAmount,
            LatePaymentFee = exceptedLatePaymentFee
        });
    }
    

    [Fact]
    public async Task Create_throw_exception_when_amount_is_zero_or_less()
    {
        var dto = AddLoanDtoFactory.Create(0, 2);

        var actual = () => _sut.Create(dto);

        await actual.Should().ThrowExactlyAsync<InvalidLoanAmountException>();
    }
    
    [Fact]
    public async Task Create_throw_exception_when_installment_count_is_zero_or_less()
    {
        var dto = AddLoanDtoFactory.Create(10, 0);

        var actual = () => _sut.Create(dto);

        await actual.Should().ThrowExactlyAsync<
            InvalidLoanInstallmentCountException>();
    }
}