using System;
using UnityEngine;

public abstract class Savable : MonoBehaviour
{
    [HideInInspector] public string Uid;

    // Registering / Unregistering from SaveManager

    private void Awake()
    {
        Uid = Guid.NewGuid().ToString();
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
}
