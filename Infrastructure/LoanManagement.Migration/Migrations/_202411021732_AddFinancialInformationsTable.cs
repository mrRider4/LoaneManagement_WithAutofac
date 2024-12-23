namespace LoanManagement.Migration.Migrations;

[Migration(202411021732)]
public class _202411021732_AddFinancialInformationsTable:FluentMigrator.Migration
{
    public override void Up()
    {
        Create.Table("FinancialInformations")
            .WithColumn("Id").AsInt32().Identity().PrimaryKey()
            .WithColumn("UserId").AsInt32().NotNullable()
            .WithColumn("JobType").AsInt32().NotNullable()
            .WithColumn("MonthlyIncome").AsDecimal().NotNullable()
            .WithColumn("FinancialAssets").AsDecimal().NotNullable()
            .WithColumn("IsDeleted").AsBoolean().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("FinancialInformations");
    }
}