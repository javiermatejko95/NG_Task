/// <summary>
/// Contract that any system must implement to participate in the save/load pipeline.
/// Each saveable system defines its own data shape (T) and knows how to
/// serialize itself to that shape and restore itself from it.
///
/// Usage:
///   public class InventorySaveHandler : MonoBehaviour, ISaveable<InventorySaveData> { ... }
/// </summary>
public interface ISaveable<T>
{
    /// <summary>Unique key used as the filename on disk. Must be stable across sessions.</summary>
    string SaveKey { get; }

    /// <summary>Collect current runtime state into a serializable data object.</summary>
    T Capture();

    /// <summary>Apply a previously saved data object back to the runtime state.</summary>
    void Restore(T data);
}
