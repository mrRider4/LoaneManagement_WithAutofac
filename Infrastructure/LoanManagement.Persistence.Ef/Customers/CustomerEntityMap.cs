namespace LoanManagement.Persistence.Ef.Customers;

public class CustomerEntityMap : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).UseIdentityColumn();
        builder.Property(c => c.Name).IsRequired();
        builder.Property(c => c.LastName).IsRequired();
        builder.Property(c => c.NationalCode).IsRequired().HasMaxLength(10);
        builder.Property(c => c.PhoneNumber).IsRequired().HasMaxLength(10);
        builder.Property(c => c.Email).IsRequired();
        builder.Property(c => c.IsVerified).IsRequired().HasDefaultValue(false);
    }
}