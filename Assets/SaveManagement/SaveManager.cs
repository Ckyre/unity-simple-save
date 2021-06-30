using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System;

public static class SaveManager
{
    public static string savePath => $"{Application.persistentDataPath}/game.save";
    public static Dictionary<string, Savable> savables = new Dictionary<string, Savable>();

    private static readonly byte[] key = { 4, 1, 6, 2, 7, 5, 1, 9 };
    private static readonly byte[] iv = { 5, 8, 6, 7, 2, 8, 9, 2 };
    private static readonly DESCryptoServiceProvider des = new DESCryptoServiceProvider();

    [System.Serializable]
    private struct SaveFileSection
    {
        public string guid;
        public string json;
    }

    public static bool Save()
    {
        if (savables.Count < 1 || string.IsNullOrEmpty(savePath)) return false;

        try
        {
            SaveFileSection[] sections = new SaveFileSection[savables.Count];
            for (int i = 0; i < savables.Count; i++)
            {
                SaveFileSection section = new SaveFileSection()
                {
                    guid = savables.Values.ElementAt(i).Uid,
                    json = savables.Values.ElementAt(i).Capture()
                };

                sections[i] = section;
            }

            using (Stream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            using (Stream cs = new CryptoStream(fs, des.CreateEncryptor(key, iv), CryptoStreamMode.Write))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(cs, sections);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw e;
        }

        return true;
    }

    public static bool Load()
    {
        if (!File.Exists(savePath)) return false;

        try
        {
            using (Stream fs = new FileStream(savePath, FileMode.Open, FileAccess.Read))
            using (Stream cs = new CryptoStream(fs, des.CreateDecryptor(key, iv), CryptoStreamMode.Read))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                SaveFileSection[] sections = (SaveFileSection[])formatter.Deserialize(cs);

                // Apply loaded file to scene
                for (int i = 0; i < sections.Length; i++)
                {
                    if (savables.ContainsKey(sections[i].guid))
                    {
                        savables[sections[i].guid].Restore(sections[i].json);
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw e;
        }

        return true;
    }

}
