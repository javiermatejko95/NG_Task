using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Low-level save/load layer. Handles only JSON serialization and file I/O.
/// Completely decoupled from game logic — works with any serializable type T.
///
/// Files are stored in: Application.persistentDataPath/saves/<key>.json
///
/// To do:
///   - Add encryption by wrapping the json string before writing / after reading.
/// </summary>
public static class SaveSystem
{
    // ──────────────────────────────────────────────
    // CONFIGURATION
    // ──────────────────────────────────────────────
    private const string SAVE_FOLDER = "saves";
    private static string SaveDirectory => Path.Combine(Application.persistentDataPath, SAVE_FOLDER);

    // ──────────────────────────────────────────────
    // PUBLIC API
    // ──────────────────────────────────────────────

    /// <summary>
    /// Serializes <paramref name="data"/> to JSON and writes it to disk.
    /// </summary>
    /// <returns>True if the write succeeded.</returns>
    public static bool Save<T>(string key, T data)
    {
        try
        {
            EnsureDirectoryExists();

            string json = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(BuildPath(key), json);

            Debug.Log($"[SaveSystem] Saved '{key}' → {BuildPath(key)}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to save '{key}': {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Reads the JSON file for <paramref name="key"/> and deserializes it.
    /// </summary>
    /// <param name="data">The deserialized object, or default(T) if not found / error.</param>
    /// <returns>True if the file existed and was loaded successfully.</returns>
    public static bool TryLoad<T>(string key, out T data)
    {
        string path = BuildPath(key);

        if (!File.Exists(path))
        {
            Debug.Log($"[SaveSystem] No save file found for '{key}'.");
            data = default;
            return false;
        }

        try
        {
            string json = File.ReadAllText(path);
            data = JsonConvert.DeserializeObject<T>(json);
            Debug.Log($"[SaveSystem] Loaded '{key}' ← {path}");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveSystem] Failed to load '{key}': {e.Message}");
            data = default;
            return false;
        }
    }

    /// <summary>
    /// Deletes the save file for <paramref name="key"/> if it exists.
    /// </summary>
    public static void Delete(string key)
    {
        string path = BuildPath(key);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveSystem] Deleted save '{key}'.");
        }
    }

    /// <summary>
    /// Returns true if a save file exists for the given key.
    /// </summary>
    public static bool Exists(string key)
    {
        return File.Exists(BuildPath(key));
    }

    /// <summary>
    /// Deletes ALL save files in the save directory. Use with caution.
    /// </summary>
    public static void DeleteAll()
    {
        if (!Directory.Exists(SaveDirectory)) return;

        foreach (string file in Directory.GetFiles(SaveDirectory, "*.json"))
            File.Delete(file);

        Debug.Log("[SaveSystem] All save files deleted.");
    }

    // ──────────────────────────────────────────────
    // PRIVATE HELPERS
    // ──────────────────────────────────────────────

    private static string BuildPath(string key) =>
        Path.Combine(SaveDirectory, $"{key}.json");

    private static void EnsureDirectoryExists()
    {
        if (!Directory.Exists(SaveDirectory))
            Directory.CreateDirectory(SaveDirectory);
    }
}
