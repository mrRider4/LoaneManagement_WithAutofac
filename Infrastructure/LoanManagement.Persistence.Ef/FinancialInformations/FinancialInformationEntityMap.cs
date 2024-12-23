namespace LoanManagement.Persistence.Ef.FinancialInformations;

public class
    FinancialInformationEntityMap : IEntityTypeConfiguration<
    FinancialInformation>
{
    public void Configure(EntityTypeBuilder<FinancialInformation> builder)
    {
        builder.ToTable("FinancialInformations");
        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).UseIdentityColumn();
        builder.Property(f => f.CustomerId).IsRequired();
        builder.Property(f => f.JobType).IsRequired();
        builder.Property(f => f.MonthlyIncome).IsRequired();
        builder.Property(f => f.FinancialAssets).IsRequired();
        builder.Property(f => f.IsDeleted).IsRequired();
    }
}