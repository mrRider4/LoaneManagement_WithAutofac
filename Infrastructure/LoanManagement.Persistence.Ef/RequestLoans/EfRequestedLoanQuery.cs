namespace LoanManagement.Persistence.Ef.RequestLoans;

public class EfRequestedLoanQuery(
    EfDataContext context,
    DateOnlyService dateOnlyService) : RequestedLoanQuery
{
    public async Task<HashSet<GetRequestedLoanDto>> GetAllPending()
    {
        var query = await context.Set<RequestedLoan>()
            .Where(r => r.RequestedLoanStatus == RequestedLoanStatus.Pending)
            .Select(r => new GetRequestedLoanDto()
            {
                Id = r.Id,
                LoanId = r.LoanId,
                FinancialInformationId = r.FinancialInformationId,
                RequestedDate = r.RequestedDate,
                RequestedLoanStatus = r.RequestedLoanStatus,
                CreditRate = r.CreditRate
            }).ToListAsync();
        return query.ToHashSet();
    }

    public async Task<HashSet<GetRequestedLoanAndPaymentSummaryDto>>
        GetRefundingAndDelayedRefundingLoansReport()
    {
        var statusFilter = new List<RequestedLoanStatus>()
        {
            RequestedLoanStatus.Approved,
            RequestedLoanStatus.Refunding,
            RequestedLoanStatus.DelayedRefunding
        };
        var query = (
                from loan in context.Set<Loan>()
                join request in context.Set<RequestedLoan>()
                    on loan.Id equals request.LoanId
                where statusFilter.Contains(request.RequestedLoanStatus)
                join installment in context.Set<Installment>()
                    on request.Id equals installment.RequestedLoanId
                select new
                {
                    Loan = loan,
                    Request = request,
                    Installment = installment
                }).GroupBy(q => q.Request)
            .Select(g => new
            {
                Request = g.Key,
                Loann = g.First().Loan,
                Installments = g.Select(gg => gg.Installment).ToList()
            });

        var data = await query.ToListAsync();

        var result = data
            .Select(r => new GetRequestedLoanAndPaymentSummaryDto()
            {
                Id = r.Request.Id,
                LoanId = r.Request.LoanId,
                FinancialInformationId = r.Request.FinancialInformationId,
                RequestedDate = r.Request.RequestedDate,
                RequestedLoanStatus = r.Installments.Any(i =>
                    (i.PaymentDate == null &&
                     i.DueDate < dateOnlyService.NowUtc) ||
                    (i.PaymentDate != null &&
                     i.DueDate < i.PaymentDate))
                    ? RequestedLoanStatus.DelayedRefunding
                    : RequestedLoanStatus.Refunding,
                CreditRate = r.Request.CreditRate,
                LoanAmount = r.Loann.Amount,
                InstallmentCount = r.Loann.InstallmentCount,
                PayedAmount = r.Installments
                    .Where(i => i.PaymentDate != null)
                    .Sum(i => i.PayableAmount),
                RemainingInstallments = r.Installments
                    .Where(i => i.PaymentDate == null)
                    .Select(i => new GetInstallmentDto()
                    {
                        Id = i.Id,
                        DueDate = i.DueDate,
                        PaymentDate = i.PaymentDate,
                        PayableAmount = i.DueDate > dateOnlyService.NowUtc
                            ? i.PayableAmount
                            : i.PayableAmount + r.Loann.LatePaymentFee
                    }).ToHashSet()
            }).ToHashSet();


        return result;
    }

    public async Task<HashSet<GetHighRisckCustomerDto>>
        GetAllHighRiskCustomersReport()
    {
        var query = await (
                from customer in context.Set<Customer>()
                join financialInfo in context.Set<FinancialInformation>()
                    on customer.Id equals financialInfo.CustomerId
                join request in context.Set<RequestedLoan>()
                    on financialInfo.Id equals request.FinancialInformationId
                where request.RequestedLoanStatus !=
                      RequestedLoanStatus.Rejected
                join installment in context.Set<Installment>()
                    on request.Id equals installment.RequestedLoanId
                where (installment.PaymentDate == null &&
                       installment.DueDate < dateOnlyService.NowUtc) ||
                      (installment.PaymentDate != null &&
                       installment.DueDate < installment.PaymentDate)
                select new
                {
                    Customer = customer,
                    Instalment = installment
                })
            .GroupBy(q => q.Customer)
            .Select(g => new
            {
                Customer = g.Key,
                LatePayedInstallmentsCount = g.Select(r => r.Instalment).Count()
            }).ToListAsync();

        var result = query
            .Where(q => q.LatePayedInstallmentsCount >= 2)
            .Select(q => new GetHighRisckCustomerDto()
            {
                Id = q.Customer.Id,
                Name = q.Customer.Name,
                LastName = q.Customer.LastName,
                NationalCode = q.Customer.NationalCode,
                PhoneNumber = q.Customer.PhoneNumber,
                Email = q.Customer.Email,
                IsVerified = q.Customer.IsVerified,
                LatePayedInstallmentsCount = q.LatePayedInstallmentsCount
            }).ToHashSet();

        return result;
    }

    public async Task<GetMonthlyRevenueReportDto>
        GetMonthlyInterestAndLatePaymentFeeRevenueReportByDateTime(
            DateTime dateTime)
    {
        var fromDate = DateOnly.FromDateTime(dateTime);
        var statuses = new HashSet<RequestedLoanStatus>()
        {
            RequestedLoanStatus.Refunding,
            RequestedLoanStatus.DelayedRefunding,
            RequestedLoanStatus.Closed,
        };

        var query = (
                from request in context.Set<RequestedLoan>()
                where statuses.Contains(request.RequestedLoanStatus)
                join installment in context.Set<Installment>()
                    on request.Id equals installment.RequestedLoanId
                where installment.PaymentDate != null &&
                      installment.PaymentDate >= fromDate &&
                      installment.PaymentDate < fromDate.AddMonths(1)
                join loan in context.Set<Loan>()
                    on request.LoanId equals loan.Id
                select new
                {
                    Unit = "theUnifier",
                    TotalInterestRevenue =
                        // installment.DueDate >= installment.PaymentDate?
                        loan.InstallmentAmount -
                        (Math.Round(loan.Amount / loan.InstallmentCount, 2))
                    // : 0
                    ,
                    TotalLatePaymentFeeRevenue =
                        installment.DueDate < installment.PaymentDate
                            ? loan.LatePaymentFee
                            : 0
                })
            .GroupBy(q => q.Unit)
            .Select(g => new GetMonthlyRevenueReportDto()
            {
                FromDate = fromDate,
                ToDate = fromDate.AddMonths(1),
                TotalInterestRevenue =
                    g.Select(r => r.TotalInterestRevenue).Sum(),
                TotalLatePaymentFeeRevenue =
                    g.Select(r => r.TotalLatePaymentFeeRevenue).Sum()
            });

        var result = await query.FirstOrDefaultAsync();

        return result ?? new GetMonthlyRevenueReportDto
        {
            FromDate = fromDate,
            ToDate = fromDate.AddMonths(1),
            TotalInterestRevenue = 0,
            TotalLatePaymentFeeRevenue = 0
        };
    }

    public async Task<HashSet<GetClosedRequestedLoanDto>>
        GetAllClosedRequestedLoans()
    {
        var query = await (
                from request in context.Set<RequestedLoan>()
                where request.RequestedLoanStatus == RequestedLoanStatus.Closed
                join installment in context.Set<Installment>()
                    on request.Id equals installment.RequestedLoanId
                join loan in context.Set<Loan>()
                    on request.LoanId equals loan.Id
                select new
                {
                    Idd = request.Id,
                    Loan = loan,
                    Installment = installment,
                })
            .GroupBy(q => q.Idd)
            .Select(g => new
            {
                Id = g.Key,
                LoanAmount = g.First().Loan.Amount,
                InstallmentsCount = g.First().Loan.InstallmentCount,
                LatePaymentFeee = g.First().Loan.LatePaymentFee,
                Installments = g.Select(r => r.Installment).ToList()
            }).ToListAsync();

        var result = query.Select(q => new GetClosedRequestedLoanDto()
        {
            Id = q.Id,
            LoanAmount = q.LoanAmount,
            InstallmentsCount = q.InstallmentsCount,
            TotalLatePayedFees =
                (q.Installments.Where(i => i.DueDate < i.PaymentDate)
                    .Count()) * q.LatePaymentFeee
        }).ToHashSet();

        return result;
    }
}