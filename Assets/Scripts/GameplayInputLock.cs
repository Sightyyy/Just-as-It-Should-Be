using System.Collections.Generic;

public static class GameplayInputLock
{
    private static readonly HashSet<object> owners = new HashSet<object>();

    public static bool IsLocked => owners.Count > 0;

    public static void Lock(object owner)
    {
        if (owner != null)
        {
            owners.Add(owner);
        }
    }

    public static void Unlock(object owner)
    {
        if (owner != null)
        {
            owners.Remove(owner);
        }
    }
}
