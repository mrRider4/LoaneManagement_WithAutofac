namespace LoanManagement.Services.Loans.Contracts;

public interface LoanQuery
{
    Task<HashSet<GetLoanDto>> GetLoansWithOptionalTermFilter(
        bool? isShortTerm = null);
}