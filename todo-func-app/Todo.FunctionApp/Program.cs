using Csv.FuncApp1.IOContainer;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

// REQUIRED for Azure Functions Isolated (.NET 8)
builder.Configuration
       .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);

builder.Services.SetupIocContainer();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.ConfigureFunctionsWebApplication();

var app = builder.Build();
app.Run();
