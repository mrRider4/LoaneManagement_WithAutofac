namespace LoanManagement.Applications.RequestedLoans.Handlers.ApproveRequestAndCreateInstallments.Contracts;

public interface ApproveRequestAndCreateInstallmentsHandler
{
    Task Handel(int requestedLoanId);
}