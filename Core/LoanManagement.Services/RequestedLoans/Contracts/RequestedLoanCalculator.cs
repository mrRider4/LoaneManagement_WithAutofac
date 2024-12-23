namespace LoanManagement.Services.RequestedLoans.Contracts;

public interface RequestedLoanCalculator
{
    RequestedLoanStatus DeterminateStatusOnAdd(
        int creditRate);

    int DeterminateCreditRate(GetCustomerInformationDto customerInfo,
        decimal loanAmount,
        GetAllRequstedLoansSummaryDto requestedLoansSummary);

    int MonthlyIncomeRate(decimal monthlyIncome);
    int JobTypeRate(JobType jobType);
    int FinancialAssetsRate(decimal financialAssets, decimal loanAmount);
    int OnTimeClosedLoanRate(int onTimeClosedLoans);
    int DelayedInstallmentsRate(int delayedInstallments);
}