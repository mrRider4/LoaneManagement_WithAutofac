namespace LoanManagement.Migration.Migrations;

[Migration(202411021802)]
public class _202411021802_AddRequestedLoansTable:FluentMigrator.Migration
{
    public override void Up()
    {
        Create.Table("RequestedLoans")
            .WithColumn("Id").AsInt32().Identity().PrimaryKey()
            .WithColumn("LoanId").AsInt32().NotNullable()
            .WithColumn("FinancialInformationId").AsInt32().NotNullable()
            .WithColumn("RequestedDate").AsDate().NotNullable()
            .WithColumn("CreditRate").AsInt32().NotNullable().WithDefaultValue(0)
            .WithColumn("RequestedLoanStatus").AsInt32().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("RequestedLoans");
    }
}