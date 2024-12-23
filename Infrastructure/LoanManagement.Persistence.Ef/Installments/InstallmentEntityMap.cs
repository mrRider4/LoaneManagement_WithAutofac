namespace LoanManagement.Persistence.Ef.Installments;

public class InstallmentEntityMap:IEntityTypeConfiguration<Installment>
{
    public void Configure(EntityTypeBuilder<Installment> builder)
    {
        builder.ToTable("Installments");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.Id).UseIdentityColumn();
        builder.Property(i => i.RequestedLoanId).IsRequired();
        builder.Property(i => i.DueDate).IsRequired();
        builder.Property(i => i.PaymentDate).IsRequired(false);
        builder.Property(i => i.PayableAmount).IsRequired();
    }
}