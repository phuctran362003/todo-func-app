using Application.Interfaces;
using Application.Utils;
using Domain.DTOs.TodoDTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Todo.FunctionApp.Functions;

public class TodoFunctions
{
    private readonly ILogger<TodoFunctions> _logger;
    private readonly ITodoService _todoService;

    public TodoFunctions(ILogger<TodoFunctions> logger, ITodoService todoService)
    {
        _logger = logger;
        _todoService = todoService;
    }

    [Function("CreateTodo")]
    public async Task<IActionResult> CreateTodo(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "todos")] HttpRequest req)
    {
        try
        {

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var createDto = JsonSerializer.Deserialize<CreateTodoItemDto>(requestBody);

            if (createDto == null || string.IsNullOrWhiteSpace(createDto.Title))
            {
                return new BadRequestObjectResult(
                    ApiResult.Failure("400", "Title is required."));
            }

            var result = await _todoService.CreateAsync(createDto);
            return new CreatedResult($"/api/todos/{result.Id}",
                ApiResult<Domain.DTOs.TodoDTOs.TodoItemDto>.Success(result, "201", "Todo created successfully."));
        }
        catch (Exception ex)
        {
            return new ObjectResult(
                ApiResult.Failure("500", "Internal server error."))
            { StatusCode = 500 };
        }
    }

    [Function("GetTodoById")]
    public async Task<IActionResult> GetTodoById(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "todos/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation($"Get Todo by Id function triggered for id: {id}");

            if (string.IsNullOrWhiteSpace(id))
            {
                return new BadRequestObjectResult(
                    ApiResult.Failure("400", "Id is required."));
            }

            var result = await _todoService.GetByIdAsync(id);
            return new OkObjectResult(
                ApiResult<Domain.DTOs.TodoDTOs.TodoItemDto>.Success(result, "200", "Todo retrieved successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"Todo not found: {ex.Message}");
            return new NotFoundObjectResult(
                ApiResult.Failure("404", ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetTodoById: {ex.Message}");
            return new ObjectResult(
                ApiResult.Failure("500", "Internal server error."))
            { StatusCode = 500 };
        }
    }

    [Function("GetAllTodos")]
    public async Task<IActionResult> GetAllTodos(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "todos")] HttpRequest req)
    {
        try
        {
            _logger.LogInformation("Get All Todos function triggered.");

            var result = await _todoService.GetAllAsync();
            return new OkObjectResult(
                ApiResult<IEnumerable<Domain.DTOs.TodoDTOs.TodoItemDto>>.Success(
                    result, "200", "Todos retrieved successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in GetAllTodos: {ex.Message}");
            return new ObjectResult(
                ApiResult.Failure("500", "Internal server error."))
            { StatusCode = 500 };
        }
    }

    [Function("UpdateTodo")]
    public async Task<IActionResult> UpdateTodo(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "todos/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation($"Update Todo function triggered for id: {id}");

            if (string.IsNullOrWhiteSpace(id))
            {
                return new BadRequestObjectResult(
                    ApiResult.Failure("400", "Id is required."));
            }

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updateDto = JsonSerializer.Deserialize<UpdateTodoItemDto>(requestBody);

            if (updateDto == null || string.IsNullOrWhiteSpace(updateDto.Title))
            {
                return new BadRequestObjectResult(
                    ApiResult.Failure("400", "Title is required."));
            }

            var result = await _todoService.UpdateAsync(id, updateDto);
            return new OkObjectResult(
                ApiResult<Domain.DTOs.TodoDTOs.TodoItemDto>.Success(result, "200", "Todo updated successfully."));
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning($"Todo not found: {ex.Message}");
            return new NotFoundObjectResult(
                ApiResult.Failure("404", ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in UpdateTodo: {ex.Message}");
            return new ObjectResult(
                ApiResult.Failure("500", "Internal server error."))
            { StatusCode = 500 };
        }
    }

    [Function("DeleteTodo")]
    public async Task<IActionResult> DeleteTodo(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "todos/{id}")] HttpRequest req,
        string id)
    {
        try
        {
            _logger.LogInformation($"Delete Todo function triggered for id: {id}");

            if (string.IsNullOrWhiteSpace(id))
            {
                return new BadRequestObjectResult(
                    ApiResult.Failure("400", "Id is required."));
            }

            var isDeleted = await _todoService.DeleteAsync(id);

            if (!isDeleted)
            {
                return new NotFoundObjectResult(
                    ApiResult.Failure("404", $"Todo with id {id} not found."));
            }

            return new OkObjectResult(
                ApiResult.Success("200", "Todo deleted successfully."));
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error in DeleteTodo: {ex.Message}");
            return new ObjectResult(
                ApiResult.Failure("500", "Internal server error."))
            { StatusCode = 500 };
        }
    }
}