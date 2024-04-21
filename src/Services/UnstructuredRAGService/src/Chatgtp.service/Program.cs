using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using UnstructuredRAG.Service.Contracts;
using UnstructuredRAG.Service.Options;
using UnstructuredRAG.Service.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        // Versions must match: SwaggerDoc("v1" --and-- Version = "v1",
        Version = "v1",
        Title = "ChatFusion OpenAi Microservice",
        Description = "Use this REST-based service to enable chat capabilities into any modern app. It exposes endpoints to leverage Azure Open AI LLM features and store state to a pluggable state store.",
        Contact = new OpenApiContact
        {
            Name = "Rob Vettor",
            Email = "robvet@microsoft.com",
            Url = new Uri("https://microsoft.sharepoint.com/teams/CEContent")
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

// Add AppInsights instrumentation
builder.Services.AddApplicationInsightsTelemetry();

// adds reference to blazor bootstrap
//builder.Services.AddBlazorBootstrap();

builder.RegisterConfiguration();
builder.Services.RegisterServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

static class ProgramExtensions
{
    public static void RegisterConfiguration(this WebApplicationBuilder builder)
    {
        //builder.Services.AddOptions<CosmosDb>()
        //    .Bind(builder.Configuration.GetSection("CosmosDb"));

        //builder.Services.AddOptions<CosmosDb>()
        //    .Bind(builder.Configuration.GetSection("CosmosKey") ?? throw new ArgumentNullException("CosmosKey"));

        //builder.Services.AddOptions<OpenAi>()
        //    .Bind(builder.Configuration.GetSection("OpenAi"));

        // Read App Insights Instrumentation Key from environment variable or secrets
        builder.Configuration.GetSection("InstrumentationKey");

        // Read key from environment variable or secrets
        builder.Configuration.GetSection("CosmosKey");
        builder.Configuration.GetSection("CosmosEndpoint");
        builder.Configuration.GetSection("CosmosDatabase");
        builder.Configuration.GetSection("CosmosContainer");

        builder.Configuration.GetSection("OpenAiKey");
        builder.Configuration.GetSection("OpenAiEndpoint");
        builder.Configuration.GetSection("OpenAiModel");

        builder.Configuration.GetSection("SearchEndpoint");
        builder.Configuration.GetSection("SearchKey");
        builder.Configuration.GetSection("OpenAiModel");

        //builder.Services.AddOptions<OpenAi>()
        //    .Bind(builder.Configuration.GetSection("CosmosKey"));

        //builder.Configuration.GetSection("OpenAiKey");

        //var configuration = new ConfigurationBuilder()
        //    .AddJsonFile("appsettings.json")
        //    .Build();
        //builder.Services.Configure<CosmosOptions>(configuration.GetSection("Cosmos"));
    }

    public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register ChatService, OpenAiService, and CosmosDbService
        //services.AddTransient<IChatService, ChatService>();
        //services.AddTransient<IOpenAiService, OpenAiService>();
        //services.AddTransient<ICosmosDbService, CosmosDbService >();

        services.AddSingleton<IDataRepository, CosmosDbService>((provider) =>
        //services.AddSingleton<ICosmosDbService, CosmosDbService>((provider) =>
        {
            return new CosmosDbService(
                    endpoint: configuration.GetSection("CosmosEndpoint").Value ?? throw new ArgumentNullException("Endpoint", "CosmosDb Endpoint cannot be null"),
                    key: configuration.GetSection("CosmosKey").Value ?? throw new ArgumentNullException("Key", "CosmosDb Key cannot be null"),
                    databaseName: configuration.GetSection("CosmosDatabase").Value ?? throw new ArgumentNullException("Database", "CosmosDb Database Name cannot be null"),
                    containerName: configuration.GetSection("CosmosContainer").Value ?? throw new ArgumentNullException("Container", "CosmosDb Container Name cannot be null"),
                    logger: provider.GetRequiredService<ILogger<CosmosDbService>>()
               );
        });


        //services.AddSingleton<CosmosDbService, CosmosDbService>((provider) =>
        ////services.AddSingleton<ICosmosDbService, CosmosDbService>((provider) =>
        //{
        //    var cosmosDbOptions = provider.GetRequiredService<IOptions<CosmosDb>>();
        //    if (cosmosDbOptions is null)
        //    {
        //        throw new ArgumentException($"{nameof(IOptions<CosmosDb>)} was not resolved through dependency injection.");
        //    }
        //    else
        //    {
        //        return new CosmosDbService(
        //             endpoint: cosmosDbOptions.Value.Endpoint ?? throw new ArgumentNullException("Endpoint", "CosmosDb Endpoint cannot be null"),
        //             //endpoint: configuration.GetSection("Endpoint").Value ?? throw new ArgumentNullException("Endpoint", "CosmosDb Endpoint cannot be null"),
        //             key: cosmosDbOptions.Value.Key ?? throw new ArgumentNullException("Key", "CosmosDb Key cannot be null"),
        //             databaseName: cosmosDbOptions.Value.Database ?? throw new ArgumentNullException("Database", "CosmosDb Database Name cannot be null"),
        //             containerName: cosmosDbOptions.Value.Container ?? throw new ArgumentNullException("Container", "CosmosDb Container Name cannot be null"),
        //             logger: provider.GetRequiredService<ILogger<CosmosDbService>>()
        //        );
        //    }
        //});

        services.AddSingleton<IOpenAiService, OpenAiService>((provider) =>
        //services.AddSingleton<IOpenAiService, OpenAiService>((provider) =>
        {
            return new OpenAiService(
                    endpoint: configuration.GetSection("OpenAiEndpoint").Value ?? throw new ArgumentNullException("Endpoint", "OpenAI Endpoint cannot be null"),
                    //endpoint: openAiOptions.Value?.Endpoint ?? String.Empty,
                    key: configuration.GetSection("OpenAiKey").Value ?? throw new ArgumentNullException("Key", "OpenAI Key cannot be null"),
                    //key: openAiOptions.Value?.Key ?? String.Empty,
                    modelName: configuration.GetSection("OpenAiModel").Value ?? throw new ArgumentNullException("ModelName", "OpenAI ModelName cannot be null"),
                    //modelName: openAiOptions.Value?.ModelName ?? String.Empty
                    searchEndpoint: configuration.GetSection("SearchEndpoint").Value ?? throw new ArgumentNullException("SearchEndpoint", "Search Endpoint cannot be null"),
                    //endpoint: openAiOptions.Value?.Endpoint ?? String.Empty,
                    searchKey: configuration.GetSection("SearchKey").Value ?? throw new ArgumentNullException("SearchKey", "Search Key cannot be null"),
                    //key: openAiOptions.Value?.Key ?? String.Empty,
                    searchIndex: configuration.GetSection("SearchIndex").Value ?? throw new ArgumentNullException("SearchIndex", "Search Index cannot be null")
                );
        });


        //services.AddSingleton<OpenAiService, OpenAiService>((provider) =>
        ////services.AddSingleton<IOpenAiService, OpenAiService>((provider) =>
        //{
        //    var openAiOptions = provider.GetRequiredService<IOptions<OpenAi>>();
        //    if (openAiOptions is null)
        //    {
        //        throw new ArgumentException($"{nameof(IOptions<OpenAi>)} was not resolved through dependency injection.");
        //    }
        //    else
        //    {
        //        return new OpenAiService(
        //            endpoint: configuration.GetSection("OpenAiEndpoint").Value ?? throw new ArgumentNullException("Endpoint", "OpenAI Endpoint cannot be null"),
        //            //endpoint: openAiOptions.Value?.Endpoint ?? String.Empty,
        //            key: configuration.GetSection("OpenAiKey").Value ?? throw new ArgumentNullException("Key", "OpenAI Key cannot be null"),
        //            //key: openAiOptions.Value?.Key ?? String.Empty,
        //            modelName: openAiOptions.Value?.ModelName ?? throw new ArgumentNullException("ModelName", "OpenAI ModelName cannot be null")
        //        //modelName: openAiOptions.Value?.ModelName ?? String.Empty
        //        );
        //    }
        //});

        //services.AddSingleton<ChatService>((provider) =>
        services.AddSingleton<IChatService>((provider) =>
        {
            var openAiOptions = provider.GetRequiredService<IOptions<OpenAi>>();

            if (openAiOptions is null)
            {
                throw new ArgumentException($"{nameof(IOptions<OpenAi>)} was not resolved through dependency injection.");
            }
            else
            {
                var cosmosDbService = provider.GetRequiredService<IDataRepository>();
                var openAiService = provider.GetRequiredService<IOpenAiService>();

                return new ChatService(openAiService: openAiService,
                                       cosmosDbService: cosmosDbService,
                                       maxConversationTokens: openAiOptions.Value?.MaxConversationTokens ?? String.Empty,
                                       logger: provider.GetRequiredService<ILogger<ChatService>>()
                );
            }
        });
    }



    //public static void RegisterCosmos(this WebApplicationBuilder builder)
    //{
    //    builder.Services.AddSingleton<CosmosClient>(s =>
    //    {
    //        var options = s.GetService<IOptions<CosmosDb>>();
    //        var cosmosDb = options.Value;
    //        return new CosmosClient(cosmosDb.Endpoint, cosmosDb.Key);
    //    });
    //}



}