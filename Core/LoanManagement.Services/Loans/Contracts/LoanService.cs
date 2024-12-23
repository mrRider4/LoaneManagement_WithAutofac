using LoanManagement.Services.Loans.Contracts.DTOs;

namespace LoanManagement.Services.Loans.Contracts;

public interface LoanService
{
    Task<int> Create(AddLoanDto dto);
}