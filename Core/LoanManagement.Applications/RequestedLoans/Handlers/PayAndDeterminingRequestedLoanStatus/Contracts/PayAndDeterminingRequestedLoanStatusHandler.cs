namespace LoanManagement.Applications.RequestedLoans.Handlers.ApproveRequestAndCreateInstallments.Contracts;

public interface PayAndDeterminingRequestedLoanStatusHandler: Service
{
    Task Handle(int requestedLoanId);
}