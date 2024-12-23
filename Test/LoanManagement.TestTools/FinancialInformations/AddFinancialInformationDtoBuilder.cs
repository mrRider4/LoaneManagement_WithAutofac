namespace LoanManagement.TestTools.FinancialInformations;

public class AddFinancialInformationDtoBuilder
{
    private readonly AddFinancialInformationDto _addFinancialInformationDto;

    public AddFinancialInformationDtoBuilder()
    {
        _addFinancialInformationDto = new AddFinancialInformationDto()
        {
            JobType = JobType.Unemployed,
            MonthlyIncome = 100,
            FinancialAssets = 200
        };
    }


    public AddFinancialInformationDtoBuilder WithJobType(JobType jobType)
    {
        _addFinancialInformationDto.JobType = jobType;
        return this;
    }

    public AddFinancialInformationDtoBuilder WithMonthlyIncome(
        decimal monthlyIncome)
    {
        _addFinancialInformationDto.MonthlyIncome = monthlyIncome;
        return this;
    }

    public AddFinancialInformationDtoBuilder WithFinancialAssets(
        decimal financialAssets)
    {
        _addFinancialInformationDto.FinancialAssets = financialAssets;
        return this;
    }

    public AddFinancialInformationDto Build()
    {
        return _addFinancialInformationDto;
    }
}