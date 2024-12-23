namespace LoanManagement.Service.Unit.Test.Loans;

public class LoanQueryTest : BusinessIntegrationTest
{
    private readonly LoanQuery _sut;

    public LoanQueryTest()
    {
        _sut = LoanQueryFactory.Create(SetupContext);
    }

    [Fact]
    public async Task GetLoansWithOptionalTermFilter_get_all_loans_properly()
    {
        var shortTermLoan = new LoanBuilder()
            .WithInstallmentCount(12)
            .Build();
        Save(shortTermLoan);
        var longTermLoan = new LoanBuilder()
            .WithInstallmentCount(13)
            .Build();
        Save(longTermLoan);

        var result = await _sut.GetLoansWithOptionalTermFilter(null);

        var excepted = ReadContext.Set<Loan>();
        var exceptedShortTermLone = excepted
            .Where(l =>
                l.Id == shortTermLoan.Id)
            .Select(l => new GetLoanDto()
            {
                Id = l.Id,
                Amount = l.Amount,
                AnnualInterestPercentage = l.AnnualInterestPercentage,
                InstallmentCount = l.InstallmentCount,
                InstallmentAmount = l.InstallmentAmount,
                LatePaymentFee = l.LatePaymentFee
            }).Single();
        result.Should().HaveCount(2)
            .And.Contain(l =>
                l.Id == longTermLoan.Id && l.InstallmentCount ==
                longTermLoan.InstallmentCount);
        result.Single(l => l.Id == shortTermLoan.Id).Should()
            .BeEquivalentTo(exceptedShortTermLone);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task
        GetLoansWithOptionalTermFilter_get_all_loans_With_is_short_term_filter_properly(
            bool isShortTerm)
    {
        var shortTermLoan = new LoanBuilder()
            .WithInstallmentCount(12)
            .Build();
        Save(shortTermLoan);
        var longTermLoan = new LoanBuilder()
            .WithInstallmentCount(13)
            .Build();
        Save(longTermLoan);

        var result = await _sut.GetLoansWithOptionalTermFilter(isShortTerm);

        var excepted = ReadContext.Set<Loan>()
            .Where(l =>
                isShortTerm
                    ? l.InstallmentCount <= 12
                    : l.InstallmentCount > 12);
        result.Should().HaveCount(1);
        result.All(r => excepted.Any(l => l.Id == r.Id) &&
                        isShortTerm
                ? r.InstallmentCount <= 12
                : r.InstallmentCount > 12)
            .Should().BeTrue();
    }
}