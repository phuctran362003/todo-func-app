using Application.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Todo.FunctionApp.Functions;

public class TodoFunctions
{
    private readonly ILogger<TodoFunctions> _logger;

    public TodoFunctions(ILogger<TodoFunctions> logger)
    {
        _logger = logger;
    }

    [Function("TodoFunctions")]
    public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");
        return new OkObjectResult(ApiResult.Success("200", "Welcome to Azure Functions!"));
    }
}