using System.Collections.Concurrent;

public static class EntityManager
{
    private static ConcurrentDictionary<int, NetworkTransform> entities = new ConcurrentDictionary<int, NetworkTransform>();

    public static bool Register(int entityId, NetworkTransform networkTransform)
    {
        if (entities.ContainsKey(entityId))
        {
            return false;
        }
        entities[entityId] = networkTransform;
        return true;
    }

    public static bool TryGet(int entityId, out NetworkTransform networkTransform)
    {
        return entities.TryGetValue(entityId, out networkTransform);
    }

    public static bool UnRegister(int entityId)
    {
        if (entities.ContainsKey(entityId))
        {
            return entities.TryRemove(entityId, out var _);
        }
        return false;
    }

    public static void Clear()
    {
        entities.Clear();
    }
}
