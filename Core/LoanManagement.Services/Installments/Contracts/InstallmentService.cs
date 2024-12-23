namespace LoanManagement.Services.Installments.Contracts;

public interface InstallmentService
{
    Task CreatRangeByRequestedLoanId(int requestedLoanId);
    Task UpdatePaymentDateAndAmountByRequestId(int requestedLoanId);
}