using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TodoItem> TodoItems { get; }
    Task<int> SaveChangesAsync();
}