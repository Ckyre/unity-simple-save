using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using UnityEngine;

public class SaveFile
{
    public GameData Data;
    public string Path;

    // Events
    public delegate void LoadEvent();
    public static event LoadEvent OnLoad;

    public delegate void SaveEvent();
    public static event SaveEvent OnSave;

    private static readonly byte[] key = { 4, 1, 6, 2, 7, 5, 1, 9 };
    private static readonly byte[] iv = { 5, 8, 6, 7, 2, 8, 9, 2 };
    private static readonly DESCryptoServiceProvider des = new DESCryptoServiceProvider();

    #region Load
    public SaveFile()
    {
        Data = new GameData();
    }

    public SaveFile (string path)
    {
        Load(path);
    }

    public bool Load (string path)
    {
        if (!File.Exists(path))
        {
            Debug.Log("Cannot find save file at " + path);
            return false;
        }

        try
        {
            using (Stream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (Stream cs = new CryptoStream(fs, des.CreateDecryptor(key, iv), CryptoStreamMode.Read))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                Data = (GameData)formatter.Deserialize(cs);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw e;
        }

        Path = path;

        if (OnLoad != null)
            OnLoad();

        Debug.Log("Loaded file at " + Path);
        return true;
    }
    #endregion

    public bool Save()
    {
        if(Path == null || Path.Length < 5)
        {
            Debug.Log("Incorrect file path! Can't save this file. Given path : '" + Path + "'");
            return false;
        }

        try
        {
            using (Stream fs = new FileStream(Path, FileMode.Create, FileAccess.Write))
            using (Stream cs = new CryptoStream(fs, des.CreateEncryptor(key, iv), CryptoStreamMode.Write))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(cs, Data);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
            throw e;
        }

        if (OnSave != null)
            OnSave();

        Debug.Log("Saved file at " + Path);
        return true;
    }
}