namespace LoanManagement.restApi.Controllers.RequestedLoans
{
    [Route("api/RequestedLoans")]
    [ApiController]
    public class RequestedLoansController(
        RequestedLoanService service,
        RequestedLoanQuery query,
        ApproveRequestAndCreateInstallmentsHandler
            approveRequestAndCreateInstallmentsHandler,
        PayAndDeterminingRequestedLoanStatusHandler
            payAndDeterminingRequestedLoanStatusHandler) : ControllerBase
    {
        [HttpPost("create")]
        public async Task<int> Create([FromQuery] int loanId = 0,
            int customerId = 0)
        {
            return await service.Create(loanId, customerId);
        }

        [HttpPatch("approve_by/{requestedLoanId}")]
        public async Task Approve(
            [FromRoute] int requestedLoanId)
        {
            await approveRequestAndCreateInstallmentsHandler.Handel(
                requestedLoanId);
        }

        [HttpPatch("pay_installment_by/{requestedLoanId}")]
        public async Task PayInstallment(int requestedLoanId)
        {
            await payAndDeterminingRequestedLoanStatusHandler.Handle(
                requestedLoanId);
        }

        [HttpGet("all_with_pending_status")]
        public async Task<HashSet<GetRequestedLoanDto>> GetAllPending()
        {
            return await query.GetAllPending();
        }

        [HttpGet("all_repayment_requests_status")]
        public async Task<HashSet<GetRequestedLoanAndPaymentSummaryDto>>
            GetRefundingAndDelayedRefundingLoansReport()
        {
            return await query.GetRefundingAndDelayedRefundingLoansReport();
        }

        [HttpGet("all_high_risk_customers")]
        public async Task<HashSet<GetHighRisckCustomerDto>>
            GetAllHighRiskCustomersReport()
        {
            return await query.GetAllHighRiskCustomersReport();
        }

        [HttpGet("monthly_revenue_by_date")]
        public async Task<GetMonthlyRevenueReportDto>
            GetMonthlyInterestAndLatePaymentFeeRevenueReportByDateTime(
                [FromQuery] DateTime dateTime)
        {
            return await query
                .GetMonthlyInterestAndLatePaymentFeeRevenueReportByDateTime(
                    dateTime);
        }

        [HttpGet("all_closed_requests")]
        public async Task<HashSet<GetClosedRequestedLoanDto>>
            GetAllClosedRequestedLoans()
        {
            return await query.GetAllClosedRequestedLoans();
        }
    }
}