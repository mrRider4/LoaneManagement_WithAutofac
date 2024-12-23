namespace LoanManagement.Service.Unit.Test.RequestedLoans;

public class RequestedLoanServiceTest : BusinessIntegrationTest
{
    private readonly RequestedLoanService _sut;
    private readonly DateOnly _defaultDateOnly;


    public RequestedLoanServiceTest()
    {
        _defaultDateOnly = new DateOnly(2023, 1, 1);
        Mock<DateOnlyService> dateOnlyServiceMock = new();
        dateOnlyServiceMock.Setup(s => s.NowUtc).Returns(_defaultDateOnly);
        _sut = RequestedLoanServiceFactory.Create(
            SetupContext,
            dateOnlyService: dateOnlyServiceMock.Object
        );
    }

    [Fact]
    public async Task Create_create_and_add_request_loan__properly()
    {
        var loan = new LoanBuilder()
            .Build();
        Save(loan);
        var customer = new CustomerBuilder()
            .WithIsVerified(true)
            .Build();
        Save(customer);
        var financialInfo = new FinancialInformationBuilder()
            .WithCustomerId(customer.Id)
            .WithJobType(JobType.Unemployed)
            .WithMonthlyIncome(0)
            .WithFinancialAssets(0)
            .Build();
        Save(financialInfo);


        var actual = await _sut.Create(loan.Id, customer.Id);


        var excepted = ReadContext.Set<RequestedLoan>();
        excepted.Single().Should().BeEquivalentTo(new RequestedLoan()
        {
            Id = actual,
            LoanId = loan.Id,
            FinancialInformationId = financialInfo.Id,
            RequestedDate = _defaultDateOnly,
            RequestedLoanStatus = RequestedLoanStatus.Rejected,
            CreditRate = 1
        });
    }

    [Theory]
    [InlineData(-1)]
    public async Task Create_throw_exception_when_customer_not_found(
        int fakeCustomerId)
    {
        var loan = new LoanBuilder()
            .Build();
        Save(loan);

        var actual = () => _sut.Create(loan.Id, fakeCustomerId);

        await actual.Should().ThrowExactlyAsync<CustomerNotFoundException>();
        ReadContext.Set<RequestedLoan>().Should().HaveCount(0);
    }

    [Fact]
    public async Task
        Create_throw_exception_when_customer_financial_information_not_found()
    {
        var loan = new LoanBuilder()
            .Build();
        Save(loan);
        var customer = new CustomerBuilder()
            .WithIsVerified(true)
            .Build();
        Save(customer);

        var actual = () => _sut.Create(loan.Id, customer.Id);

        await actual.Should()
            .ThrowExactlyAsync<FinancialInformationNotFoundException>();
        ReadContext.Set<RequestedLoan>().Should().HaveCount(0);
    }

    [Theory]
    [InlineData(-1)]
    public async Task Create_throw_exception_when_loan_not_found(
        int fakeLoanId)
    {
        var customer = new CustomerBuilder()
            .WithIsVerified(true)
            .Build();
        Save(customer);
        var financialInfo = new FinancialInformationBuilder()
            .WithCustomerId(customer.Id)
            .WithJobType(JobType.Unemployed)
            .WithMonthlyIncome(0)
            .WithFinancialAssets(0)
            .Build();
        Save(financialInfo);

        var actual = () => _sut.Create(fakeLoanId, customer.Id);

        await actual.Should().ThrowExactlyAsync<LoanNotFoundException>();
        ReadContext.Set<RequestedLoan>().Should().HaveCount(0);
    }

    [Theory]
    [InlineData(RequestedLoanStatus.Approved)]
    [InlineData(RequestedLoanStatus.Closed)]
    [InlineData(RequestedLoanStatus.Pending)]
    [InlineData(RequestedLoanStatus.Refunding)]
    [InlineData(RequestedLoanStatus.DelayedRefunding)]
    public async Task
        Create_throw_exception_when_the_loan_has_already_benn_taken_once(
            RequestedLoanStatus fakeStatus)
    {
        var loan = new LoanBuilder()
            .Build();
        Save(loan);
        var customer = new CustomerBuilder()
            .WithIsVerified(true)
            .Build();
        Save(customer);
        var financialInfo = new FinancialInformationBuilder()
            .WithCustomerId(customer.Id)
            .Build();
        Save(financialInfo);
        var requestedLoan = new RequestedLoanBuilder()
            .WithLoanId(loan.Id)
            .WithFinancialInformationId(financialInfo.Id)
            .WithRequestedLoanStatus(fakeStatus)
            .Build();
        Save(requestedLoan);

        var actual = () => _sut.Create(loan.Id, customer.Id);

        await actual.Should()
            .ThrowExactlyAsync<
                TheLoanHasAlreadyTakenOnceByThisCustomerException>();
        ReadContext.Set<RequestedLoan>().Should().HaveCount(1);
    }

