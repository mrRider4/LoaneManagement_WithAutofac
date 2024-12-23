namespace LoanManagement.Migration.Migrations;

[Migration(202411021743)]
public class _202411021743_AddInstallmentsTable:FluentMigrator.Migration
{
    public override void Up()
    {
        Create.Table("Installments")
            .WithColumn("Id").AsInt32().Identity().PrimaryKey()
            .WithColumn("RequestedLoanId").AsInt32().NotNullable()
            .WithColumn("DueDate").AsDate().NotNullable()
            .WithColumn("PaymentDate").AsDate().Nullable()
            .WithColumn("PayableAmount").AsDecimal().NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Installments");
    }
}