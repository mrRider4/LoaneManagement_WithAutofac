namespace LoanManagement.Services.Customers.Contracts;

public class AddCustomerDto
{
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required  string NationalCode { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
}