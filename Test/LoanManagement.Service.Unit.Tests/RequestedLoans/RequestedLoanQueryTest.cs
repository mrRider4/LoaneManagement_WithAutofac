using Microsoft.VisualBasic;

namespace LoanManagement.Service.Unit.Test.RequestedLoans;

public class RequestedLoanQueryTest : BusinessIntegrationTest
{
    private readonly RequestedLoanQuery _sut;
    private readonly DateOnly _defaultDateOnly;

    public RequestedLoanQueryTest()
    {
        _defaultDateOnly = new DateOnly(2024, 1, 1);
        Mock<DateOnlyService> dateOnlyServiceMock = new();
        dateOnlyServiceMock.Setup(s => s.NowUtc).Returns(_defaultDateOnly);
        _sut = RequestedLoanQueryFactory.Create(
            SetupContext,
            dateOnlyServiceMock.Object);
    }

    [Fact]
    public async Task GetAllPending_get_all_requests_that_status_is_pending()
    {
        var loan = new LoanBuilder().Build();
        Save(loan);
        var requestedLoan =
            new RequestedLoanBuilder()
                .WithLoanId(loan.Id)
                .WithRequestedLoanStatus(RequestedLoanStatus.Pending)
                .Build();
        Save(requestedLoan);
        var requestedLoan2 =
            new RequestedLoanBuilder()
                .WithLoanId(loan.Id)
                .WithRequestedLoanStatus(RequestedLoanStatus.Refunding)
                .Build();
        Save(requestedLoan2);

        var result = await _sut.GetAllPending();

        var excepted = ReadContext.Set<RequestedLoan>();
        var exceptedResult = excepted
            .Where(r => r.RequestedLoanStatus == RequestedLoanStatus.Pending)
            .Select(r => new GetRequestedLoanDto()
            {
                Id = r.Id,
                LoanId = r.LoanId,
                FinancialInformationId = r.FinancialInformationId,
                RequestedDate = r.RequestedDate,
                RequestedLoanStatus = r.RequestedLoanStatus,
                CreditRate = r.CreditRate
            }).Single();
        result.All(r => excepted.Any(e => e.Id == r.Id)).Should().BeTrue();
        result.Single().Should().BeEquivalentTo(exceptedResult);
    }

    [Theory]
    [InlineData(RequestedLoanStatus.Pending)]
    [InlineData(RequestedLoanStatus.Closed)]
    [InlineData(RequestedLoanStatus.Rejected)]
    public async Task
        GetRefundingAndDelayedRefundingLoansReport_get_all_received_loans_that_status_is_just_refunding_or_delayedRefunding_properly(
            RequestedLoanStatus fakeStatus)
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
        var requestedLoan2 = new RequestedLoanBuilder()
            .WithLoanId(loan.Id)
            .WithRequestedLoanStatus(RequestedLoanStatus.Approved)
            .Build();
        Save(requestedLoan2);
        var requestedLoan3 = new RequestedLoanBuilder()
            .WithLoanId(loan.Id)
            .WithRequestedLoanStatus(RequestedLoanStatus.Refunding)
            .Build();
        Save(requestedLoan3);
        var requestedLoan4 = new RequestedLoanBuilder()
            .WithLoanId(loan.Id)
            .WithRequestedLoanStatus(fakeStatus)
            .Build();
        Save(requestedLoan4);
        for (int i = 1; i <= loan.InstallmentCount; i++)
        {
            var instalment = new InstallmentBuilder()
                .WithRequestedLoanId(requestedLoan.Id)
                .WithDueDate(_defaultDateOnly.AddMonths(i))
                .Build();
            Save(instalment);
        }

        for (int i = 1; i <= loan.InstallmentCount; i++)
        {
            var instalment2 = new InstallmentBuilder()
                .WithRequestedLoanId(requestedLoan2.Id)
                .WithDueDate(_defaultDateOnly.AddMonths(i - 3))
                .Build();
            Save(instalment2);
        }

        for (int i = 1; i < loan.InstallmentCount; i++)
        {
            var instalment3 = new InstallmentBuilder()
                .WithRequestedLoanId(requestedLoan3.Id)
                .WithDueDate(_defaultDateOnly.AddMonths(i - 1))
                .WithPaymentDate(_defaultDateOnly.AddMonths(i - 1).AddDays(-5))
                .WithPayableAmount(loan.InstallmentAmount)
                .Build();
            Save(instalment3);
        }

