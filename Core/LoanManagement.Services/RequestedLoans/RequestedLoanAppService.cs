namespace LoanManagement.Services.RequestedLoans;

public class RequestedLoanAppService(
    UnitOfWork unitOfWork,
    RequestedLoanRepository repository,
    DateOnlyService dateOnlyService,
    CustomerRepository customerRepository,
    LoanRepository loanRepository,
    RequestedLoanCalculator requestedLoanCalculator) : RequestedLoanService
{
    public async Task<int> Create(int loanId, int customerId)
    {
        var customerInfo = await customerRepository.GetInfoById(customerId);
        if (customerInfo == null)
        {
            throw new CustomerNotFoundException();
        }

        if (!customerInfo.IsVerified)
        {
            throw new CustomerIsNotVerifiedException();
        }

        if (customerInfo.FinancialInformationId == null)
        {
            throw new FinancialInformationNotFoundException();
        }

        var loan = await loanRepository.GetInfoById(loanId);

        if (loan == null)
        {
            throw new LoanNotFoundException();
        }

        if (await repository.IsExistByLoanIdAndCustomerId(loanId, customerId))
        {
            throw new TheLoanHasAlreadyTakenOnceByThisCustomerException();
        }

        var requestedLoansSummary =
            await repository
                .GetAllRequestedLoansSummaryByCustomerId(customerId);
        var creditRate =
            requestedLoanCalculator.DeterminateCreditRate(customerInfo,
                loan.Amount, requestedLoansSummary);
        var requestedLoanStatus =
            requestedLoanCalculator.DeterminateStatusOnAdd(creditRate);
        var requestedLoan = new RequestedLoan()
        {
            LoanId = loanId,
            FinancialInformationId = customerInfo.FinancialInformationId.Value,
            RequestedDate = dateOnlyService.NowUtc,
            CreditRate = creditRate,
            RequestedLoanStatus = requestedLoanStatus
        };
        await repository.Add(requestedLoan);
        await unitOfWork.Save();
        return requestedLoan.Id;
    }

    public async Task ApproveById(int id)
    {
        var requestedLoan = await repository.FindById(id);
        if (requestedLoan == null)
        {
            throw new RequestedLoanNotFoundException();
        }

        if (requestedLoan.RequestedLoanStatus != RequestedLoanStatus.Pending)
        {
            throw new RequestLoanIsNotInPendingStatusException();
        }

        if (requestedLoan.CreditRate < 60)
        {
            throw new
                RequestedLoanCreditRateIsLessThanTheRequirementException();
        }

        if (await repository.IsExistActiveLoanByFinancialInformationId(
                requestedLoan.FinancialInformationId))
        {
            throw new TheCustomerAlreadyHasAnActiveReceivedLoanException();
        }

        requestedLoan.RequestedLoanStatus = RequestedLoanStatus.Approved;
        await unitOfWork.Save();
    }

    public async Task UpdateReceivedLoanStatusById(int id)
    {
        var requestedLoan = await repository.FindById(id);
        if (requestedLoan == null)
        {
            throw new RequestedLoanNotFoundException();
        }

        if (requestedLoan.RequestedLoanStatus == RequestedLoanStatus.Closed)
        {
            throw new RequestedLoanIsAlreadyClosedException();
        }

        if ((requestedLoan.RequestedLoanStatus ==
             RequestedLoanStatus.Approved ||
             requestedLoan.RequestedLoanStatus ==
             RequestedLoanStatus.Refunding ||
             requestedLoan.RequestedLoanStatus ==
             RequestedLoanStatus.DelayedRefunding)
            &&
            await repository.IsRequestClosedById(id))
        {
            requestedLoan.RequestedLoanStatus = RequestedLoanStatus.Closed;
        }

        var isExistAnyDelayedByIdAndDateOnly =
            await repository.IsExistAnyDelayedByIdAndDateOnly(
                id,
                dateOnlyService.NowUtc);
        if ((requestedLoan.RequestedLoanStatus ==
             RequestedLoanStatus.Approved ||
             requestedLoan.RequestedLoanStatus ==
             RequestedLoanStatus.Refunding) &&
            isExistAnyDelayedByIdAndDateOnly)
        {
            requestedLoan.RequestedLoanStatus =
                RequestedLoanStatus.DelayedRefunding;
        }

        if (requestedLoan.RequestedLoanStatus == RequestedLoanStatus.Approved &&
            !isExistAnyDelayedByIdAndDateOnly)
        {
            requestedLoan.RequestedLoanStatus = RequestedLoanStatus.Refunding;
        }

        await unitOfWork.Save();
    }
}