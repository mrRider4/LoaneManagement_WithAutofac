namespace LoanManagement.TestTools.FinancialInformations;

public class UpdateFinancialInformationDtoBuilder
{
    private readonly UpdateFinancialInformationDto
        _updateFinancialInformationDto;

    public UpdateFinancialInformationDtoBuilder()
    {
        _updateFinancialInformationDto = new UpdateFinancialInformationDto()
        {
            JobType = JobType.Unemployed,
            MonthlyIncome = 200,
            FinancialAssets = 300
        };
    }

    public UpdateFinancialInformationDtoBuilder WithJobType(JobType? jobType)
    {
        _updateFinancialInformationDto.JobType = jobType;
        return this;
    }

    public UpdateFinancialInformationDtoBuilder WithMonthlyIncome(
        decimal? monthlyIncome)
    {
        _updateFinancialInformationDto.MonthlyIncome = monthlyIncome;
        return this;
    }

    public UpdateFinancialInformationDtoBuilder WithFinancialAsserts(
        decimal? financialAsserts)
    {
        _updateFinancialInformationDto.FinancialAssets = financialAsserts;
        return this;
    }

    public UpdateFinancialInformationDto Build()
    {
        return _updateFinancialInformationDto;
    }
}