    [Fact]
    public async Task Create_throw_exception_when_customer_is_not_verified()
    {
        var loan = new LoanBuilder().Build();
        Save(loan);
        var customer = new CustomerBuilder().WithIsVerified(false).Build();
        Save(customer);

        var actual = () => _sut.Create(loan.Id, customer.Id);

        await actual.Should()
            .ThrowExactlyAsync<CustomerIsNotVerifiedException>();
    }

    [Fact]
    public async Task
        ApproveById_change_requested_loan_status_to_Approved_by_id_properly()
    {
        var requestedLoan = new RequestedLoanBuilder().Build();
        Save(requestedLoan);

        await _sut.ApproveById(requestedLoan.Id);

        var excepted = ReadContext.Set<RequestedLoan>();
        excepted.Single().RequestedLoanStatus.Should()
            .Be(RequestedLoanStatus.Approved);
        
    }

    [Theory]
    [InlineData(-1)]
    public async Task ApproveById_throw_exception_when_requested_loan_not_found(
        int fakeRequestedLoanId)
    {
        var actual = () => _sut.ApproveById(fakeRequestedLoanId);

        await actual.Should()
            .ThrowExactlyAsync<RequestedLoanNotFoundException>();
    }

    [Theory]
    [InlineData(RequestedLoanStatus.Approved)]
    [InlineData(RequestedLoanStatus.DelayedRefunding)]
    [InlineData(RequestedLoanStatus.Refunding)]
    [InlineData(RequestedLoanStatus.Closed)]
    [InlineData(RequestedLoanStatus.Rejected)]
    public async Task
        ApproveById_throw_exception_when_the_request_loan_is_not_in_pending_status(
            RequestedLoanStatus fakeStatus)
    {
        var requestedLoan = new RequestedLoanBuilder()
            .WithRequestedLoanStatus(fakeStatus)
            .Build();
        Save(requestedLoan);

        var actual = () => _sut.ApproveById(requestedLoan.Id);

        await actual.Should()
            .ThrowExactlyAsync<RequestLoanIsNotInPendingStatusException>();
        ReadContext.Set<RequestedLoan>().Single().RequestedLoanStatus.Should()
            .Be(fakeStatus);
    }

    [Theory]
    [InlineData(59)]
    public async Task
        ApproveById_throw_exception_when_credit_rate_is_less_than_the_requirement(
            int fakeRate)
    {
        var requestedLoan = new RequestedLoanBuilder()
            .WithCreditRate(fakeRate)
            .Build();
        Save(requestedLoan);

        var actual = () => _sut.ApproveById(requestedLoan.Id);

        await actual.Should()
            .ThrowExactlyAsync<
                RequestedLoanCreditRateIsLessThanTheRequirementException>();
        ReadContext.Set<RequestedLoan>().Single().RequestedLoanStatus.Should()
            .Be(RequestedLoanStatus.Pending);
    }

    [Theory]
    [InlineData(RequestedLoanStatus.Approved)]
    [InlineData(RequestedLoanStatus.Refunding)]
    [InlineData(RequestedLoanStatus.DelayedRefunding)]
    public async Task
        ApproveById_throw_exception_when_the_customer_already_has_an_active_received_loan(
            RequestedLoanStatus fakeStatus)
    {
        var customer = new CustomerBuilder()
            .Build();
        Save(customer);
        var financialInfo = new FinancialInformationBuilder()
            .WithCustomerId(customer.Id)
            .Build();
        Save(financialInfo);
        var financialInfo2 = new FinancialInformationBuilder()
            .WithCustomerId(customer.Id)
            .Build();
        Save(financialInfo2);
        var requestedLoan = new RequestedLoanBuilder()
            .WithFinancialInformationId(financialInfo.Id)
            .WithRequestedLoanStatus(fakeStatus)
            .Build();
        Save(requestedLoan);
        var requestedLoan2 = new RequestedLoanBuilder()
            .WithFinancialInformationId(financialInfo2.Id)
            .WithRequestedLoanStatus(RequestedLoanStatus.Pending)
            .Build();
        Save(requestedLoan2);

        var actual = () => _sut.ApproveById(requestedLoan2.Id);

        await actual.Should()
            .ThrowExactlyAsync<
                TheCustomerAlreadyHasAnActiveReceivedLoanException>();
        ReadContext.Set<RequestedLoan>()
            .Where(r => r.RequestedLoanStatus == RequestedLoanStatus.Pending)
            .Should().HaveCount(1);
    }

