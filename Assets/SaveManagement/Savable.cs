using System;
using UnityEngine;

namespace GabrielRouleau.SaveManagement
{
    public abstract class Savable : MonoBehaviour
    {
        [ReadOnly] public string Uid;

        #region Registering / Unregistering from SaveManager

        private void Awake()
        {
            if (string.IsNullOrEmpty(Uid))
            {
                Debug.LogError(gameObject + " this gameObject has no unique ID!");
                return;
            }

            SaveManager.SceneSavables.Add(Uid, this);
        }

        private void OnDisable()
        {
            if (SaveManager.SceneSavables.ContainsKey(Uid))
            {
                SaveManager.SceneSavables.Remove(Uid);
            }
        }

        private void OnDestroy()
        {
            if (SaveManager.SceneSavables.ContainsKey(Uid))
            {
                SaveManager.SceneSavables.Remove(Uid);
            };
        }

        #endregion

        #region Capturing / Restoring states

        public virtual string Capture()
        {
            return null;
        }

        public virtual void Restore(string json)
        {
        }

        #endregion

        #region Generating unique ID

        [ContextMenu("Generate GUID")]
        public void GenerateGUID()
        {
            Uid = Guid.NewGuid().ToString();
        }

        private void Reset()
        {
            if (string.IsNullOrEmpty(Uid))
                GenerateGUID();
        }
        
        #endregion

    }
}