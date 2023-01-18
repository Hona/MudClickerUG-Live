using Newtonsoft.Json;

namespace MudClicker.Infrastructure;

public interface IDocument
{
    [JsonProperty("id")]
    string Id { get; }
    public int PartitionKey { get; set; }

}