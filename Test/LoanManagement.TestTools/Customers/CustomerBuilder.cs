namespace LoanManagement.TestTools.Customers;

public class CustomerBuilder
{
    private readonly Customer _customer;

    public CustomerBuilder()
    {
        _customer = new Customer()
        {
            Name = "dummy_name",
            LastName = "dummy_lastName",
            NationalCode = "dummy_nCod",
            PhoneNumber = "dummy_pNum",
            Email = "dummy_email@",
            IsVerified = false
        };
    }

    public CustomerBuilder WithNationalCode(string nationalCode)
    {
        _customer.NationalCode = nationalCode;
        return this;
    }

    public CustomerBuilder WithPhoneNumber(string phoneNumber)
    {
        _customer.PhoneNumber = phoneNumber;
        return this;
    }


    public CustomerBuilder WithIsVerified(bool isActive)
    {
        _customer.IsVerified = isActive;
        return this;
    }

    public Customer Build()
    {
        return _customer;
    }
}