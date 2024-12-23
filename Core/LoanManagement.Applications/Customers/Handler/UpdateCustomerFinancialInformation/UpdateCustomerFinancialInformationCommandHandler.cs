namespace LoanManagement.Applications.Customers.Handler.
    UpdateCustomerFinancialInformation;

public class
    UpdateCustomerFinancialInformationCommandHandler(
        UnitOfWork unitOfWork,
        CustomerService customerService,
        FinancialInformationService financialInformationService) :
    UpdateCustomerFinancialInformationHandler
{
    public async Task<int> Handle(UpdateFinancialInformationCommand command)
    {
        await unitOfWork.Begin();
        try
        {
            var dto = new UpdateFinancialInformationDto()
            {
                JobType = command.JobType,
                MonthlyIncome = command.MonthlyIncome,
                FinancialAssets = command.FinancialAssets
            };
            var financialInfoId =
                await financialInformationService.UpdateByCustomerId(
                    command.CustomerId, dto);
            await customerService.DisableVerificationById(command.CustomerId);

            await unitOfWork.Commit();

            return financialInfoId;
        }
        catch (Exception e)
        {
            await unitOfWork.Rollback();
            throw;
        }
    }
}