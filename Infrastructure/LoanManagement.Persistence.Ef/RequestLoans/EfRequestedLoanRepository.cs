using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.VisualBasic;

namespace LoanManagement.Persistence.Ef.RequestLoans;

public class EfRequestedLoanRepository(EfDataContext context)
    : RequestedLoanRepository
{
    public async Task Add(RequestedLoan requestedLoan)
    {
        await context.Set<RequestedLoan>().AddAsync(requestedLoan);
    }

    public async Task<bool> IsExistByLoanIdAndCustomerId(int loanId,
        int customerId)
    {
        var financialInformationList = context.Set<FinancialInformation>()
            .Where(f => f.CustomerId == customerId)
            .Select(f => f.Id);
        return await context.Set<RequestedLoan>().AnyAsync(r =>
            r.LoanId == loanId &&
            financialInformationList.Contains(r.FinancialInformationId) &&
            r.RequestedLoanStatus != RequestedLoanStatus.Rejected);
    }

    public async Task<GetAllRequstedLoansSummaryDto>
        GetAllRequestedLoansSummaryByCustomerId(int customerId)
    {
        var query = (
            from financialInfo in context.Set<FinancialInformation>()
            where financialInfo.CustomerId == customerId
            join requestedLoan in context.Set<RequestedLoan>()
                on financialInfo.Id equals requestedLoan.FinancialInformationId
            where requestedLoan.RequestedLoanStatus ==
                  RequestedLoanStatus.Closed
            join installment in context.Set<Installment>()
                on requestedLoan.Id equals installment.RequestedLoanId
            select new
            {
                RequestedLoan = requestedLoan,
                Installment = installment
            });

        var queryList = await query.ToListAsync();

        var allOnTimeLoansCount = queryList
            .GroupBy(q => q.RequestedLoan)
            .Where(g =>
                g.All(q => q.Installment.DueDate <= q.Installment.PaymentDate))
            .Count();

        var delayedInstallmentsCount = queryList
            .Where(q => q.Installment.DueDate > q.Installment.PaymentDate)
            .Count();

        return new GetAllRequstedLoansSummaryDto()
        {
            TotalAllOnTimeClosedLoansCount = allOnTimeLoansCount,
            TotalDelayedInstallmentsCount = delayedInstallmentsCount
        };
    }

    public async Task<RequestedLoan?> FindById(int id)
    {
        return await context.Set<RequestedLoan>()
            .SingleOrDefaultAsync(r => r.Id == id);
    }

    public async Task<bool> IsExistActiveLoanByFinancialInformationId(
        int financialInformationId)
    {
        var financialInformation = await context.Set<FinancialInformation>()
            .SingleOrDefaultAsync(f => f.Id == financialInformationId);
        if (financialInformation == null)
        {
            return false;
        }

        var customerId = financialInformation.CustomerId;

        var query = (from financialInfo in context.Set<FinancialInformation>()
            where financialInfo.CustomerId == customerId
            join requestedLoan in context.Set<RequestedLoan>()
                on financialInfo.Id equals requestedLoan.FinancialInformationId
            select requestedLoan.RequestedLoanStatus);

        return await query.AnyAsync(r =>
            r == RequestedLoanStatus.Approved ||
            r == RequestedLoanStatus.Refunding ||
            r == RequestedLoanStatus.DelayedRefunding);
    }

    public async Task<GetRequestedLoanSummaryDto?> GetInfoById(int id)
    {
        return await context.Set<RequestedLoan>()
            .Where(r => r.Id == id)
            .Select(r => new GetRequestedLoanSummaryDto()
            {
                LoanId = r.LoanId,
                FinancialInformationId = r.FinancialInformationId,
                RequestedLoanStatus = r.RequestedLoanStatus
            })
            .SingleOrDefaultAsync();
    }

    public async Task<bool> IsExistAnyDelayedByIdAndDateOnly(int id,
        DateOnly nowUtc)
    {
        return await context.Set<Installment>()
            .AnyAsync(i => i.RequestedLoanId == id &&
                           (i.PaymentDate == null && i.DueDate < nowUtc) ||
                           (i.PaymentDate != null &&
                            i.PaymentDate > i.DueDate));
    }

    public async Task<bool> IsRequestClosedById(int id)
    {
        return await context.Set<Installment>().AllAsync(i =>
            i.RequestedLoanId == id && i.PaymentDate != null);
    }
}