using LoanManagement.Entities.Installments;

namespace LoanManagement.Services.Installments;

public class InstallmentAppService(
    UnitOfWork unitOfWork,
    InstallmentRepository repository,
    DateOnlyService dateOnlyService,
    LoanRepository loanRepository,
    RequestedLoanRepository requestedLoanRepository) : InstallmentService
{
    public async Task CreatRangeByRequestedLoanId(int requestedLoanId)
    {
        var requestedLoan =
            await requestedLoanRepository.GetInfoById(requestedLoanId);
        if (requestedLoan == null)
        {
            throw new RequestedLoanNotFoundException();
        }

        var loan = await loanRepository.GetInfoById(requestedLoan.LoanId);
        if (loan == null)
        {
            throw new LoanNotFoundException();
        }

        HashSet<Installment> installments = new HashSet<Installment>();
        for (int i = 1; i <= loan!.InstallmentCount; i++)
        {
            installments.Add(new Installment()
            {
                RequestedLoanId = requestedLoanId,
                DueDate = dateOnlyService.NowUtc.AddMonths(i),
                PaymentDate = null,
                PayableAmount = loan.InstallmentAmount
            });
        }

        await repository.AddRange(installments);
        await unitOfWork.Save();
    }

    public async Task UpdatePaymentDateAndAmountByRequestId(int requestedLoanId)
    {
        var requestedLoan =
            await requestedLoanRepository.GetInfoById(requestedLoanId);
        if (requestedLoan == null)
        {
            throw new RequestedLoanNotFoundException();
        }

        if (requestedLoan.RequestedLoanStatus == RequestedLoanStatus.Pending ||
            requestedLoan.RequestedLoanStatus == RequestedLoanStatus.Rejected)
        {
            throw new ThisLoanIsNotReceivedException();
        }

        if (requestedLoan.RequestedLoanStatus == RequestedLoanStatus.Closed)
        {
            throw new RequestedLoanIsAlreadyClosedException();
        }

        var loan = await loanRepository.GetInfoById(requestedLoan.LoanId);
        if (loan == null)
        {
            throw new LoanNotFoundException();
        }

        var installment =
            await repository.FirstNotPayedInstallmentByRequestId(
                requestedLoanId);
        installment.PaymentDate = dateOnlyService.NowUtc;
        if (installment.DueDate < installment.PaymentDate)
        {
            installment.PayableAmount += loan.LatePaymentFee;
        }

        await unitOfWork.Save();
    }
}