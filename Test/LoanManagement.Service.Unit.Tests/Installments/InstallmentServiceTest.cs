namespace LoanManagement.Service.Unit.Test.Installments;

public class InstallmentServiceTest : BusinessIntegrationTest
{
    private readonly InstallmentService _sut;
    private readonly Mock<DateOnlyService> _dateOnlyServiceMock;
    private readonly DateOnly _defaultDateOnly;

    public InstallmentServiceTest()
    {
        _dateOnlyServiceMock = new Mock<DateOnlyService>();
        _defaultDateOnly = new DateOnly(2024, 1, 1);
        _dateOnlyServiceMock.Setup(s => s.NowUtc).Returns(_defaultDateOnly);
        _sut = InstallmentServiceFactory.Create(
            SetupContext,
            dateOnlyService: _dateOnlyServiceMock.Object);
    }

    [Fact]
    public async Task
        CreatRangeByRequestedLoanId_create_and_add_all_required_installments_by_requested_loan_id_properly()
    {
        var loan = new LoanBuilder()
            .WithAmount(1000)
            .WithInstallmentCount(2)
            .Build();
        Save(loan);
        var requestedLoan =
            new RequestedLoanBuilder().WithLoanId(loan.Id)
                .WithRequestedLoanStatus(RequestedLoanStatus.Approved)
                .Build();
        Save(requestedLoan);

        await _sut.CreatRangeByRequestedLoanId(requestedLoan.Id);

        var excepted = ReadContext.Set<Installment>();
        excepted.Should().HaveCount(loan.InstallmentCount)
            .And.Contain(i => i.DueDate == _defaultDateOnly.AddMonths(1))
            .And.Contain(i => i.DueDate == _defaultDateOnly.AddMonths(2));
        excepted.All(i => i.PayableAmount == loan.InstallmentAmount).Should()
            .BeTrue();
        excepted.All(i => i.PaymentDate == null).Should()
            .BeTrue();
    }

    [Theory]
    [InlineData(-1)]
    public async Task
        CreatRangeByRequestedLoanId_throw_exception_when_requested_loan_not_found(
            int fakeRequestedLoan)
    {
        var actual = () => _sut.CreatRangeByRequestedLoanId(fakeRequestedLoan);

        await actual.Should()
            .ThrowExactlyAsync<RequestedLoanNotFoundException>();
        ReadContext.Set<Installment>().Should().HaveCount(0);
    }

    [Theory]
    [InlineData(-1)]
    public async Task
        CreatRangeByRequestedLoanId_throw_exception_when_loan_not_found(
            int fakeLoan)
    {
        var requestedLoan =
            new RequestedLoanBuilder().WithLoanId(fakeLoan)
                .WithRequestedLoanStatus(RequestedLoanStatus.Approved)
                .Build();
        Save(requestedLoan);

        var actual = () => _sut.CreatRangeByRequestedLoanId(requestedLoan.Id);

        await actual.Should()
            .ThrowExactlyAsync<LoanNotFoundException>();
        ReadContext.Set<Installment>().Should().HaveCount(0);
    }

