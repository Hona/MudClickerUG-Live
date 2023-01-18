using System.Linq.Expressions;
using System.Net;
using Microsoft.Azure.Cosmos;

namespace MudClicker.Infrastructure;

public class GenericRepository
{
    private readonly CosmosClient _cosmosClient;

    public GenericRepository(CosmosClient cosmosClient)
    {
        _cosmosClient = cosmosClient;
    }

    private async Task<Container> GetContainerAsync<T>() where T : IDocument
    {
        var result = await _cosmosClient.CreateDatabaseIfNotExistsAsync(DbConstants.DatabaseName);
        
        var database = result.Database;
        // get the container from cosmos sdk
        var container = await database.CreateContainerIfNotExistsAsync(
            typeof(T).FullName,
            "/" + nameof(IDocument.PartitionKey),
            400);
        return container.Container;
    }
    

    public async Task<T?> GetItemAsync<T>(T model, Func<T, string> idExpression, Expression<Func<T, string>> partitionKeyExpression) where T : IDocument
    {
        var container = await GetContainerAsync<T>();

        try
        {
            var getPartitionKey = partitionKeyExpression.Compile();
            
            // get the item from cosmos sdk
            var response = await container.ReadItemAsync<T>(idExpression(model), new PartitionKey(nameof(IDocument.PartitionKey)));
            return response.Resource;
        }
        catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }
    }
    
    public async Task<IEnumerable<T>> GetItemListAsync<T>() where T : IDocument
    {
        var container = await GetContainerAsync<T>();

        try
        {
            var output = new List<T>();

            using var resultSet = container.GetItemQueryIterator<T>(
                queryDefinition: null,
                requestOptions: new QueryRequestOptions
                {
                });
            
            while (resultSet.HasMoreResults)
            {
                var response = await resultSet.ReadNextAsync();
                    
                output.AddRange(response);
            }

            return output;
        }
        catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
        {
            return Enumerable.Empty<T>();
        }
    }

    
    public async Task CreateItemAsync<T>(T item, Expression<Func<T, string>> partitionKeyExpression) where T : IDocument
    {
        var container = await GetContainerAsync<T>();
        var getPartitionKey = partitionKeyExpression.Compile();

        await container.CreateItemAsync(item, new PartitionKey(getPartitionKey(item)));
    }
    
    public async Task CreateOrUpdateItemAsync<T>(T item) where T : IDocument
    {
        var container = await GetContainerAsync<T>();
        await container.UpsertItemAsync(item);
    }
    
    public async Task DeleteItemAsync<T>(T item, Expression<Func<T, string>> partitionKeyExpression) where T : IDocument
    {
        var container = await GetContainerAsync<T>();
        
        var getPartitionKey = partitionKeyExpression.Compile();
        await container.DeleteItemAsync<T>(item.Id, new PartitionKey(getPartitionKey(item)));
    }
}