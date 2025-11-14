using Domain.Entities;
using Infrastructure.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly CosmosDbContext _cosmosDbContext;
    private readonly ICurrentTime _currentTime;
    private readonly IClaimsService _claimsService;
    private readonly ILoggerFactory _loggerFactory;

    private IGenericRepository<TodoItem>? _todoItems;

    public UnitOfWork(CosmosDbContext cosmosDbContext, ICurrentTime currentTime, IClaimsService claimsService, ILoggerFactory loggerFactory)
    {
        _cosmosDbContext = cosmosDbContext;
        _currentTime = currentTime;
        _claimsService = claimsService;
        _loggerFactory = loggerFactory;
    }

    public IGenericRepository<TodoItem> TodoItems =>
       _todoItems ??= new TodoItemRepository(
           _cosmosDbContext.GetContainer("TodoItems"),
           _currentTime,
           _claimsService,
           _loggerFactory.CreateLogger<CosmosGenericRepository<TodoItem>>()
       );

    public void Dispose()
    {
        // DO NOT dispose CosmosDbContext - it's a singleton managed by DI container
    }

    public async Task<int> SaveChangesAsync()
    {
        // Cosmos DB commits per operation; keep SaveChangesAsync for compatibility only.
        return await Task.FromResult(1);
    }
}