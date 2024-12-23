namespace LoanManagement.Applications.Customers.Handler.UpdateCustomerFinancialInformation.Contracts;

public interface UpdateCustomerFinancialInformationHandler
{
    Task<int> Handle(UpdateFinancialInformationCommand command);
}