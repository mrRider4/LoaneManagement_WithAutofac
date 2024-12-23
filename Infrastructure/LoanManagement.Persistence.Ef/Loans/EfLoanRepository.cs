namespace LoanManagement.Persistence.Ef.Loans;

public class EfLoanRepository(EfDataContext context) : LoanRepository
{
    public async Task Add(Loan loan)
    {
        await context.Set<Loan>().AddAsync(loan);
    }

    public async Task<bool> IsExistByAmountAndInstallmentCount(decimal amount,
        int installmentCount)
    {
        return await context.Set<Loan>().AnyAsync(l =>
            l.Amount == amount && l.InstallmentCount == installmentCount);
    }

    public async Task<GetLoanInfoDto?> GetInfoById(int id)
    {
        var query = context.Set<Loan>()
            .Where(l => l.Id == id)
            .Select(l => new GetLoanInfoDto()
            {
                Amount = l.Amount,
                AnnualInterestPercentage = l.AnnualInterestPercentage,
                InstallmentCount = l.InstallmentCount,
                InstallmentAmount = l.InstallmentAmount,
                LatePaymentFee = l.LatePaymentFee
            });

        return await query.SingleOrDefaultAsync();
    }
}