using System.Collections.Concurrent;

namespace AsyncToolbox.Utilities;

internal static class SharedObjectPool<TObject> where TObject : class
{
    private static readonly ConcurrentBag<TObject> PooledObjects = new();

    /// <summary>
    /// Max count of item that can be stored in this pool.
    /// </summary>
    public static int MaxCount { get; set; } = Environment.ProcessorCount;
    
    /// <summary>
    /// Take out an item from this object pool.
    /// </summary>
    /// <returns></returns>
    public static TObject? Take() => PooledObjects.TryTake(out var instance) ? instance : null;
    
    /// <summary>
    /// Put an object into this pool.
    /// </summary>
    /// <param name="item">Item to put into the pool.</param>
    public static bool Put(TObject item)
    {
        if (PooledObjects.Count >= MaxCount)
            return false;
        PooledObjects.Add(item);
        return true;
    }
}