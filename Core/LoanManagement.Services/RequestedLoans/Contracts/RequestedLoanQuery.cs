namespace LoanManagement.Services.RequestedLoans.Contracts;

public interface RequestedLoanQuery
{
    Task<HashSet<GetRequestedLoanDto>> GetAllPending();

    Task<HashSet<GetRequestedLoanAndPaymentSummaryDto>>
        GetRefundingAndDelayedRefundingLoansReport();

    Task<HashSet<GetHighRisckCustomerDto>> GetAllHighRiskCustomersReport();

    Task<GetMonthlyRevenueReportDto> GetMonthlyInterestAndLatePaymentFeeRevenueReportByDateTime(
            DateTime dateTime);

    Task<HashSet<GetClosedRequestedLoanDto>> GetAllClosedRequestedLoans();
}