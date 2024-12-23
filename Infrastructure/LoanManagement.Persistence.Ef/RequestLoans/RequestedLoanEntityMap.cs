namespace LoanManagement.Persistence.Ef.RequestLoans;

public class RequestedLoanEntityMap : IEntityTypeConfiguration<RequestedLoan>
{
    public void Configure(EntityTypeBuilder<RequestedLoan> builder)
    {
        builder.ToTable("RequestedLoans");
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).UseIdentityColumn();
        builder.Property(r => r.LoanId).IsRequired();
        builder.Property(r => r.FinancialInformationId);
        builder.Property(r => r.RequestedDate).IsRequired();
        builder.Property(r => r.CreditRate).IsRequired().HasDefaultValue(0);
        builder.Property(r => r.RequestedLoanStatus).IsRequired();
    }
}