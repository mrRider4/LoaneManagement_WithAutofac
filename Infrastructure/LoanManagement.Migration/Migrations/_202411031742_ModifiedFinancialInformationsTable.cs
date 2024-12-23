namespace LoanManagement.Migration.Migrations;

[Migration(202411031742)]
public class _202411031742_ModifiedFinancialInformationsTable:FluentMigrator.Migration
{
    public override void Up()
    {
        Rename.Column("UserId").OnTable("FinancialInformations")
            .To("CustomerId");
    }

    public override void Down()
    {
        Rename.Column("CustomerId").OnTable("FinancialInformations")
            .To("UserId");
    }
}