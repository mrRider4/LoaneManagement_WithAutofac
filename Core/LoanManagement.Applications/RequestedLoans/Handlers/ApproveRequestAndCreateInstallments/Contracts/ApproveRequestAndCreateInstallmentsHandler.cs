namespace LoanManagement.Applications.RequestedLoans.Handlers.ApproveRequestAndCreateInstallments.Contracts;

public interface ApproveRequestAndCreateInstallmentsHandler:Service
{
    Task Handel(int requestedLoanId);
}