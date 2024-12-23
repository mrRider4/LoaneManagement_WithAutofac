namespace LoanManagement.TestTools.Customers;

public class AddCustomerDtoBuilder
{
    private readonly AddCustomerDto _addCustomerDto;

    public AddCustomerDtoBuilder()
    {
        _addCustomerDto = new AddCustomerDto()
        {
            Name = "Dummy",
            LastName = "Last_Dummy",
            PhoneNumber = "9876543210",
            NationalCode = "0123456789",
            Email = "Dummy@"
        };
    }

    public AddCustomerDtoBuilder WithNationalCode(string nationalCode)
    {
        _addCustomerDto.NationalCode = nationalCode;
        return this;
    }

    public AddCustomerDtoBuilder WithPhoneNumber(string phoneNumber)
    {
        _addCustomerDto.PhoneNumber = phoneNumber;
        return this;
    }

    public AddCustomerDto Build()
    {
        return _addCustomerDto;
    }
}