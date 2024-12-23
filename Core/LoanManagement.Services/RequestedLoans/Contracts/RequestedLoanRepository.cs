namespace LoanManagement.Services.RequestedLoans.Contracts;

public interface RequestedLoanRepository
{
    Task Add(RequestedLoan requestedLoan);
    Task<bool> IsExistByLoanIdAndCustomerId(int loanId, int customerId);

    Task<GetAllRequstedLoansSummaryDto> GetAllRequestedLoansSummaryByCustomerId(
        int customerId);

    Task<RequestedLoan?> FindById(int id);

    Task<bool> IsExistActiveLoanByFinancialInformationId(
        int financialInformationId);

    Task<GetRequestedLoanSummaryDto?> GetInfoById(int id);
    Task<bool> IsExistAnyDelayedByIdAndDateOnly(int id, System.DateOnly nowUtc);
    Task<bool> IsRequestClosedById(int id);
}