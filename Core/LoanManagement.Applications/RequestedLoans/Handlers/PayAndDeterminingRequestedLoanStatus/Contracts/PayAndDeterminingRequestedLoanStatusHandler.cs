namespace LoanManagement.Applications.RequestedLoans.Handlers.ApproveRequestAndCreateInstallments.Contracts;

public interface PayAndDeterminingRequestedLoanStatusHandler
{
    Task Handle(int requestedLoanId);
}