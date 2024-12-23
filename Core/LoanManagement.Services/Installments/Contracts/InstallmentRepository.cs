using LoanManagement.Entities.Installments;

namespace LoanManagement.Services.Installments.Contracts;

public interface InstallmentRepository
{
    Task AddRange(HashSet<Installment> installments);
    Task<Installment> FirstNotPayedInstallmentByRequestId(int requestedLoanId);
}