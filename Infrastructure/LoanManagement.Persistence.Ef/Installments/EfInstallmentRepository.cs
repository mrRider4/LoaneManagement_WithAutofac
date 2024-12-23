namespace LoanManagement.Persistence.Ef.Installments;

public class EfInstallmentRepository(EfDataContext context)
    : InstallmentRepository
{
    public async Task AddRange(HashSet<Installment> installments)
    {
        await context.Set<Installment>().AddRangeAsync(installments);
    }

    public async Task<Installment> FirstNotPayedInstallmentByRequestId(
        int requestedLoanId)
    {
        return await context.Set<Installment>()
            .Where(i =>
                i.RequestedLoanId == requestedLoanId && i.PaymentDate == null)
            .OrderBy(i => i.Id).FirstAsync();
    }
}