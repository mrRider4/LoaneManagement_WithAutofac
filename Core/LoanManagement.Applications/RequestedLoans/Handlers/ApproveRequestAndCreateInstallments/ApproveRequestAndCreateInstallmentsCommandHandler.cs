namespace LoanManagement.Applications.RequestedLoans.Handlers.
    ApproveRequestAndCreateInstallments;

public class
    ApproveRequestAndCreateInstallmentsCommandHandler(
        UnitOfWork unitOfWork,
        RequestedLoanService requestedLoanService,
        InstallmentService installmentService)
    : ApproveRequestAndCreateInstallmentsHandler
{
    public async Task Handel(int requestedLoanId)
    {
        await unitOfWork.Begin();
        try
        {
            await requestedLoanService.ApproveById(requestedLoanId);
            await installmentService.CreatRangeByRequestedLoanId(
                requestedLoanId);
            await unitOfWork.Commit();
        }
        catch (Exception e)
        {
            await unitOfWork.Rollback();
            throw;
        }
    }
}