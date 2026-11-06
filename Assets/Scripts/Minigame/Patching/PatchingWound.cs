using UnityEngine;

public sealed class PatchingWound
{
    public PatchingWound(GameObject spawnedObject, GameObject fuzzyObject)
    {
        SpawnedObject = spawnedObject;
        FuzzyObject = fuzzyObject;
        HasFuzzy = fuzzyObject != null;
    }

    public GameObject SpawnedObject { get; }
    public GameObject FuzzyObject { get; private set; }
    public bool HasFuzzy { get; private set; }
    public bool IsPatchable => !HasFuzzy;

    public void CleanFuzzy()
    {
        HasFuzzy = false;

        if (FuzzyObject != null)
        {
            Object.Destroy(FuzzyObject);
            FuzzyObject = null;
        }
    }
}
