namespace LoanManagement.Services.FinancialInformations;

public class FinancialInformationAppService(
    UnitOfWork unitOfWork,
    FinancialInformationRepository repository,
    CustomerRepository customerRepository)
    : FinancialInformationService
{
    public async Task<int> CreateByCustomerId(int customerId,
        AddFinancialInformationDto dto)
    {
        if (dto.MonthlyIncome < 0)
        {
            throw new InvalidMonthlyIncomeException();
        }

        if (dto.FinancialAssets < 0)
        {
            throw new InvalidFinancialAssetsException();
        }

        if (!Enum.IsDefined(typeof(JobType), dto.JobType))
        {
            throw new InvalidJobTypeException();
        }

        await StopIfCustomerNotFound(customerId);
        if (await repository.IsExistByCustomerIdAndIsDeletedFilter(customerId))
        {
            throw new FinancialInformationIsAlreadyCreated();
        }

        var financialInformation = new FinancialInformation()
        {
            CustomerId = customerId,
            JobType = dto.JobType,
            MonthlyIncome = dto.MonthlyIncome,
            FinancialAssets = dto.FinancialAssets,
            IsDeleted = false
        };
        await repository.Add(financialInformation);
        await unitOfWork.Save();
        return financialInformation.Id;
    }


    public async Task<int> UpdateByCustomerId(int customerId,
        UpdateFinancialInformationDto dto)
    {
        if (dto.MonthlyIncome < 0)
        {
            throw new InvalidMonthlyIncomeException();
        }

        if (dto.FinancialAssets < 0)
        {
            throw new InvalidFinancialAssetsException();
        }

        if (dto.JobType != null &&
            !Enum.IsDefined(typeof(JobType), dto.JobType))
        {
            throw new InvalidJobTypeException();
        }

        await StopIfCustomerNotFound(customerId);
        if (!await repository
                .IsExistByCustomerIdAndIsDeletedFilter(customerId))
        {
            throw new FinancialInformationNotFoundException();
        }

        var lastFinancialInfo =
            await repository.FindLastOneByCustomerId(customerId);
        StopWhenNotingToChange(lastFinancialInfo, dto);
        var newFinancialInfo = new FinancialInformation()
        {
            CustomerId = customerId,
            JobType = dto.JobType ?? lastFinancialInfo.JobType,
            MonthlyIncome =
                dto.MonthlyIncome ?? lastFinancialInfo.MonthlyIncome,
            FinancialAssets = dto.FinancialAssets ??
                              lastFinancialInfo.FinancialAssets,
            IsDeleted = false
        };
        lastFinancialInfo.IsDeleted = true;
        await repository.Add(newFinancialInfo);
        await unitOfWork.Save();
        return newFinancialInfo.Id;
    }

    private static void StopWhenNotingToChange(
        FinancialInformation lastFinancialInfo,
        UpdateFinancialInformationDto dto)
    {
        if ((dto.JobType == null || dto.JobType == lastFinancialInfo.JobType) &&
            (dto.MonthlyIncome == null ||
             dto.MonthlyIncome == lastFinancialInfo.MonthlyIncome) &&
            (dto.FinancialAssets == null || dto.FinancialAssets ==
                lastFinancialInfo.FinancialAssets))
        {
            throw new NothingToChangeFinancialInformationException();
        }
    }

    private async Task StopIfCustomerNotFound(int customerId)
    {
        if (!await customerRepository.IsExistById(customerId))
            throw new CustomerNotFoundException();
    }
}