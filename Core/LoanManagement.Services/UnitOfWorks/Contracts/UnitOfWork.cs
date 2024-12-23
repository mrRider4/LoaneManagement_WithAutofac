namespace LoanManagement.Services.UnitOfWorks.Contracts;

public interface UnitOfWork
{
    public Task Save();
    public Task Begin();
    public Task Commit();
    public Task Rollback();
}