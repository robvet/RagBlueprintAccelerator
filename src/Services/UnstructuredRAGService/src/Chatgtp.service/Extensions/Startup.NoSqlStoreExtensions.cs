﻿using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
//using order.domain.Contracts;
//using order.infrastructure.nosql.Configurations;
//using order.infrastructure.nosql.Persistence;
//using order.infrastructure.nosql.Persistence.Contracts;
//using order.infrastructure.nosql.Persistence.Repositories;
using System;

namespace UnstructuredRAG.Service.Extensions
{
    //public static class NoSqlStoreExtensions
    //{
    //    public static IServiceCollection RegisterNoSqlStore(this IServiceCollection services,
    //        IConfiguration configuration)
    //    {
    //        // new Cosmos DB Repository Code configuration
    //        //var connectionString = configuration["cosmosconnection"] ??
    //        //                       throw new ArgumentNullException("cosmosconnection", "Cosmos Connection missing");

    //        //CosmosClient client = new CosmosClient(connectionString);

    //        //CosmosDbConfiguration cosmosDbConfig = configuration.GetSection(CosmosDbConfiguration.SectionKey).Get<CosmosDbConfiguration>() ??
    //        //                       throw new ArgumentNullException("cosmosDBConfig", "Missing from AppSettings.Json"); ;

    //        //services.AddSingleton<ICosmosDbContainerFactory>(c => new CosmosDbContainerFactory(client, cosmosDbConfig.DatabaseName, cosmosDbConfig.Containers));

    //        return services;
    //    }
    //}
}
