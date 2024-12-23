using FluentAssertions.Extensions;
using LoanManagement.Entities.FinancialInformations.Enums;

namespace LoanManagement.Application.Integration.Test.CustomerHandlers;

public class
    UpdateCustomerFinancialInformationCommandHandlerTest :
    BusinessIntegrationTest
{
    private readonly UpdateCustomerFinancialInformationHandler _sut;
    private readonly Mock<CustomerService> _customerService;

    private readonly Mock<FinancialInformationService>
        _financialInformationService;

    public UpdateCustomerFinancialInformationCommandHandlerTest()
    {
        _customerService = new Mock<CustomerService>();
        _financialInformationService = new Mock<FinancialInformationService>();
        Mock<UnitOfWork> unitOfWork = new();
        _sut = new UpdateCustomerFinancialInformationCommandHandler(
            unitOfWork.Object,
            _customerService.Object,
            _financialInformationService.Object);
    }

    [Fact]
    public async Task
        Handle_customer_financial_information_and_unverified_customer_properly()
    {
        int newFinancialInfoId = 1;
        var customer = new CustomerBuilder()
            .WithIsVerified(true)
            .Build();
        Save(customer);
        var financialInfo = new FinancialInformationBuilder()
            .WithCustomerId(customer.Id)
            .Build();
        Save(financialInfo);
        var command = new UpdateFinancialInformationCommand()
        {
            CustomerId = customer.Id,
            JobType = JobType.GovernmentEmployee,
            MonthlyIncome = 1000,
            FinancialAssets = 2000
        };
        _financialInformationService
            .Setup(f => f.UpdateByCustomerId(command.CustomerId,
                It.Is<UpdateFinancialInformationDto>(dto =>
                    dto.JobType == command.JobType &&
                    dto.MonthlyIncome == command.MonthlyIncome &&
                    dto.FinancialAssets == command.FinancialAssets)))
            .ReturnsAsync(newFinancialInfoId);

        var actual = await _sut.Handle(command);

        _financialInformationService.Verify(f =>
            f.UpdateByCustomerId(command.CustomerId,
                It.Is<UpdateFinancialInformationDto>(dto =>
                    dto.JobType == command.JobType &&
                    dto.MonthlyIncome == command.MonthlyIncome &&
                    dto.FinancialAssets == command.FinancialAssets)));
        _customerService.Verify(c =>
            c.DisableVerificationById(command.CustomerId));
        actual.GetHashCode().December(newFinancialInfoId);
    }
}