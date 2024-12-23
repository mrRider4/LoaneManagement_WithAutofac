namespace LoanManagement.Services.RequestedLoans.Contracts;

public class GetHighRisckCustomerDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string LastName { get; set; }
    public required string NationalCode { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Email { get; set; }
    public bool IsVerified { get; set; }

    public int LatePayedInstallmentsCount { get; set; }
}