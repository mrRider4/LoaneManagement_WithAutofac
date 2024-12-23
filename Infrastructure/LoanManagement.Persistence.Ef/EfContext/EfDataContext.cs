namespace LoanManagement.Persistence.Ef.EfContext;

public class EfDataContext : DbContext
{
    public EfDataContext(string connectionString) : this(
        new DbContextOptionsBuilder<EfDataContext>()
            .UseSqlServer(connectionString).Options)
    {
    }

    public EfDataContext(DbContextOptions<EfDataContext> options) :
        base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(CustomerEntityMap).Assembly);
    }
}