using LoanManagement.Entities.RequestedLoans;
using LoanManagement.Entities.RequestedLoans.Enums;

namespace LoanManagement.TestTools.RequestedLoans;

public class RequestedLoanBuilder
{
    private readonly RequestedLoan _requestedLoan;

    public RequestedLoanBuilder()
    {
        _requestedLoan = new RequestedLoan()
        {
            LoanId = -1,
            FinancialInformationId = -1,
            RequestedDate = new DateOnly(2024, 1, 1),
            RequestedLoanStatus = RequestedLoanStatus.Pending,
            CreditRate = 60,
        };
    }

    public RequestedLoanBuilder WithLoanId(int id)
    {
        _requestedLoan.LoanId = id;
        return this;
    }

    public RequestedLoanBuilder WithFinancialInformationId(int id)
    {
        _requestedLoan.FinancialInformationId = id;
        return this;
    }

    public RequestedLoanBuilder WithRequestedLoanStatus(
        RequestedLoanStatus status)
    {
        _requestedLoan.RequestedLoanStatus = status;
        return this;
    }

    public RequestedLoanBuilder WithCreditRate(int rate)
    {
        _requestedLoan.CreditRate = rate;
        return this;
    }

    public RequestedLoan Build()
    {
        return _requestedLoan;
    }
}