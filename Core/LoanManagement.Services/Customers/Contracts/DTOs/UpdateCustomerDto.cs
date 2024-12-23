namespace LoanManagement.Services.Customers.Contracts;

public class UpdateCustomerDto
{
    public int Id { get; set; }
    public string? Name { get; set; } = null;
    public string? LastName { get; set; } = null;
    public string? NationalCode { get; set; } = null;
    public string? PhoneNumber { get; set; } = null;
    public string? Email { get; set; } = null;
}