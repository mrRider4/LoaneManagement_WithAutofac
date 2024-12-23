namespace LoanManagement.Entities.Installments;

public class Installment
{
    public int Id { get; set; }
    public int RequestedLoanId { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? PaymentDate { get; set; }
    public decimal PayableAmount { get; set; }
}