    [Fact]
    public async Task
        UpdateReceivedLoanStatusById_update_properly_status_to_delayed_by_id_properly()
    {
        var requestedLoan = new RequestedLoanBuilder()
            .WithRequestedLoanStatus(RequestedLoanStatus.Approved)
            .Build();
        Save(requestedLoan);
        var installment = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan.Id)
            .WithDueDate(_defaultDateOnly.AddMonths(-2).AddDays(-1))
            .Build();
        Save(installment);
      

        await _sut.UpdateReceivedLoanStatusById(requestedLoan.Id);

        var excepted = ReadContext.Set<RequestedLoan>();
        excepted.Single().RequestedLoanStatus.Should()
            .Be(RequestedLoanStatus.DelayedRefunding);
    }
    
    [Fact]
    public async Task
        UpdateReceivedLoanStatusById_update_properly_status_to_refunding_by_id_properly()
    {
        var requestedLoan = new RequestedLoanBuilder()
            .WithRequestedLoanStatus(RequestedLoanStatus.Approved)
            .Build();
        Save(requestedLoan);
        var installment = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan.Id)
            .WithDueDate(_defaultDateOnly.AddDays(1))
            .WithPaymentDate(_defaultDateOnly.AddDays(-1))
            .Build();
        Save(installment);
        var installment2 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan.Id)
            .WithDueDate(_defaultDateOnly.AddMonths(1))
            .Build();
        Save(installment2);

        await _sut.UpdateReceivedLoanStatusById(requestedLoan.Id);

        var excepted = ReadContext.Set<RequestedLoan>();
        excepted.Single().RequestedLoanStatus.Should()
            .Be(RequestedLoanStatus.Refunding);
    }
    
    [Theory]
    [InlineData(RequestedLoanStatus.Refunding)]
    [InlineData(RequestedLoanStatus.DelayedRefunding)]
    [InlineData(RequestedLoanStatus.Approved)]
    public async Task
        UpdateReceivedLoanStatusById_update_properly_status_to_closed_by_id_properly(RequestedLoanStatus
            fakeStatus)
    {
        var loan = new LoanBuilder().WithInstallmentCount(2).Build();
        Save(loan);
        var requestedLoan = new RequestedLoanBuilder()
            .WithLoanId(loan.Id)
            .WithRequestedLoanStatus(fakeStatus)
            .Build();
        Save(requestedLoan);
        var installment = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan.Id)
            .WithDueDate(_defaultDateOnly.AddDays(1))
            .WithPaymentDate(_defaultDateOnly.AddDays(-1))
            .Build();
        Save(installment);
        var installment2 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan.Id)
            .WithDueDate(_defaultDateOnly.AddMonths(1))
            .WithPaymentDate(_defaultDateOnly.AddDays(20))
            .Build();
        Save(installment2);

        await _sut.UpdateReceivedLoanStatusById(requestedLoan.Id);

        var excepted = ReadContext.Set<RequestedLoan>();
        excepted.Single().RequestedLoanStatus.Should()
            .Be(RequestedLoanStatus.Closed);
    }

    [Theory]
    [InlineData(-1)]
    public async Task
        UpdateReceivedLoanStatusById_throw_exception_when_request_not_found(int fakeId)
    {
        var actual = () => _sut.UpdateReceivedLoanStatusById(fakeId);

        await actual.Should()
            .ThrowExactlyAsync<RequestedLoanNotFoundException>();
    }

    [Fact]
    public async Task
        UpdateReceivedLoanStatusById_throw_exception_when_request_is_already_closed()
    {
        var requestedLoan = new RequestedLoanBuilder()
            .WithRequestedLoanStatus(RequestedLoanStatus.Closed).Build();
        Save(requestedLoan);

        var actual = () => _sut.UpdateReceivedLoanStatusById(requestedLoan.Id);

        await actual.Should()
            .ThrowExactlyAsync<RequestedLoanIsAlreadyClosedException>();
    }
}