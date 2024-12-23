namespace LoanManagement.Persistence.Ef.Loans;

public class EfLoanQuery(EfDataContext context) : LoanQuery
{
    public async Task<HashSet<GetLoanDto>> GetLoansWithOptionalTermFilter(
        bool? isShortTerm = null)
    {
        var query = context.Set<Loan>()
            .Select(l => new GetLoanDto()
            {
                Id = l.Id,
                Amount = l.Amount,
                AnnualInterestPercentage = l.AnnualInterestPercentage,
                InstallmentCount = l.InstallmentCount,
                InstallmentAmount = l.InstallmentAmount,
                LatePaymentFee = l.LatePaymentFee
            });
        if (isShortTerm != null)
        {
            query = query.Where(l =>
                isShortTerm.Value
                    ? l.InstallmentCount <= 12
                    : l.InstallmentCount > 12);
        }

        var result = await query.ToListAsync();
        return result.ToHashSet();
    }
}