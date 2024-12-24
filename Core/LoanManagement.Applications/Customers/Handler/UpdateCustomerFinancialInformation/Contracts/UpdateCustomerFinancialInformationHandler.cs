namespace LoanManagement.Applications.Customers.Handler.UpdateCustomerFinancialInformation.Contracts;

public interface UpdateCustomerFinancialInformationHandler:Service
{
    Task<int> Handle(UpdateFinancialInformationCommand command);
}