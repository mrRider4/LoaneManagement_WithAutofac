namespace LoanManagement.Applications.RequestedLoans.Handlers.
    PayAndDeterminingRequestedLoanStatus;

public class
    PayAndDeterminingRequestedLoanStatusCommandHandler(
        UnitOfWork unitOfWork,
        RequestedLoanService requestedLoanService,
        InstallmentService installmentService) :
    PayAndDeterminingRequestedLoanStatusHandler
{
    public async Task Handle(int requestedLoanId)
    {
        await unitOfWork.Begin();
        try
        {
            await installmentService.UpdatePaymentDateAndAmountByRequestId(
                requestedLoanId);
            await requestedLoanService.UpdateReceivedLoanStatusById(
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