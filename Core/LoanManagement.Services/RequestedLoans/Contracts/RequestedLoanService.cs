namespace LoanManagement.Services.RequestedLoans.Contracts;

public interface RequestedLoanService
{
    Task<int> Create(int loanId, int customerId);
    Task ApproveById(int id);
    Task UpdateReceivedLoanStatusById(int id);
}