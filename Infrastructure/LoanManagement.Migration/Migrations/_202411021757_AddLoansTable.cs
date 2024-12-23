namespace LoanManagement.Migration.Migrations;

[Migration(202411021757)]
public class _202411021757_AddLoansTable:FluentMigrator.Migration
{
    public override void Up()
    {
        Create.Table("Loans")
            .WithColumn("Id").AsInt32().Identity().PrimaryKey()
            .WithColumn("Amount").AsDecimal().NotNullable()
            .WithColumn("AnnualInterestPercentage").AsDecimal().NotNullable()
            .WithColumn("InstallmentCount").AsInt32().NotNullable()
            .WithColumn("InstallmentAmount").AsDecimal().NotNullable()
            .WithColumn("LatePaymentFee").AsDecimal().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Loans");
    }
}