    [Fact]
    public async Task
        UpdatePaymentDateAndAmountByRequestId_set_nowUtc_as_paymentDate_for_first_not_payed_installment_without_delay_properly()
    {
        var loan = new LoanBuilder()
            .WithInstallmentCount(2)
            .Build();
        Save(loan);
        var requestedLoan = new RequestedLoanBuilder()
            .WithLoanId(loan.Id)
            .WithRequestedLoanStatus(RequestedLoanStatus.Approved)
            .Build();
        Save(requestedLoan);
        var installment = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan.Id)
            .WithPayableAmount(loan.InstallmentAmount)
            .WithDueDate(_defaultDateOnly.AddDays(4))
            .Build();
        Save(installment);
        var installment2 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan.Id)
            .WithPayableAmount(loan.InstallmentAmount)
            .WithDueDate(_defaultDateOnly.AddMonths(1).AddDays(4))
            .Build();
        Save(installment2);

        await _sut.UpdatePaymentDateAndAmountByRequestId(requestedLoan.Id);

        var excepted = ReadContext.Set<Installment>();
        excepted.Should().Contain(i =>
                i.Id == installment.Id &&
                i.PayableAmount == loan.InstallmentAmount &&
                i.PaymentDate == _defaultDateOnly)
            .And.Contain(i =>
                i.Id == installment2.Id &&
                i.PaymentDate == null);
    }

    [Fact]
    public async Task
        UpdatePaymentDateAndAmountByRequestId_set_nowUtc_as_paymentDate_for_first_not_payed_installment_with_delay_properly()
    {
        var loan = new LoanBuilder()
            .WithInstallmentCount(2)
            .Build();
        Save(loan);
        var requestedLoan = new RequestedLoanBuilder()
            .WithLoanId(loan.Id)
            .WithRequestedLoanStatus(RequestedLoanStatus.Approved)
            .Build();
        Save(requestedLoan);
        var installment = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan.Id)
            .WithPayableAmount(loan.InstallmentAmount)
            .WithDueDate(_defaultDateOnly.AddDays(-4))
            .Build();
        Save(installment);
        var installment2 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan.Id)
            .WithPayableAmount(loan.InstallmentAmount)
            .WithDueDate(_defaultDateOnly.AddMonths(1).AddDays(4))
            .Build();
        Save(installment2);

        await _sut.UpdatePaymentDateAndAmountByRequestId(requestedLoan.Id);

        var excepted = ReadContext.Set<Installment>();
        excepted.Should().Contain(i =>
                i.Id == installment.Id &&
                i.PayableAmount ==
                loan.InstallmentAmount + loan.LatePaymentFee &&
                i.PaymentDate == _defaultDateOnly)
            .And.Contain(i =>
                i.Id == installment2.Id &&
                i.PaymentDate == null);
    }

    [Theory]
    [InlineData(-1)]
    public async Task
        UpdatePaymentDateAndAmountByRequestId_throw_exception_when_request_not_found(
            int fakeRequestId)
    {
        var actual = () =>
            _sut.UpdatePaymentDateAndAmountByRequestId(fakeRequestId);

        await actual.Should()
            .ThrowExactlyAsync<RequestedLoanNotFoundException>();
    }

    [Theory]
    [InlineData(-1)]
    public async Task
        UpdatePaymentDateAndAmountByRequestId_throw_exception_when_loan_not_found(
            int fakeLoanId)
    {
        var requestedLoan =
            new RequestedLoanBuilder()
                .WithRequestedLoanStatus(RequestedLoanStatus.Approved)
                .WithLoanId(fakeLoanId).Build();
        Save(requestedLoan);

        var actual = () =>
            _sut.UpdatePaymentDateAndAmountByRequestId(requestedLoan.Id);

        await actual.Should()
            .ThrowExactlyAsync<LoanNotFoundException>();
    }

    [Fact]
    public async Task
        UpdatePaymentDateAndAmountByRequestId_throw_exception_when_request_is_already_closed()
    {
        var requestedLoan =
            new RequestedLoanBuilder()
                .WithRequestedLoanStatus(RequestedLoanStatus.Closed)
                .Build();
        Save(requestedLoan);

        var actual = () =>
            _sut.UpdatePaymentDateAndAmountByRequestId(requestedLoan.Id);

        await actual.Should()
            .ThrowExactlyAsync<RequestedLoanIsAlreadyClosedException>();
    }

    [Theory]
    [InlineData(RequestedLoanStatus.Pending)]
    [InlineData(RequestedLoanStatus.Rejected)]
    public async Task
        UpdatePaymentDateAndAmountByRequestId_throw_exception_when_request_is_rejected_or_pending(
            RequestedLoanStatus fakeStatus)
    {
        var requestedLoan =
            new RequestedLoanBuilder()
                .WithRequestedLoanStatus(fakeStatus)
                .Build();
        Save(requestedLoan);

        var actual = () =>
            _sut.UpdatePaymentDateAndAmountByRequestId(requestedLoan.Id);

        await actual.Should()
            .ThrowExactlyAsync<ThisLoanIsNotReceivedException>();
    }
}