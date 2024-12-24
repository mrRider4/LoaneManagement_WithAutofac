namespace LoanManagement.Services.Loans.Contracts;

public interface LoanQuery : Repository
{
    Task<HashSet<GetLoanDto>> GetLoansWithOptionalTermFilter(
        bool? isShortTerm = null);
}