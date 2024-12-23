namespace LoanManagement.TestTools.RequestedLoans;

public static class RequestedLoanQueryFactory
{
    public static RequestedLoanQuery Create(
        EfDataContext context,
        DateOnlyService? dateOnlyService = null)
    {
        dateOnlyService ??= new DateOnlyAppService();
        return new EfRequestedLoanQuery(context, dateOnlyService);
    }
}