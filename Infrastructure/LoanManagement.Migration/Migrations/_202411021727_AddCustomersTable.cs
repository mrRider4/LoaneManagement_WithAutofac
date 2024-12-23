

namespace LoanManagement.Migration.Migrations;

[Migration(202411021727)]
public class _202411021727_AddCustomersTable:FluentMigrator.Migration
{
    public override void Up()
    {
        Create.Table("Customers")
            .WithColumn("Id").AsInt32().Identity().PrimaryKey()
            .WithColumn("Name").AsString().NotNullable()
            .WithColumn("LastName").AsString().NotNullable()
            .WithColumn("NationalCode").AsString(10).NotNullable()
            .WithColumn("PhoneNumber").AsString(10).NotNullable()
            .WithColumn("Email").AsString().NotNullable()
            .WithColumn("IsVerified").AsBoolean().WithDefaultValue(false)
            .NotNullable();
    }

    public override void Down()
    {
        Delete.Table("Customers");
    }
}