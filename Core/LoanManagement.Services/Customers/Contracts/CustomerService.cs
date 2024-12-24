namespace LoanManagement.Services.Customers.Contracts;

public interface CustomerService : Service
{
    Task<int> Create(AddCustomerDto dto);
    Task Update(UpdateCustomerDto dto);
    Task EnableVerificationById(int id);
    Task DisableVerificationById(int id);
}