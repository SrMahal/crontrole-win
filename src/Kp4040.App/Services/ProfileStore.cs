using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Kp4040.App.Models;

namespace Kp4040.App.Services;

public sealed class ProfileStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public string ProfilePath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "KP4040AIController", "profiles", "default.json");

    public ControllerProfile Load()
    {
        if (!File.Exists(ProfilePath)) return ControllerProfile.CreateDefault();
        var loaded = JsonSerializer.Deserialize<ControllerProfile>(File.ReadAllText(ProfilePath), JsonOptions)
            ?? throw new InvalidDataException("O arquivo do perfil esta vazio.");
        if (loaded.SchemaVersion != 1)
            throw new InvalidDataException("Versao do perfil ainda nao suportada.");
        loaded.Mappings ??= new Dictionary<string, ActionKind>();
        loaded.Shortcuts ??= new Dictionary<string, KeyboardShortcut>();
        foreach (var button in ControllerButtons.All)
            loaded.Mappings.TryAdd(button.Key, ActionKind.None);
        return loaded;
    }

    public void Save(ControllerProfile profile)
    {
        var directory = Path.GetDirectoryName(ProfilePath)!;
        Directory.CreateDirectory(directory);
        var tempPath = ProfilePath + ".tmp";
        File.WriteAllText(tempPath, JsonSerializer.Serialize(profile, JsonOptions));
        File.Move(tempPath, ProfilePath, overwrite: true);
    }
}
