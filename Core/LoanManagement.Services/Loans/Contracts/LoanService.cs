using LoanManagement.Services.Loans.Contracts.DTOs;

namespace LoanManagement.Services.Loans.Contracts;

public interface LoanService : Service
{
    Task<int> Create(AddLoanDto dto);
}