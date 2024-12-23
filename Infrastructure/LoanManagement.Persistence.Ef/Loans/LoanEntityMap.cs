namespace LoanManagement.Persistence.Ef.Loans;

public class LoanEntityMap : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).UseIdentityColumn();
        builder.Property(l => l.Amount).IsRequired();
        builder.Property(l => l.AnnualInterestPercentage).IsRequired();
        builder.Property(l => l.InstallmentCount).IsRequired();
        builder.Property(l => l.InstallmentAmount).IsRequired();
        builder.Property(l => l.LatePaymentFee).IsRequired();
    }
}