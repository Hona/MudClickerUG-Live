using MudClicker.Infrastructure;

namespace MudClicker.Domain.Models;

public class Player : IDocument
{
    public string Id { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public PlayerSkills Skills { get; set; } = new();
}