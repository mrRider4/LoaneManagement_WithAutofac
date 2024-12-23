namespace LoanManagement.TestTools.FinancialInformations;

public class FinancialInformationBuilder
{
    private readonly FinancialInformation _financialInfo;

    public FinancialInformationBuilder()
    {
        _financialInfo = new FinancialInformation()
        {
            CustomerId = -1,
            JobType = JobType.Unemployed,
            MonthlyIncome = 100,
            FinancialAssets = 0,
            IsDeleted = false
        };
    }

    public FinancialInformationBuilder WithJobType(JobType jobType)
    {
        _financialInfo.JobType = jobType;
        return this;
    }

    public FinancialInformationBuilder WithFinancialAssets(decimal financialAssets)
    {
        _financialInfo.FinancialAssets = financialAssets;
        return this;
    }

    public FinancialInformationBuilder WithMonthlyIncome(decimal monthlyIncome)
    {
        _financialInfo.MonthlyIncome = monthlyIncome;
        return this;
    }

    public FinancialInformationBuilder WithCustomerId(int customerId)
    {
        _financialInfo.CustomerId = customerId;
        return this;
    }

    public FinancialInformationBuilder WithIsDeleted(bool isDeleted)
    {
        _financialInfo.IsDeleted = isDeleted;
        return this;
    }

    public FinancialInformation Build()
    {
        return _financialInfo;
    }
}