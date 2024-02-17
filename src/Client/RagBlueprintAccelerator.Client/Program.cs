using Shared.Contracts;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RagBlueprintAccelerator.Client;
using RagBlueprintAccelerator.Client.Contracts;
using RagBlueprintAccelerator.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//builder.Services.AddSingleton(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//builder.Services.AddSingleton( x => new HttpClient {
//    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
//});

// Enagage plumbing for IConfiguration and user secerts
IConfiguration configuration = builder.Configuration;

builder.Services.AddSingleton<IChatService, ChatService>();
builder.Services.AddSingleton<ISimpleChatService, SimpleChatService>();
//builder.Services.AddSingleton<IDataExtractionCompletion, DataExtractionCompletion>();
builder.Services.AddScoped<IPOCService, POCService>();

// .NET 8 feature to register multiple services implementations of the same interface
//builder.Services.AddScoped<Func<string, IChatService>>(serviceProvider => key =>
//{
//    switch (key)
//    {
//        case "ChatService":
//            return serviceProvider.GetService<ChatService>();
//        case "SimpleChatService":
//            return serviceProvider.GetService<SimpleChatService>();
//        default:
//            throw new KeyNotFoundException(); // or maybe return null, up to you
//    }
//});

builder.Services.AddSingleton(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
    Timeout = TimeSpan.FromMinutes(5) // Increase the timeout to 5 minutes
});


//builder.Services.AddHttpClient<POCService>(client =>
//{
//    client.BaseAddress = new Uri(builder.Configuration.GetSection("BaseUri").Value!);
//});



await builder.Build().RunAsync();
