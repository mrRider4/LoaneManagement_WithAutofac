namespace LoanManagement.Services.Installments.Contracts;

public interface InstallmentService : Service
{
    Task CreatRangeByRequestedLoanId(int requestedLoanId);
    Task UpdatePaymentDateAndAmountByRequestId(int requestedLoanId);
}