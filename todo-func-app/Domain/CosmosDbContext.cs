using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

public class CosmosDbContext
{
    private readonly CosmosClient _client;
    private readonly Database _database;
    private readonly CosmosDbSettings _settings;

    // Container names - dbset
    public const string UsersContainer = "Users";
    public const string TodoItemsContainer = "TodoItems";

    public CosmosDbContext(IOptions<CosmosDbSettings> settings)
    {
        _settings = settings.Value;

        _client = new CosmosClient(
            _settings.AccountEndpoint,
            _settings.AccountKey,
            new CosmosClientOptions
            {
                SerializerOptions = new CosmosSerializationOptions
                {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                },
                AllowBulkExecution = true
            });

        // Create database if not exists (sync on startup)
        _database = _client
            .CreateDatabaseIfNotExistsAsync(_settings.DatabaseName, _settings.Throughput)
            .GetAwaiter()
            .GetResult();
    }

    // Generic container getter
    public Container GetContainer(string containerName) => _database.GetContainer(containerName);

    // Reusable method
    public async Task<Container> EnsureContainerAsync(string containerName, string partitionKeyPath)
    {
        var response = await _database.CreateContainerIfNotExistsAsync(
            containerName,
            partitionKeyPath,
            _settings.Throughput ?? 400
        );

        return response.Container;
    }

    // The required Initialize()
    public async Task InitializeAsync()
    {
        // USERS CONTAINER
        await _database.CreateContainerIfNotExistsAsync(
            new ContainerProperties
            {
                Id = UsersContainer,
                PartitionKeyPath = "/pk",
                UniqueKeyPolicy = new UniqueKeyPolicy
                {
                    UniqueKeys =
                    {
                        new UniqueKey { Paths = { "/email" } }
                    }
                }
            },
            _settings.Throughput ?? 400
        );

        // TODO ITEMS CONTAINER
        await _database.CreateContainerIfNotExistsAsync(
            new ContainerProperties
            {
                Id = TodoItemsContainer,
                PartitionKeyPath = "/userId"
            },
            _settings.Throughput ?? 400
        );
    }

    // For DI config
    public class CosmosDbSettings
    {
        public string AccountEndpoint { get; set; } = string.Empty;
        public string AccountKey { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
        public int? Throughput { get; set; } = 400; // default RU/s
    }
}
