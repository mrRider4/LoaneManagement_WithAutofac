namespace LoanManagement.Services.RequestedLoans.Contracts;

public class GetInstallmentDto
{
    public int Id { get; set; }
    public System.DateOnly DueDate { get; set; }
    public System.DateOnly? PaymentDate { get; set; }
    public decimal PayableAmount { get; set; }
}