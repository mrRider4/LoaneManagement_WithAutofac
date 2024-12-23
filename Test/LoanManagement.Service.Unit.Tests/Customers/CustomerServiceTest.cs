using NuGet.Frameworks;

namespace LoanManagement.Service.Unit.Test.Customers;

public class CustomerServiceTest : BusinessIntegrationTest
{
    private readonly CustomerService _sut;

    public CustomerServiceTest()
    {
        _sut = CustomerServiceFactory.Create(SetupContext);
    }

    [Fact]
    public async Task Create_create_customer_properly()
    {
        var isVerified = false;
        var dto = new AddCustomerDtoBuilder().Build();

        var actual = await _sut.Create(dto);

        var excepted = ReadContext.Set<Customer>();
        excepted.Should().HaveCount(1);
        excepted.Single().Should().BeEquivalentTo(new Customer()
        {
            Id = actual,
            Name = dto.Name,
            LastName = dto.LastName,
            NationalCode = dto.NationalCode,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            IsVerified = isVerified
        });
    }

    [Theory]
    [InlineData("12345678900")]
    [InlineData("123456789")]
    public async Task
        Create_throw_exception_when_national_code_length_is_invalid(
            string invalidNationalCode)
    {
        var dto = new AddCustomerDtoBuilder()
            .WithNationalCode(invalidNationalCode)
            .Build();

        var actual = () => _sut.Create(dto);

        await actual.Should()
            .ThrowExactlyAsync<InvalidNationalCodeLengthException>();
        ReadContext.Set<Customer>().Should().HaveCount(0);
    }

    [Theory]
    [InlineData("12345678900")]
    [InlineData("123456789")]
    public async Task
        Create_throw_exception_when_phone_number_length_is_invalid(
            string invalidPhoneNumber)
    {
        var dto = new AddCustomerDtoBuilder()
            .WithPhoneNumber(invalidPhoneNumber)
            .Build();

        var actual = () => _sut.Create(dto);

        await actual.Should()
            .ThrowExactlyAsync<InvalidPhoneNumberLengthException>();
        ReadContext.Set<Customer>().Should().HaveCount(0);
    }

