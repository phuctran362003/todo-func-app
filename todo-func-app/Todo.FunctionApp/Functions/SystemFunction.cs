using Application.Utils;
using Domain.Entities;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace Todo.FunctionApp.Functions;

public class SystemFunction
{
    private readonly ILogger<SystemFunction> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CosmosDbContext _cosmosDbContext;

    public SystemFunction(ILogger<SystemFunction> logger, IUnitOfWork unitOfWork, CosmosDbContext cosmosDbContext)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _cosmosDbContext = cosmosDbContext;
    }

    [Function("SeedTodoItems")]
    public async Task<IActionResult> SeedTodoItemsHttpTrigger(
                    [HttpTrigger(AuthorizationLevel.Function, "post", Route = "seed-todoitems")] HttpRequest req)
    {
        try
        {
            await SeedTodoItemsAsync();
            return new OkObjectResult(ApiResult.Success("201", "TodoItems seeded successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error seeding TodoItems: {ex.Message}");
            return new BadRequestObjectResult(ApiResult.Failure("500", "Error seeding TodoItems."));
        }
    }

    [Function("ClearData")]
    public async Task<IActionResult> ClearDataHttpTrigger(
                    [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "clear-data")] HttpRequest req)
    {
        try
        {
            await ClearDataAsync();
            return new OkObjectResult(ApiResult.Success("200", "Data cleared successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error clearing data: {ex.Message}");
            return new BadRequestObjectResult(ApiResult.Failure("500", "Error clearing data."));
        }
    }

    private async Task ClearDataAsync()
    {
        var database = _cosmosDbContext.GetContainer(CosmosDbContext.TodoItemsContainer).Database;
        
        // Delete and recreate TodoItems container
        try
        {
            await database.GetContainer(CosmosDbContext.TodoItemsContainer).DeleteContainerAsync();
        }
        catch { }
        
        // Delete and recreate Users container
        try
        {
            await database.GetContainer(CosmosDbContext.UsersContainer).DeleteContainerAsync();
        }
        catch { }
        
        // Reinitialize containers
        await _cosmosDbContext.InitializeAsync();
    }

    private async Task SeedTodoItemsAsync()
    {
        var items = new List<TodoItem>
            {
                new TodoItem { id = Guid.NewGuid().ToString(), Title = "Buy groceries", IsCompleted = false, userId = "system" },
                new TodoItem { id = Guid.NewGuid().ToString(), Title = "Finish project report", IsCompleted = true, userId = "system" },
                new TodoItem { id = Guid.NewGuid().ToString(), Title = "Call mom", IsCompleted = false, userId = "system" },
                new TodoItem { id = Guid.NewGuid().ToString(), Title = "Book flight tickets", IsCompleted = true, userId = "system" },
                new TodoItem { id = Guid.NewGuid().ToString(), Title = "Read a book", IsCompleted = false, userId = "system" },
                new TodoItem { id = Guid.NewGuid().ToString(), Title = "Clean the house", IsCompleted = true, userId = "system" },
                new TodoItem { id = Guid.NewGuid().ToString(), Title = "Pay electricity bill", IsCompleted = false, userId = "system" },
                new TodoItem { id = Guid.NewGuid().ToString(), Title = "Exercise for 30 minutes", IsCompleted = true, userId = "system" },
                new TodoItem { id = Guid.NewGuid().ToString(), Title = "Prepare dinner", IsCompleted = false, userId = "system" },
                new TodoItem { id = Guid.NewGuid().ToString(), Title = "Review meeting notes", IsCompleted = true, userId = "system" }
            };
        foreach (var item in items)
        {
            await _unitOfWork.TodoItems.AddAsync(item);
        }
        await _unitOfWork.SaveChangesAsync();
    }
}