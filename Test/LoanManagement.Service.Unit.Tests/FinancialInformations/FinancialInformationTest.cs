namespace LoanManagement.Service.Unit.Test.FinancialInformations;

public class FinancialInformationTest : BusinessIntegrationTest
{
    private readonly FinancialInformationService _sut;

    public FinancialInformationTest()
    {
        _sut = FinancialInformationServiceFactory.Create(SetupContext);
    }

    [Fact]
    public async Task
        CreateByCustomerId_create_and_add_financial_information_properly()
    {
        var isDeleted = false;
        var customer = new CustomerBuilder().Build();
        Save(customer);
        var dto = new AddFinancialInformationDtoBuilder()
            .Build();

        var actual = await _sut.CreateByCustomerId(customer.Id, dto);

        var excepted = ReadContext.Set<FinancialInformation>();
        excepted.Single().Should().BeEquivalentTo(new FinancialInformation()
        {
            Id = actual,
            CustomerId = customer.Id,
            JobType = dto.JobType,
            MonthlyIncome = dto.MonthlyIncome,
            FinancialAssets = dto.FinancialAssets,
            IsDeleted = isDeleted
        });
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task
        CreateByCustomerId_throw_exception_when_customer_not_found(
            int fakeCustomerId)
    {
        var dto = new AddFinancialInformationDtoBuilder()
            .Build();

        var actual = () => _sut.CreateByCustomerId(fakeCustomerId, dto);

        await actual.Should().ThrowExactlyAsync<CustomerNotFoundException>();
        ReadContext.Set<FinancialInformation>().Should().HaveCount(0);
    }

    [Fact]
    public async Task
        CreateByCustomerId_throw_exception_when_financial_information_already_exist()
    {
        var customer = new CustomerBuilder().Build();
        Save(customer);
        var financialInfo = new FinancialInformationBuilder()
            .WithCustomerId(customer.Id)
            .Build();
        Save(financialInfo);
        var dto = new AddFinancialInformationDtoBuilder().Build();

        var actual = () => _sut.CreateByCustomerId(customer.Id, dto);

        await actual.Should()
            .ThrowExactlyAsync<FinancialInformationIsAlreadyCreated>();
        ReadContext.Set<FinancialInformation>().Should().HaveCount(1);
    }

    [Fact]
    public async Task
        CreateByCustomerId_throw_exception_when_monthly_income_is_less_than_zero()
    {
        var customer = new CustomerBuilder()
            .Build();
        Save(customer);
        var dto = new AddFinancialInformationDtoBuilder()
            .WithMonthlyIncome(-1)
            .Build();
        var actual = () => _sut.CreateByCustomerId(customer.Id, dto);

        await actual.Should()
            .ThrowExactlyAsync<InvalidMonthlyIncomeException>();
    }

    [Fact]
    public async Task
        CreateByCustomerId_throw_exception_when_financial_assets_is_less_than_zero()
    {
        var customer = new CustomerBuilder()
            .Build();
        Save(customer);
        var dto = new AddFinancialInformationDtoBuilder()
            .WithFinancialAssets(-1)
            .Build();
        var actual = () => _sut.CreateByCustomerId(customer.Id, dto);

        await actual.Should()
            .ThrowExactlyAsync<InvalidFinancialAssetsException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public async Task
        CreateByCustomerId_throw_exception_when_job_type_is_invalid(
            int fakeStatusIndex)
    {
        var customer = new CustomerBuilder().Build();
        Save(customer);
        var dto = new AddFinancialInformationDtoBuilder()
            .WithJobType((JobType)fakeStatusIndex)
            .Build();

        var actual = () => _sut.CreateByCustomerId(customer.Id, dto);

        await actual.Should().ThrowExactlyAsync<InvalidJobTypeException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task
        UpdateByCustomerId_deactivate_last_financial_info_and_add_a_new_active_with_changes_by_customer_id_properly(
            int index)
    {
        JobType newJobType = JobType.GovernmentEmployee;
        decimal mewMonthlyIncome = 1234321;
        decimal newFinancialAssets = 1234321;
        var customer = new CustomerBuilder().Build();
        Save(customer);
        var financialInfo = new FinancialInformationBuilder()
            .WithCustomerId(customer.Id)
            .Build();
        Save(financialInfo);
        var dtos = new List<UpdateFinancialInformationDto>()
        {
            new UpdateFinancialInformationDto()
            {
                JobType = newJobType,
                MonthlyIncome = null,
                FinancialAssets = null
            },
            new UpdateFinancialInformationDto()
            {
                JobType = null,
                MonthlyIncome = mewMonthlyIncome,
                FinancialAssets = newFinancialAssets
            },
        };


        var actual = await _sut.UpdateByCustomerId(customer.Id, dtos[index]);

        var excepted = ReadContext.Set<FinancialInformation>();
        excepted.Should().HaveCount(2)
            .And.Contain(f =>
                f.Id == financialInfo.Id &&
                f.IsDeleted == true &&
                f.CustomerId == customer.Id)
            .And.Contain(f =>
                f.Id == actual &&
                f.IsDeleted == false &&
                f.CustomerId == customer.Id);
        var exceptedList = new List<FinancialInformation>()
        {
            new FinancialInformation()
            {
                Id = actual,
                CustomerId = customer.Id,
                IsDeleted = false,
                JobType = newJobType,
                MonthlyIncome = financialInfo.MonthlyIncome,
                FinancialAssets = financialInfo.FinancialAssets
            },
            new FinancialInformation()
            {
                Id = actual,
                CustomerId = customer.Id,
                IsDeleted = false,
                JobType = financialInfo.JobType,
                MonthlyIncome = mewMonthlyIncome,
                FinancialAssets = newFinancialAssets
            }
        };

        excepted.Single(f => f.Id == actual).Should()
            .BeEquivalentTo(exceptedList[index]);
    }

    [Fact]
    public async Task
        UpdateByCustomerId_throw_exception_when_nothing_to_changes()
    {
        var customer = new CustomerBuilder().Build();
        Save(customer);
        var financialInfo = new FinancialInformationBuilder()
            .WithCustomerId(customer.Id)
            .Build();
        Save(financialInfo);
        var dto = new UpdateFinancialInformationDtoBuilder()
            .WithJobType(null)
            .WithMonthlyIncome(null)
            .WithFinancialAsserts(financialInfo.FinancialAssets)
            .Build();

        var actual = () => _sut.UpdateByCustomerId(customer.Id, dto);

        await actual.Should()
            .ThrowExactlyAsync<NothingToChangeFinancialInformationException>();
        ReadContext.Set<FinancialInformation>().Single().IsDeleted.Should()
            .BeFalse();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task
        UpdateByCustomerId_throw_exception_when_customer_not_found(
            int fakeCustomerId)
    {
        var dto = new UpdateFinancialInformationDtoBuilder().Build();

        var actual = () => _sut.UpdateByCustomerId(fakeCustomerId, dto);

        await actual.Should().ThrowExactlyAsync<CustomerNotFoundException>();
    }

    [Fact]
    public async Task
        UpdateByCustomerId_throw_exception_when_not_found_any_financial_information_to_update()
    {
        var customer = new CustomerBuilder().Build();
        Save(customer);
        var dto = new UpdateFinancialInformationDtoBuilder().Build();

        var actual = () => _sut.UpdateByCustomerId(customer.Id, dto);

        await actual.Should()
            .ThrowExactlyAsync<FinancialInformationNotFoundException>();
    }

    [Fact]
    public async Task
        UpdateByCustomerId_throw_exception_when_monthly_income_is_less_than_zero()
    {
        var dto = new UpdateFinancialInformationDtoBuilder()
            .WithMonthlyIncome(-1)
            .Build();

        var actual = () => _sut.UpdateByCustomerId(1, dto);

        await actual.Should()
            .ThrowExactlyAsync<InvalidMonthlyIncomeException>();
    }

    [Fact]
    public async Task
        UpdateByCustomerId_throw_exception_when_financial_assets_is_less_than_zero()
    {
        var dto = new UpdateFinancialInformationDtoBuilder()
            .WithFinancialAsserts(-1)
            .Build();

        var actual = () => _sut.UpdateByCustomerId(1, dto);

        await actual.Should()
            .ThrowExactlyAsync<InvalidFinancialAssetsException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(4)]
    public async Task
        UpdateByCustomerId_throw_exception_when_job_type_is_invalid(
            int fakeStatusIndex)
    {
        var customer = new CustomerBuilder().Build();
        Save(customer);
        var financialInfo = new FinancialInformationBuilder()
            .WithCustomerId(customer.Id)
            .Build();
        Save(financialInfo);
        var dto = new UpdateFinancialInformationDtoBuilder()
            .WithJobType((JobType)fakeStatusIndex)
            .Build();

        var actual = () => _sut.UpdateByCustomerId(customer.Id, dto);

        await actual.Should().ThrowExactlyAsync<InvalidJobTypeException>();
    }
}