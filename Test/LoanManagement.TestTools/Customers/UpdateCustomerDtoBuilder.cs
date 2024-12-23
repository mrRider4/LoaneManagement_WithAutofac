namespace LoanManagement.TestTools.Customers;

public class UpdateCustomerDtoBuilder
{
    private readonly UpdateCustomerDto _updateCustomerDto;
    public UpdateCustomerDtoBuilder()
    {
        _updateCustomerDto = new UpdateCustomerDto()
        {
            Id = -1,
            Name = "new_dummy__name",
            LastName = "new_dummy_LastName",
            NationalCode = "new_456789",
            PhoneNumber = "new_987654",
            Email = "new_dummy_Email@"
        };
    }

    public UpdateCustomerDtoBuilder WithId(int id)
    {
        _updateCustomerDto.Id = id;
        return this;
    }

    public UpdateCustomerDtoBuilder WithNationalCode(string nationalCode)
    {
        _updateCustomerDto.NationalCode = nationalCode;
        return this;
    }

    public UpdateCustomerDtoBuilder WithPhoneNumber(string phoneNumber)
    {
        _updateCustomerDto.PhoneNumber = phoneNumber;
        return this;
    }

    public UpdateCustomerDto Build()
    {
        return _updateCustomerDto;
    }
}