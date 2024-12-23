namespace LoanManagement.Services.RequestedLoans.CalculateTools;

public class RequestedLoanCalculatorTool : RequestedLoanCalculator
{
    public RequestedLoanStatus DeterminateStatusOnAdd(int creditRate)
    {
        var result = RequestedLoanStatus.Pending;
        if (creditRate < 60)
        {
            result = RequestedLoanStatus.Rejected;
        }

        return result;
    }

    public int DeterminateCreditRate(GetCustomerInformationDto customerInfo,
        decimal loanAmount, GetAllRequstedLoansSummaryDto requestedLoansSummary)
    {
        var creditRate = MonthlyIncomeRate((decimal)customerInfo.MonthlyIncome);
        creditRate += JobTypeRate((JobType)customerInfo.JobType);
        creditRate +=
            FinancialAssetsRate((decimal)customerInfo.FinancialAssets!,
                loanAmount);
        creditRate +=
            OnTimeClosedLoanRate(requestedLoansSummary
                .TotalAllOnTimeClosedLoansCount);
        creditRate +=
            DelayedInstallmentsRate(requestedLoansSummary
                .TotalDelayedInstallmentsCount);

        if (creditRate > 100)
        {
            creditRate = 100;
        }

        if (creditRate < 1)
        {
            creditRate = 1;
        }

        return creditRate;
    }

    public int MonthlyIncomeRate(decimal monthlyIncome)
    {
        var rate = 0;
        var max = 10000000;
        var min = 5000000;
        if (max > monthlyIncome && monthlyIncome >= min)
        {
            rate = 10;
        }

        if (monthlyIncome >= max)
        {
            rate = 20;
        }

        return rate;
    }

    public int JobTypeRate(JobType jobType)
    {
        int rate;
        switch (jobType)
        {
            case JobType.GovernmentEmployee:
                rate = 20;
                break;
            case JobType.SelfEmployee:
                rate = 10;
                break;
            default:
                rate = 0;
                break;
        }

        return rate;
    }

    public int FinancialAssetsRate(decimal financialAssets, decimal loanAmount)
    {
        var rate = 0;
        var max = financialAssets / 2;
        var min = (financialAssets / 10) * 7;
        if (max < loanAmount && min >= loanAmount)
        {
            rate = 10;
        }

        if (max >= loanAmount)
        {
            rate = 20;
        }

        return rate;
    }

    public int OnTimeClosedLoanRate(int onTimeClosedLoans)
    {
        return onTimeClosedLoans * 30;
    }

    public int DelayedInstallmentsRate(int delayedInstallments)
    {
        int penaltyRate = -5;
        return delayedInstallments * penaltyRate;
    }
}