namespace LoanManagement.Tool.Unit.Tests.RequestedLoanCalculator;

public class RequestedLoanCalculatorTest : BusinessIntegrationTest
{
    private readonly Services.RequestedLoans.Contracts.RequestedLoanCalculator
        _tut;

    public RequestedLoanCalculatorTest()
    {
        _tut = new RequestedLoanCalculatorTool();
    }

    [Fact]
    public void DelayedInstallmentsRate_return_rate_properly()
    {
        var delayedInstallments = 3;

        var actual = _tut.DelayedInstallmentsRate(delayedInstallments);

        var excepted = delayedInstallments * (-5);
        actual.Should().Be(excepted);
    }

    [Fact]
    public void OnTimeClosedLoanRate_return_rate_properly()
    {
        var onTimeClosedLoans = 3;

        var actual = _tut.OnTimeClosedLoanRate(onTimeClosedLoans);

        var excepted = onTimeClosedLoans * (30);
        actual.Should().Be(excepted);
    }

    [Theory]
    [InlineData(1000, 500)]
    [InlineData(2000, 1400)]
    [InlineData(3000, 2101)]
    public void FinancialAssetsRate_return_rate_properly(
        decimal financialAssets, decimal loanAmount)
    {

        var actual = _tut.FinancialAssetsRate(financialAssets, loanAmount);


        var excepted = 0;
        var max = financialAssets / 2;
        var min = (financialAssets / 10) * 7;
        if (max < loanAmount && min >= loanAmount)
        {
            excepted = 10;
        }
        if (max >= loanAmount)
        {
            excepted = 20;
        }
        actual.Should().Be(excepted);
    }

    [Theory]
    [InlineData(JobType.GovernmentEmployee)]
    [InlineData(JobType.SelfEmployee)]
    [InlineData(JobType.Unemployed)]
    public void JobTypeRate_return_rate_properly(JobType jobType)
    {

        var actual = _tut.JobTypeRate(jobType);

        var excepted = 0;
        if (jobType == JobType.GovernmentEmployee)
        {
            excepted = 20;
        }
        if (jobType == JobType.SelfEmployee)
        {
            excepted = 10;
        }
        actual.Should().Be(excepted);
    }

    [Theory]
    [InlineData(100000000)]
    [InlineData(5000000)]
    [InlineData(4999999)]
    public void MonthlyIncomeRate_return_rate_properly(decimal monthlyIncome)
    {

        var actual = _tut.MonthlyIncomeRate(monthlyIncome);

        var excepted = 0;
        var max = 10000000;
        var min = 5000000;
        if (max > monthlyIncome && monthlyIncome >= min)
        {
            excepted = 10;
        }
        if (monthlyIncome >= max)
        {
            excepted = 20;
        }
        actual.Should().Be(excepted);
    }


    [Fact]
    public void DeterminateCreditRate_return_rate_properly()
    {
        var customerInfo = new GetCustomerInformationDto()
        {
            Name = "dummy",
            LastName = "dummy",
            NationalCode = "dummy",
            IsVerified = true,
            FinancialInformationId = -1,
            JobType = JobType.SelfEmployee,
            MonthlyIncome = 6000000,
            FinancialAssets = 100
        };
        var requestedLoansSummary = new GetAllRequstedLoansSummaryDto()
        {
            TotalAllOnTimeClosedLoansCount = 1,
            TotalDelayedInstallmentsCount = 1
        };
        decimal loanAmount = 50;
        
        var actual = _tut.DeterminateCreditRate(
            customerInfo,
            loanAmount,
            requestedLoansSummary);

        actual.Should().Be(65);
    }

    [Theory]
    [InlineData(5,0)]
    [InlineData(0,20)]
    public void DeterminateCreditRate_return_rate_in_1_to_100_range_properly(
        int allOnTimeClosedLoansCount, int allDelayedInstallmentsCount)
    {
        var customerInfo = new GetCustomerInformationDto()
        {
            Name = "dummy",
            LastName = "dummy",
            NationalCode = "dummy",
            IsVerified = true,
            FinancialInformationId = -1,
            JobType = JobType.Unemployed,
            MonthlyIncome = 0,
            FinancialAssets = 0
        };
        var requestedLoansSummary = new GetAllRequstedLoansSummaryDto()
        {
            TotalAllOnTimeClosedLoansCount = allOnTimeClosedLoansCount,
            TotalDelayedInstallmentsCount = allDelayedInstallmentsCount
        };
        decimal loanAmount = 1000000;

        var actual = _tut.DeterminateCreditRate(
            customerInfo,
            loanAmount,
            requestedLoansSummary);
        var excepted = 0;
        if (allOnTimeClosedLoansCount>0)
        {
            excepted = 100;
        }
        if (allDelayedInstallmentsCount>0)
        {
            excepted = 1;
        }
        actual.Should().Be(excepted);
    }

    [Theory]
    [InlineData(60)]
    [InlineData(59)]
    public void DeterminateStatusOnAdd_return_status_properly(int creditRate)
    {

        var actual = _tut.DeterminateStatusOnAdd(creditRate);
        
        var excepted = RequestedLoanStatus.Rejected;
        if (creditRate >= 60)
        {
            excepted = RequestedLoanStatus.Pending;
        }
        actual.Should().Be(excepted);
    }
}