    [Theory]
    [InlineData("0123456789")]
    [InlineData("1122334455")]
    public async Task Create_throw_exception_when_national_code_duplicated(
        string sameNationalCode)
    {
        var customer = new CustomerBuilder().WithNationalCode(sameNationalCode)
            .Build();
        Save(customer);
        var dto = new AddCustomerDtoBuilder().WithNationalCode(sameNationalCode)
            .Build();

        var actual = () => _sut.Create(dto);

        await actual.Should()
            .ThrowExactlyAsync<CustomerNationalCodeDuplicateException>();
        ReadContext.Set<Customer>().Should().HaveCount(1);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public async Task Update_update_customer_properly(int index)
    {
        var isVerified = false;
        var customer = new CustomerBuilder().WithIsVerified(true).Build();
        Save(customer);
        var dtos = new List<UpdateCustomerDto>()
        {
            new UpdateCustomerDto()
            {
                Id = customer.Id,
                Name = null,
                LastName = null,
                NationalCode = "dummy_5678",
                PhoneNumber = "dummy_1234",
                Email = "dummy_@"
            },
            new UpdateCustomerDto()
            {
                Id = customer.Id,
                Name = "dummy_new_name",
                LastName = "dummy_new_lastName",
                NationalCode = null,
                PhoneNumber = null,
                Email = null
            },
        };

        await _sut.Update(dtos[index]);

        var excepted = ReadContext.Set<Customer>();
        excepted.Should().HaveCount(1);
        var exceptedList = new List<Customer>()
        {
            new Customer()
            {
                Id = customer.Id,
                Name = customer.Name,
                LastName = customer.LastName,
                NationalCode = dtos[0].NationalCode,
                PhoneNumber = dtos[0].PhoneNumber,
                Email = dtos[0].Email,
                IsVerified = isVerified
            },
            new Customer()
            {
                Id = customer.Id,
                Name = dtos[1].Name,
                LastName = dtos[1].LastName,
                NationalCode = customer.NationalCode,
                PhoneNumber = customer.PhoneNumber,
                Email = customer.Email,
                IsVerified = isVerified
            },
        };
        excepted.Single().Should().BeEquivalentTo(exceptedList[index]);
        // excepted.Single().Should().BeEquivalentTo(new Customer()
        // {
        //     Id = customer.Id,
        //     Name = string.IsNullOrWhiteSpace(dto.Name)
        //         ? customer.Name
        //         : dto.Name,
        //     LastName = string.IsNullOrWhiteSpace(dto.LastName)
        //         ? customer.LastName
        //         : dto.LastName,
        //     NationalCode = string.IsNullOrWhiteSpace(dto.NationalCode)
        //         ? customer.NationalCode
        //         : dto.NationalCode,
        //     PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber)
        //         ? customer.PhoneNumber
        //         : dto.PhoneNumber,
        //     Email = string.IsNullOrWhiteSpace(dto.Email)
        //         ? customer.Email
        //         : dto.Email,
        //     IsVerified = isActive
        // });
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(-22)]
    public async Task Update_throw_exception_when_customer_not_found(int fakeId)
    {
        var dto = new UpdateCustomerDtoBuilder().WithId(fakeId).Build();

        var actual = () => _sut.Update(dto);

        await actual.Should().ThrowExactlyAsync<CustomerNotFoundException>();
        ReadContext.Set<Customer>().Should().HaveCount(0);
    }

    [Theory]
    [InlineData("1213141516")]
    [InlineData("2123242526")]
    public async Task Update_throw_exception_when_national_code_duplicated(
        string duplicateNationalCode)
    {
        var customer = new CustomerBuilder()
            .Build();
        Save(customer);
        var customer2 = new CustomerBuilder()
            .WithNationalCode(duplicateNationalCode)
            .Build();
        Save(customer2);
        var dto = new UpdateCustomerDtoBuilder()
            .WithNationalCode(duplicateNationalCode)
            .WithId(customer.Id)
            .Build();

        var actual = () => _sut.Update(dto);

        await actual.Should()
            .ThrowExactlyAsync<CustomerNationalCodeDuplicateException>();
        ReadContext.Set<Customer>().Should().HaveCount(2);
    }

    [Theory]
    [InlineData("1213141516")]
    [InlineData("2123242526")]
    public async Task
        Update_update_customer_when_national_code_remains_the_same_properly(
            string sameNationalCode)
    {
        var isActive = false;
        var customer = new CustomerBuilder()
            .WithIsVerified(true)
            .WithNationalCode(sameNationalCode)
            .Build();
        Save(customer);
        var dto = new UpdateCustomerDtoBuilder()
            .WithId(customer.Id)
            .WithNationalCode(sameNationalCode)
            .Build();

        await _sut.Update(dto);

        var excepted = ReadContext.Set<Customer>();
        excepted.Should().HaveCount(1);
        excepted.Single().Should().BeEquivalentTo(new Customer()
        {
            Id = customer.Id,
            Name = string.IsNullOrWhiteSpace(dto.Name)
                ? customer.Name
                : dto.Name,
            LastName = string.IsNullOrWhiteSpace(dto.LastName)
                ? customer.LastName
                : dto.LastName,
            NationalCode = string.IsNullOrWhiteSpace(dto.NationalCode)
                ? customer.NationalCode
                : dto.NationalCode,
            PhoneNumber = string.IsNullOrWhiteSpace(dto.PhoneNumber)
                ? customer.PhoneNumber
                : dto.PhoneNumber,
            Email = string.IsNullOrWhiteSpace(dto.Email)
                ? customer.Email
                : dto.Email,
            IsVerified = isActive
        });
    }

    [Theory]
    [InlineData("123456789")]
    [InlineData("01234567890")]
    public async Task
        Update_throw_exception_when_national_code_length_is_invalid(
            string invalidNationalCode)
    {
        var customer = new CustomerBuilder()
            .Build();
        Save(customer);
        var dto = new UpdateCustomerDtoBuilder()
            .WithId(customer.Id)
            .WithNationalCode(invalidNationalCode)
            .Build();

        var actual = () => _sut.Update(dto);

        await actual.Should()
            .ThrowExactlyAsync<InvalidNationalCodeLengthException>();
        ReadContext.Set<Customer>().Should().HaveCount(1);
    }

    [Theory]
    [InlineData("123456789")]
    [InlineData("01234567890")]
    public async Task
        Update_throw_exception_when_phone_number_length_is_invalid(
            string invalidPhoneNumber)
    {
        var customer = new CustomerBuilder()
            .Build();
        Save(customer);
        var dto = new UpdateCustomerDtoBuilder()
            .WithId(customer.Id)
            .WithPhoneNumber(invalidPhoneNumber)
            .Build();

        var actual = () => _sut.Update(dto);

        await actual.Should()
            .ThrowExactlyAsync<InvalidPhoneNumberLengthException>();
        ReadContext.Set<Customer>().Should().HaveCount(1);
    }

    [Fact]
    public async Task Update_throw_exception_when_nothing_to_changes()
    {
        var customer = new CustomerBuilder()
            .WithIsVerified(true)
            .Build();
        Save(customer);
        var dto = new UpdateCustomerDto()
        {
            Id = customer.Id,
            Name = customer.Name
        };

        var actual = () => _sut.Update(dto);

        await actual.Should()
            .ThrowExactlyAsync<NothingToChangeCustomerException>();
        ReadContext.Set<Customer>().Single().IsVerified.Should().BeTrue();
    }

    [Fact]
    public async Task
        EnableVerificationById_make_isVerified_equal_to_true_properly()
    {
        var customer = new CustomerBuilder().Build();
        Save(customer);
        var financialInfo = new FinancialInformationBuilder()
            .WithCustomerId(customer.Id)
            .Build();
        Save(financialInfo);

        await _sut.EnableVerificationById(customer.Id);

        var actual = ReadContext.Set<Customer>();
        actual.Should().HaveCount(1);
        actual.Single().IsVerified.Should().BeTrue();
        ReadContext.Set<FinancialInformation>().Should().HaveCount(1);
    }


    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task
        EnableVerificationById_throw_exception_when_Customer_not_found(
            int fakeCustomerId)
    {
        var actual = () =>
            _sut.EnableVerificationById(fakeCustomerId);

        await actual.Should().ThrowExactlyAsync<CustomerNotFoundException>();
        ReadContext.Set<Customer>().Should().HaveCount(0)
            .And.NotContain(c => c.IsVerified == true);
    }

    [Fact]
    public async Task
        EnableVerificationById_throw_exception_when_Customer_has_no_active_financial_info()
    {
        var customer = new CustomerBuilder().Build();
        Save(customer);
        var customer2 = new CustomerBuilder().Build();
        Save(customer2);
        var financialInfo = new FinancialInformationBuilder()
            .WithCustomerId(customer.Id)
            .WithIsDeleted(true)
            .Build();
        Save(financialInfo);
        var financialInfo2 = new FinancialInformationBuilder()
            .WithCustomerId(customer2.Id)
            .Build();
        Save(financialInfo2);

        var actual = () =>
            _sut.EnableVerificationById(customer.Id);

        await actual.Should()
            .ThrowExactlyAsync<FinancialInformationNotFoundException>();
        ReadContext.Set<Customer>().Should().HaveCount(2).And
            .NotContain(c => c.IsVerified == true);
        ReadContext.Set<FinancialInformation>().Should().HaveCount(2);
    }

    [Fact]
    public async Task
        DisableVerificationById_make_isVerified_equal_to_false_properly()
    {
        var customer = new CustomerBuilder()
            .WithIsVerified(true)
            .Build();
        Save(customer);

        await _sut.DisableVerificationById(customer.Id);

        var excepted = ReadContext.Set<Customer>();
        excepted.Single().IsVerified.Should().BeFalse();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task
        DisableVerificationById_throw_exception_when_customer_not_found(
            int fakeCustomerId)
    {
        var customer = new CustomerBuilder()
            .WithIsVerified(true)
            .Build();
        Save(customer);

        var actual = () => _sut.DisableVerificationById(fakeCustomerId);

        await actual.Should().ThrowExactlyAsync<CustomerNotFoundException>();
        ReadContext.Set<Customer>().Single().IsVerified.Should().BeTrue();
    }
}