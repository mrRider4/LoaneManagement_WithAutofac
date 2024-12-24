using LoanManagement.Entities.Installments;

namespace LoanManagement.Services.Installments.Contracts;

public interface InstallmentRepository : Repository
{
    Task AddRange(HashSet<Installment> installments);
    Task<Installment> FirstNotPayedInstallmentByRequestId(int requestedLoanId);
}