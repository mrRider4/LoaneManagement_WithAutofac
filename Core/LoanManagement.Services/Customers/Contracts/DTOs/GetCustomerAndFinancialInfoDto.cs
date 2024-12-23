namespace LoanManagement.Services.Customers.Contracts;

public class GetCustomerAndFinancialInfoDto
{
    public int Id { get; set; }
    public bool IsVerified { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string NationalCode { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public GetFinancialInformationDto? FinancialInformation { get; set; }
}