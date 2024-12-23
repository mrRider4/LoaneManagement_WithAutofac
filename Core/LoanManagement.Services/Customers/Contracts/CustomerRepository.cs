namespace LoanManagement.Services.Customers.Contracts;

public interface CustomerRepository
{
    Task Add(Customer customer);
    Task<bool> IsExistByNationalCode(string nationalCode);
    Task<Customer?> FindById(int id);
    Task<bool> IsExistById(int id);
    Task<GetCustomerInformationDto?> GetInfoById(int id);
}