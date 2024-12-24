namespace LoanManagement.Services.UnitOfWorks.Contracts;

public interface UnitOfWork : IScope
{
    public Task Save();
    public Task Begin();
    public Task Commit();
    public Task Rollback();
}