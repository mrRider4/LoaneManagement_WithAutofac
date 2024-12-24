namespace LoanManagement.Services.Customers.Contracts;

public interface CustomerRepository : Repository
{
    Task Add(Customer customer);
    Task<bool> IsExistByNationalCode(string nationalCode);
    Task<Customer?> FindById(int id);
    Task<bool> IsExistById(int id);
    Task<GetCustomerInformationDto?> GetInfoById(int id);
}