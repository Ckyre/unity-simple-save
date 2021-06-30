using System;
using UnityEngine;

public abstract class Savable : MonoBehaviour
{
    [ReadOnly] public string Uid;

    // Registering / Unregistering from SaveManager

    private void Awake()
    {
        if (string.IsNullOrEmpty(Uid))
        {
            Debug.LogError(gameObject + " this gameObject has no unique ID!");
            return;
        }

        SaveManager.savables.Add(Uid, this);
    }

    private void OnDisable()
    {
        if (SaveManager.savables.ContainsKey(Uid))
        {
            SaveManager.savables.Remove(Uid);
        }
    }

    private void OnDestroy()
    {
        if (SaveManager.savables.ContainsKey(Uid))
        {
            SaveManager.savables.Remove(Uid);
        };
    }

    // Capturing / Restoring states

    public virtual string Capture()
    {
        return null;
    }

    public virtual void Restore (string json)
    {
    }

    [ContextMenu("Generate GUID")]
    public void GenerateGUID()
    {
        Uid = Guid.NewGuid().ToString();
    }

    private void Reset()
    {
        if(string.IsNullOrEmpty(Uid))
            GenerateGUID();
    }
}