        var instalment4 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan3.Id)
            .WithDueDate(_defaultDateOnly.AddDays(1))
            .WithPayableAmount(loan.InstallmentAmount)
            .Build();
        Save(instalment4);
        var fakeRequestedLoan = new RequestedLoanBuilder()
            .WithRequestedLoanStatus(RequestedLoanStatus.Approved)
            .WithLoanId(loan.Id)
            .Build();
        Save(fakeRequestedLoan);
        var fakeInstallment = new InstallmentBuilder()
            .WithRequestedLoanId(fakeRequestedLoan.Id)
            .WithDueDate(_defaultDateOnly)
            .Build();
        Save(fakeInstallment);
        var fakeInstallment2 = new InstallmentBuilder()
            .WithRequestedLoanId(fakeRequestedLoan.Id)
            .WithDueDate(_defaultDateOnly)
            .WithPaymentDate(_defaultDateOnly)
            .Build();
        Save(fakeInstallment2);


        var result = await _sut.GetRefundingAndDelayedRefundingLoansReport();


        var excepted = (
                from request in ReadContext.Set<RequestedLoan>()
                where request.RequestedLoanStatus ==
                      RequestedLoanStatus.Approved ||
                      request.RequestedLoanStatus ==
                      RequestedLoanStatus.DelayedRefunding ||
                      request.RequestedLoanStatus ==
                      RequestedLoanStatus.Refunding
                join inst in ReadContext.Set<Installment>()
                    on request.Id equals inst.RequestedLoanId
                select new
                {
                    Request = request,
                    Installment = inst
                })
            .GroupBy(e => e.Request)
            .Select(g => new
            {
                Request = g.Key,
                Installments = g.Select(r => r.Installment).ToList()
            }).ToList();
        result.Should().HaveCount(4);
        result.Should().Contain(r =>
            r.Id == requestedLoan.Id &&
            r.LoanId == loan.Id &&
            r.RequestedLoanStatus == RequestedLoanStatus.Refunding &&
            r.PayedAmount == 0 &&
            r.RemainingInstallments.Count == loan.InstallmentCount);
        result.Should().Contain(r =>
                r.Id == requestedLoan2.Id &&
                r.LoanId == loan.Id &&
                r.RequestedLoanStatus == RequestedLoanStatus.DelayedRefunding &&
                r.PayedAmount == 0 &&
                r.RemainingInstallments.Count == loan.InstallmentCount)
            .And.Contain(r =>
                r.Id == fakeRequestedLoan.Id &&
                r.RequestedLoanStatus == RequestedLoanStatus.Refunding);
        var exceptedRequestLoan3 = excepted
            .Where(e => e.Request.Id == requestedLoan3.Id)
            .Select(e => new GetRequestedLoanAndPaymentSummaryDto()
            {
                Id = e.Request.Id,
                LoanId = e.Request.LoanId,
                FinancialInformationId = e.Request.FinancialInformationId,
                RequestedDate = e.Request.RequestedDate,
                RequestedLoanStatus = RequestedLoanStatus.Refunding,
                CreditRate = e.Request.CreditRate,
                LoanAmount = loan.Amount,
                InstallmentCount = loan.InstallmentCount,
                PayedAmount = e.Installments
                    .Where(i => i.PaymentDate != null)
                    .Sum(i => i.PayableAmount),
                RemainingInstallments = e.Installments
                    .Where(i => i.PaymentDate == null)
                    .Select(i => new GetInstallmentDto()
                    {
                        Id = i.Id,
                        DueDate = i.DueDate,
                        PaymentDate = i.PaymentDate,
                        PayableAmount = i.PayableAmount
                    }).ToHashSet()
            }).Single();
        result.Single(r => r.Id == requestedLoan3.Id).Should()
            .BeEquivalentTo(exceptedRequestLoan3);
    }

    [Fact]
    public async Task
        GetAllHighRiskCustomersReport_get_all_customers_who_paid_at_least_tow_installments_late_properly()
    {
        var customer1 = new CustomerBuilder()
            .Build();
        Save(customer1);
        var financialInfo1 = new FinancialInformationBuilder()
            .WithCustomerId(customer1.Id)
            .Build();
        Save(financialInfo1);
        var financialInfo2 = new FinancialInformationBuilder()
            .WithCustomerId(customer1.Id)
            .Build();
        Save(financialInfo2);
        var requestedLoan1 = new RequestedLoanBuilder()
            .WithFinancialInformationId(financialInfo1.Id)
            .Build();
        Save(requestedLoan1);
        var requestedLoan2 = new RequestedLoanBuilder()
            .WithFinancialInformationId(financialInfo2.Id)
            .Build();
        Save(requestedLoan2);
        var installment1 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan1.Id)
            .WithDueDate(_defaultDateOnly)
            .WithPaymentDate(_defaultDateOnly.AddDays(3))
            .Build();
        Save(installment1);
        var installment2 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan2.Id)
            .WithDueDate(_defaultDateOnly.AddDays(-3))
            .Build();
        Save(installment2);
        var customer2 = new CustomerBuilder()
            .Build();
        Save(customer2);
        var financialInfo3 = new FinancialInformationBuilder()
            .WithCustomerId(customer2.Id)
            .Build();
        Save(financialInfo3);
        var requestedLoan3 = new RequestedLoanBuilder()
            .WithFinancialInformationId(financialInfo3.Id)
            .Build();
        Save(requestedLoan3);
        var installment3 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan3.Id)
            .WithDueDate(_defaultDateOnly.AddDays(4))
            .Build();
        Save(installment3);
        var installment4 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan3.Id)
            .WithDueDate(_defaultDateOnly.AddDays(-3))
            .Build();
        Save(installment4);
        var fakeRequestedLoan1 = new RequestedLoanBuilder()
            .WithFinancialInformationId(financialInfo3.Id)
            .WithRequestedLoanStatus(RequestedLoanStatus.Rejected)
            .Build();
        Save(fakeRequestedLoan1);
        var fakeInstallment = new InstallmentBuilder()
            .WithRequestedLoanId(fakeRequestedLoan1.Id)
            .WithDueDate(_defaultDateOnly.AddMonths(-10))
            .Build();
        Save(fakeInstallment);
        var fakeInstallment2 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan3.Id)
            .WithDueDate(_defaultDateOnly)
            .Build();
        Save(fakeInstallment2);
        var fakeInstallment3 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan3.Id)
            .WithDueDate(_defaultDateOnly)
            .WithPaymentDate(_defaultDateOnly)
            .Build();
        Save(fakeInstallment3);


        var result = await _sut.GetAllHighRiskCustomersReport();

        var excepted = ReadContext.Set<Customer>();
        result.Should().HaveCount(1);
        result.All(r => excepted.Any(e => r.Id == e.Id)).Should().BeTrue();
        var exceptedCustomer1 = excepted
            .Where(c => c.Id == customer1.Id)
            .Select(c => new GetHighRisckCustomerDto()
            {
                Id = c.Id,
                Name = c.Name,
                LastName = c.LastName,
                NationalCode = c.NationalCode,
                PhoneNumber = c.PhoneNumber,
                Email = c.Email,
                IsVerified = c.IsVerified,
                LatePayedInstallmentsCount = 2
            }).Single();
        result.Single().Should().BeEquivalentTo(exceptedCustomer1);
    }

    [Theory]
    [InlineData(RequestedLoanStatus.Approved)]
    [InlineData(RequestedLoanStatus.Rejected)]
    [InlineData(RequestedLoanStatus.Pending)]
    public async Task
        GetMonthlyInterestAndLatePaymentFeeRevenueReportByDateTime_get_all_revenues_from_interest_and_late_fee_in_last_month_by_date_time_properly(
            RequestedLoanStatus fakeStatus)
    {
        var dateTime = _defaultDateOnly.ToDateTime(new TimeOnly(0, 0));
        var loan1 = new LoanBuilder()
            .Build();
        Save(loan1);
        var loan2 = new LoanBuilder()
            .Build();
        Save(loan2);
        var requestedLoan1 = new RequestedLoanBuilder()
            .WithLoanId(loan1.Id)
            .WithRequestedLoanStatus(RequestedLoanStatus.DelayedRefunding)
            .Build();
        Save(requestedLoan1);
        var installment1 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan1.Id)
            .WithDueDate(_defaultDateOnly)
            .WithPaymentDate(_defaultDateOnly.AddDays(1))
            .WithPayableAmount(loan1.InstallmentAmount + loan1.LatePaymentFee)
            .Build();
        Save(installment1);
        var installment2 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan1.Id)
            .WithDueDate(_defaultDateOnly.AddDays(-1))
            .Build();
        Save(installment2);
        var requestedLoan2 = new RequestedLoanBuilder()
            .WithLoanId(loan1.Id)
            .WithRequestedLoanStatus(RequestedLoanStatus.Refunding)
            .Build();
        Save(requestedLoan2);
        var installment3 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan2.Id)
            .WithDueDate(_defaultDateOnly)
            .WithPaymentDate(_defaultDateOnly)
            .WithPayableAmount(loan1.InstallmentAmount)
            .Build();
        Save(installment3);
        var requestedLoan3 = new RequestedLoanBuilder()
            .WithLoanId(loan2.Id)
            .WithRequestedLoanStatus(RequestedLoanStatus.DelayedRefunding)
            .Build();
        Save(requestedLoan3);
        var installment4 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan3.Id)
            .WithDueDate(_defaultDateOnly.AddMonths(-1))
            .WithPaymentDate(_defaultDateOnly)
            .WithPayableAmount(loan2.InstallmentAmount + loan2.LatePaymentFee)
            .Build();
        Save(installment4);
        var installment5 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan3.Id)
            .WithDueDate(_defaultDateOnly.AddMonths(-2))
            .WithPaymentDate(_defaultDateOnly.AddMonths(-1))
            .Build();
        Save(installment5);
        var installment6 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan3.Id)
            .WithDueDate(_defaultDateOnly)
            .WithPaymentDate(_defaultDateOnly.AddMonths(1))
            .Build();
        Save(installment6);
        var fakeRequestedLoan = new RequestedLoanBuilder()
            .WithLoanId(loan2.Id)
            .WithRequestedLoanStatus(fakeStatus)
            .Build();
        Save(fakeRequestedLoan);
        var fakeInstallment = new InstallmentBuilder()
            .WithRequestedLoanId(fakeRequestedLoan.Id)
            .WithDueDate(_defaultDateOnly.AddDays(4))
            .WithPaymentDate(_defaultDateOnly.AddDays(2))
            .Build();
        Save(fakeInstallment);


        var result = await _sut
            .GetMonthlyInterestAndLatePaymentFeeRevenueReportByDateTime(
                dateTime);


        var exceptedInterest =
            (installment3.PayableAmount -
             (Math.Round(loan1.Amount / loan1.InstallmentCount, 2))) +
            (installment1.PayableAmount -
             (Math.Round(loan1.Amount / loan1.InstallmentCount, 2)) -
             loan1.LatePaymentFee) +
            (installment4.PayableAmount -
             (Math.Round(loan2.Amount / loan2.InstallmentCount, 2)) -
             loan2.LatePaymentFee);
        var exceptedLatePaymentFee =
            loan1.LatePaymentFee + loan2.LatePaymentFee;
        var exceptedResult = new GetMonthlyRevenueReportDto()
        {
            FromDate = _defaultDateOnly,
            ToDate = _defaultDateOnly.AddMonths(1),
            TotalInterestRevenue = exceptedInterest,
            TotalLatePaymentFeeRevenue = exceptedLatePaymentFee
        };
        result.Should().BeEquivalentTo(exceptedResult);
    }

    [Fact]
    public async Task
        GetMonthlyInterestAndLatePaymentFeeRevenueReportByDateTime_when_not_found_any_result_properly()
    {
        var dateTime = _defaultDateOnly.ToDateTime(new TimeOnly(0, 0));

        var result = await _sut
            .GetMonthlyInterestAndLatePaymentFeeRevenueReportByDateTime(
                dateTime);

        result.Should().BeEquivalentTo(new GetMonthlyRevenueReportDto()
        {
            FromDate = _defaultDateOnly,
            ToDate = _defaultDateOnly.AddMonths(1),
            TotalInterestRevenue = 0,
            TotalLatePaymentFeeRevenue = 0
        });
    }

    [Theory]
    [InlineData(RequestedLoanStatus.DelayedRefunding)]
    [InlineData(RequestedLoanStatus.Approved)]
    [InlineData(RequestedLoanStatus.Refunding)]
    [InlineData(RequestedLoanStatus.Rejected)]
    [InlineData(RequestedLoanStatus.Pending)]
    public async Task
        GetAllClosedRequestedLoans_get_all_requested_loans_with_closed_status_properly(
            RequestedLoanStatus fakeStatus)
    {
        var loan1 = new LoanBuilder()
            .WithInstallmentCount(2)
            .Build();
        Save(loan1);
        var requestedLoan1 = new RequestedLoanBuilder()
            .WithLoanId(loan1.Id)
            .WithRequestedLoanStatus(RequestedLoanStatus.Closed)
            .Build();
        Save(requestedLoan1);
        var installment1 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan1.Id)
            .WithDueDate(_defaultDateOnly.AddDays(-1))
            .WithPaymentDate(_defaultDateOnly)
            .Build();
        Save(installment1);
        var installment2 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan1.Id)
            .WithDueDate(_defaultDateOnly)
            .WithPaymentDate(_defaultDateOnly.AddDays(1))
            .Build();
        Save(installment2);
        var loan2 = new LoanBuilder()
            .WithAmount(100)
            .WithInstallmentCount(2)
            .Build();
        Save(loan2);
        var requestedLoan2 = new RequestedLoanBuilder()
            .WithLoanId(loan2.Id)
            .WithRequestedLoanStatus(RequestedLoanStatus.Closed)
            .Build();
        Save(requestedLoan2);
        var installment3 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan2.Id)
            .WithDueDate(_defaultDateOnly)
            .WithPaymentDate(_defaultDateOnly.AddDays(-5))
            .Build();
        Save(installment3);
        var installment4 = new InstallmentBuilder()
            .WithRequestedLoanId(requestedLoan2.Id)
            .WithDueDate(_defaultDateOnly)
            .WithPaymentDate(_defaultDateOnly)
            .Build();
        Save(installment4);
        var fakeRequestedLoan = new RequestedLoanBuilder()
            .WithLoanId(loan1.Id)
            .WithRequestedLoanStatus(fakeStatus)
            .Build();
        Save(fakeRequestedLoan);
        var fakeInstallment1 = new InstallmentBuilder()
            .WithRequestedLoanId(fakeRequestedLoan.Id)
            .WithDueDate(_defaultDateOnly)
            .Build();
        Save(fakeInstallment1);
        var fakeInstallment2 = new InstallmentBuilder()
            .WithRequestedLoanId(fakeRequestedLoan.Id)
            .WithDueDate(_defaultDateOnly)
            .WithPaymentDate(_defaultDateOnly.AddDays(2))
            .Build();
        Save(fakeInstallment2);


        var result = await _sut.GetAllClosedRequestedLoans();


        var excepted = ReadContext.Set<RequestedLoan>();

        result.Should().HaveCount(2);
        result.All(r => excepted.Any(e =>
                r.Id == e.Id &&
                e.RequestedLoanStatus == RequestedLoanStatus.Closed)).Should()
            .BeTrue();
        result.Should().Contain(r =>
            r.Id == requestedLoan1.Id &&
            r.LoanAmount == loan1.Amount &&
            r.InstallmentsCount == loan1.InstallmentCount &&
            r.TotalLatePayedFees == (2 * loan1.LatePaymentFee));
        var exceptedRequestedLoan2 = excepted
            .Where(r => r.Id == requestedLoan2.Id)
            .Select(r => new GetClosedRequestedLoanDto()
            {
                Id = requestedLoan2.Id,
                LoanAmount = loan2.Amount,
                InstallmentsCount = loan2.InstallmentCount,
                TotalLatePayedFees = 0
            })
            .Single();
        result.Single(r => r.Id == requestedLoan2.Id).Should()
            .BeEquivalentTo(exceptedRequestedLoan2);
    }
}