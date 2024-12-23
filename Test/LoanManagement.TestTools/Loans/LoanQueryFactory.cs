namespace LoanManagement.TestTools.Loans;

public static class LoanQueryFactory
{
    public static LoanQuery Create(EfDataContext context)
    {
        return new EfLoanQuery(context);
    }
}