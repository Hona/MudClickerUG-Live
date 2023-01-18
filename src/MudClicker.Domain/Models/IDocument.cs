using Newtonsoft.Json;

namespace MudClicker.Infrastructure;

public interface IDocument
{
    [JsonProperty("id")]
    string Id { get; }
}