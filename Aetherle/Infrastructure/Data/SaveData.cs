using System;
using Aetherle.Core.Models;

namespace Aetherle.Infrastructure.Data;

[Serializable]
public class SaveData
{
    public string TargetWord { get; set; } = string.Empty;
    public GameState State { get; set; } = new();
    public DateTime LastSaved { get; set; } = DateTime.UtcNow;
}
