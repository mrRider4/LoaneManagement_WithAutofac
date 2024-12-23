namespace LoanManagement.Service.Unit.Test.Customers;

public class CustomerQueryTest : BusinessIntegrationTest
{
    private readonly CustomerQuery _sut;

    public CustomerQueryTest()
    {
        _sut = CustomerQueryFactory.Create(SetupContext);
    }

    [Fact]
    public async Task
        GetCustomersWithOptionalVerificationFilter_get_all_customers_properly()
    {
        bool? verificationFilter = null;
        var customer = new CustomerBuilder()
            .WithIsVerified(true)
            .Build();
        Save(customer);
        var customer2 = new CustomerBuilder()
            .WithIsVerified(false)
            .Build();
        Save(customer2);
        var financialInfo = new FinancialInformationBuilder()
            .WithCustomerId(customer.Id)
            .Build();
        Save(financialInfo);

        var result =
            await _sut.GetCustomersWithOptionalVerificationFilter(
                verificationFilter);

        var excepted = ReadContext.Set<Customer>();
        var exceptedCustomer = excepted.Where(c => c.Id == customer.Id)
            .Select(c =>
                new GetCustomerAndFinancialInfoDto()
                {
                    Id = c.Id,
                    Name = c.Name,
                    LastName = c.LastName,
                    NationalCode = c.NationalCode,
                    PhoneNumber = c.PhoneNumber,
                    Email = c.Email,
                    IsVerified = c.IsVerified,
                    FinancialInformation = new GetFinancialInformationDto()
                    {
                        JobType = financialInfo.JobType,
                        MonthlyIncome = financialInfo.MonthlyIncome,
                        FinancialAssets = financialInfo.FinancialAssets
                    }
                }).Single();
        result.Should().HaveCount(excepted.Count());
        result.Should().Contain(c =>
            c.Id == customer2.Id &&
            c.FinancialInformation == null &&
            c.IsVerified == false);
        result.Single(c => c.Id == customer.Id).Should()
            .BeEquivalentTo(exceptedCustomer);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task
        GetCustomersWithOptionalVerificationFilter_get_all_customers_by_filter_properly(
            bool verificationFilter)
    {
        var customer = new CustomerBuilder()
            .WithIsVerified(true)
            .Build();
        Save(customer);
        var customer2 = new CustomerBuilder()
            .WithIsVerified(false)
            .Build();
        Save(customer2);

        var result =
            await _sut.GetCustomersWithOptionalVerificationFilter(
                verificationFilter);

        var excepted = ReadContext.Set<Customer>()
            .Where(c => c.IsVerified == verificationFilter);
        result.Should().HaveCount(excepted.Count())
            .And.Contain(c => c.IsVerified == verificationFilter)
            .And.NotContain(c => c.IsVerified == !verificationFilter);
    }
}