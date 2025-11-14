using Domain.Entities;

namespace Infrastructure.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<TodoItem> TodoItems { get; }
    IGenericRepository<User> Users { get; }
    Task<int> SaveChangesAsync();
}