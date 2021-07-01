using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System;

namespace GabrielRouleau.SaveManagement
{
    public static class SaveManager
    {
        public static string CommonSaveDirectory => $"{Application.persistentDataPath}/";
        public static string CommonExtension = "save";
        public static int FilesLimit = 2;

        public static Dictionary<string, Savable> SceneSavables = new Dictionary<string, Savable>();

        private static List<SaveFile> saveFiles = new List<SaveFile>();
        private static int currentSaveFileIndex = 0;

        // Events
        public delegate void SaveEvent();
        public static event SaveEvent OnSave;
        public delegate void LoadEvent();
        public static event LoadEvent OnLoad;

        #region Save files manipulations

        public static int CurrentSaveFile()
        {
            return currentSaveFileIndex;
        }

        public static void UseSaveFile (int index)
        {
            if(index > saveFiles.Count - 1)
            {
                Debug.LogError("Index out of range! Given index : " + index + ". List count : " + saveFiles.Count);
                return;
            }

            currentSaveFileIndex = index;
            Load();
        }

        public static int CreateSaveFile (string fileName)
        {
            if (File.Exists($"{CommonSaveDirectory}{fileName}.{CommonExtension}"))
            {
                Debug.LogError("File with name " + fileName + " already exists at " + CommonSaveDirectory);
                return -1;
            }

            saveFiles.Add(new SaveFile(fileName));
            return saveFiles.Count - 1;
        }

        public static void RemoveSaveFile (int index, bool removeOnDisk)
        {
            if (index < 0 || index > saveFiles.Count - 1)
            {
                Debug.LogError("Index out of range! Given index : " + index + ". List end : " + (saveFiles.Count - 1));
                return;
            }

            if (removeOnDisk)
            {
                if (File.Exists(saveFiles[index].GetPath()))
                {
                    File.Delete(saveFiles[index].GetPath());
                }
            }
            saveFiles.RemoveAt(index);
        }

        public static int AllSaveFilesFromCommonDir()
        {
            string[] filesPaths = Directory.GetFiles(CommonSaveDirectory, $"*.{CommonExtension}");
            for (int i = 0; i < filesPaths.Length; i++)
            {
                if(i < FilesLimit)
                {
                    string fileName = filesPaths[i].Replace(CommonSaveDirectory, "");
                    fileName = fileName.Replace($".{CommonExtension}", "");

                    saveFiles.Add(new SaveFile(fileName));
                }
                else
                {
                    Debug.LogWarning("Reached file count limit : (" + FilesLimit + ")");
                    break;
                }
            }
            return filesPaths.Length;
        }

        #endregion

        // Save
        public static bool Save()
        {
            if (SceneSavables.Count < 1) 
            {
                Debug.LogWarning("No SceneSavables in scene, Save method aborted!");
                return false;
            }

            if(saveFiles.Count < 1)
            {
                UseSaveFile(CreateSaveFile("auto"));
            }

            // Creating sections list with gameobjects
            SaveFileSection[] sections = new SaveFileSection[SceneSavables.Count];
            for (int i = 0; i < SceneSavables.Count; i++)
            {
                sections[i] = new SaveFileSection()
                {
                    guid = SceneSavables.Values.ElementAt(i).Uid,
                    json = SceneSavables.Values.ElementAt(i).Capture()
                };
            }

            // Saving in a file
            if (saveFiles[currentSaveFileIndex].Save(sections))
            {
                if (OnSave != null) OnSave();
                return true;
            }
            else
            {
                return false;
            }
            
        }

        // Load
        public static bool Load()
        {
            // Loading sections list from file
            SaveFileSection[] sections = saveFiles[currentSaveFileIndex].Load();
            
            // Applying to scene
            if(sections != null)
            {
                for (int i = 0; i < sections.Length; i++)
                {
                    if (SceneSavables.ContainsKey(sections[i].guid))
                    {
                        SceneSavables[sections[i].guid].Restore(sections[i].json);
                    }
                }

                if (OnLoad != null) OnLoad();
                return true;
            }
            else
            {
                return false;
            }
        }

    }

    [System.Serializable]
    public struct SaveFileSection
    {
        public string guid;
        public string json;
    }

    public class SaveFile
    {
        public string Name;

        private string path;
        private List<SaveFileSection> sections = new List<SaveFileSection>();

        // Encrypting
        private static readonly byte[] key = { 4, 1, 6, 2, 7, 5, 1, 9 };
        private static readonly byte[] iv = { 5, 8, 6, 7, 2, 8, 9, 2 };
        private static readonly DESCryptoServiceProvider des = new DESCryptoServiceProvider();

        public SaveFile (string name)
        {
            Name = name;
            path = $"{SaveManager.CommonSaveDirectory}{Name}.{SaveManager.CommonExtension}";
        }

        public bool Save (SaveFileSection[] sectionsToSave)
        {
            sections = new List<SaveFileSection>(sectionsToSave);

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Incorrect path (" + path + ")");
                return false;
            }

            try
            {
                using (Stream fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                using (Stream cs = new CryptoStream(fs, des.CreateEncryptor(key, iv), CryptoStreamMode.Write))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(cs, sectionsToSave);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }

            return true;
        }

        public SaveFileSection[] Load()
        {
            if (!File.Exists(path))
            {
                Debug.LogError("File doesn't exists at : " + path);
                return null;
            }

            try
            {
                using (Stream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (Stream cs = new CryptoStream(fs, des.CreateDecryptor(key, iv), CryptoStreamMode.Read))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    SaveFileSection[] returnSections = (SaveFileSection[])formatter.Deserialize(cs);
                    sections = new List<SaveFileSection>(returnSections);

                    return returnSections;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                throw e;
            }
        }

        public string GetPath()
        {
            return path;
        }
    }

}