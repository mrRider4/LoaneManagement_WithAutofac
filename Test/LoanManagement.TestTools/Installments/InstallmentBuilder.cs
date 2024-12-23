using LoanManagement.Entities.Installments;

namespace LoanManagement.TestTools.Installments;

public class InstallmentBuilder
{
    private readonly Installment _installment;

    public InstallmentBuilder()
    {
        _installment = new Installment()
        {
            RequestedLoanId = -1,
            DueDate = new DateOnly(2024, 1, 1),
            PaymentDate = null,
            PayableAmount = 1000
        };
    }

    public InstallmentBuilder WithRequestedLoanId(int id)
    {
        _installment.RequestedLoanId = id;
        return this;
    }

    public InstallmentBuilder WithDueDate(DateOnly dueDate)
    {
        _installment.DueDate = dueDate;
        return this;
    }

    public InstallmentBuilder WithPaymentDate(DateOnly? paymentDate)
    {
        _installment.PaymentDate = paymentDate;
        return this;
    }

    public InstallmentBuilder WithPayableAmount(decimal payableAmount)
    {
        _installment.PayableAmount = payableAmount;
        return this;
    }

    public Installment Build()
    {
        return _installment;
    }
}