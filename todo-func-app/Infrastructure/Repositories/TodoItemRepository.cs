using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Repositories;

public class TodoItemRepository : CosmosGenericRepository<TodoItem>
{
    public TodoItemRepository(Container container, ICurrentTime timeService, IClaimsService claimsService, ILogger<CosmosGenericRepository<TodoItem>> logger)
        : base(container, timeService, claimsService, logger)
    {
    }

    protected override string GetPartitionKey(TodoItem entity)
    {
        return entity.userId ?? "system";
    }
}
