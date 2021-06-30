using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace FilePacker
{
    //
    // ADDING NEW TYPE :
    //
    // 01. Update switch statement under 'Load > 03 : Sections values'
    // 02. Update if/else statements under 'Save > Sections values'
    // 03. Add a new 'GetData' function under 'Get data'
    // 04. Add a 'SetData' function overload under 'Set data'
    // 05. Update if/else statements under 'GetSectionValue'
    //


    public class PackedFile
    {
        public string Path;

        private List<SectionDefinition> sectionDefinitions = new List<SectionDefinition>();
        private List<PackedSection> sectionValues = new List<PackedSection>();

        #region Constructors
        public PackedFile()
        {
        }

        private PackedFile(List<SectionDefinition> sectionDefinitions, List<PackedSection> sectionValues)
        {
            this.sectionDefinitions = sectionDefinitions;
            this.sectionValues = sectionValues;
        }
        #endregion

        #region Load
        public static PackedFile Load(byte[] data)
        {
            int offset = 0;

            // 01 : Sections count
            int sectionCount = BitConverter.ToInt32(data, 0);
            offset += 4;

            // 02 : Sections definitions
            List<SectionDefinition> definitions = new List<SectionDefinition>();
            for (int i = 0; i < sectionCount; i++)
            {
                // Name size
                int _nameSize = BitConverter.ToInt32(data, offset);
                offset += 4;

                // Name
                byte[] _nameBytes = new byte[_nameSize];
                Array.Copy(data, offset, _nameBytes, 0, _nameSize);
                string name = Encoding.ASCII.GetString(_nameBytes);
                offset += _nameSize;

                // Type
                int type = BitConverter.ToInt32(data, offset);
                offset += 4;

                // Size
                int size = BitConverter.ToInt32(data, offset);
                offset += 4;

                definitions.Add(new SectionDefinition(name, (SectionType)type, size));
            }

            // 03 : Sections values
            List<PackedSection> values = new List<PackedSection>();
            for (int i = 0; i < sectionCount; i++)
            {
                byte[] valueBytes = new byte[definitions[i].Size];
                Array.Copy(data, offset, valueBytes, 0, definitions[i].Size);

                switch (definitions[i].Type)
                {
                    case SectionType.String:
                        string value = Encoding.ASCII.GetString(valueBytes);
                        values.Add(new StringSection(value));
                        break;
                    case SectionType.Bytes:
                        values.Add(new BytesSection(valueBytes));
                        break;
                }

                offset += definitions[i].Size;
            }

            return new PackedFile(definitions, values);
        }

        public static PackedFile Load(string path)
        {
            PackedFile file = Load(File.ReadAllBytes(path));
            file.Path = path;
            return file;
        }
        #endregion

        public static void PrintError(string errorMessage)
        {
            Debug.LogError("[File Packer] " + errorMessage);
        }

        #region Save
        public void Save()
        {
            if(Path == "" || Path == null)
            {
                PrintError("The path of this file is null, cannot save this file.");
                return;
            }

            List<byte> data = new List<byte>();

            // Sections count
            data.AddRange(BitConverter.GetBytes(sectionDefinitions.Count));

            // Sections definitions
            for (int i = 0; i < sectionDefinitions.Count; i++)
            {
                // Name
                data.AddRange(BitConverter.GetBytes(sectionDefinitions[i].Name.Length));
                data.AddRange(Encoding.ASCII.GetBytes(sectionDefinitions[i].Name));
                // Type
                data.AddRange(BitConverter.GetBytes((int)sectionDefinitions[i].Type));
                // Size
                data.AddRange(BitConverter.GetBytes(sectionDefinitions[i].Size));
            }

            // Sections values
            for (int i = 0; i < sectionDefinitions.Count; i++)
            {
                if (sectionValues[i].Type == typeof(string))
                {
                    StringSection cast = (StringSection)sectionValues[i];
                    data.AddRange(Encoding.ASCII.GetBytes(cast.Value));
                }
                else if (sectionValues[i].Type == typeof(byte[]))
                {
                    BytesSection cast = (BytesSection)sectionValues[i];
                    data.AddRange(cast.Value);
                }
            }

            File.WriteAllBytes(Path, data.ToArray());
            Debug.Log("Saved file!");
        }

        public void Save(string path)
        {
            Path = path;
            Save();
        }
        #endregion

        public bool DataExists(string name)
        {
            foreach (SectionDefinition definition in sectionDefinitions)
            {
                if (definition.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        #region Get data
        public string GetString (string name)
        {
            for (int i = 0; i < sectionDefinitions.Count; i++)
            {
                if (sectionDefinitions[i].Name == name)
                {
                    return GetSectionValue<string>(sectionValues[i]);
                }
            }

            PrintError("Can't find data with name : " + name);
            return null;
        }

        public byte[] GetBytes (string name)
        {
            for (int i = 0; i < sectionDefinitions.Count; i++)
            {
                if (sectionDefinitions[i].Name == name)
                {
                    return GetSectionValue<byte[]>(sectionValues[i]);
                }
            }

            PrintError("Can't find data with name : " + name);
            return null;
        }
        #endregion

        #region Set data
        public void AddData (string name, string value)
        {
            if (!DataExists(name))
            {
                SectionDefinition definition = new SectionDefinition();
                definition.Name = name;
                definition.Type = SectionType.String;
                definition.Size = value.Length;
                sectionDefinitions.Add(definition);

                StringSection sectionValue = new StringSection(value);
                sectionValues.Add(sectionValue);
            } else { PrintError(name + " already exists."); }
        }

        public void AddData (string name, byte[] value)
        {
            if (!DataExists(name))
            {
                SectionDefinition definition = new SectionDefinition();
                definition.Name = name;
                definition.Type = SectionType.Bytes;
                definition.Size = value.Length;
                sectionDefinitions.Add(definition);

                BytesSection sectionValue = new BytesSection(value);
                sectionValues.Add(sectionValue);
            } else { PrintError(name + " already exists."); }
        }
        #endregion

        private T GetSectionValue<T> (PackedSection section)
        {
            if (section.Type == typeof(string))
            {
                return (T)Convert.ChangeType(((StringSection)section).Value, typeof(T));
            }
            else if (section.Type == typeof(byte[]))
            {
                return (T)Convert.ChangeType(((BytesSection)section).Value, typeof(T));
            }
            return default(T);
        }



    }

    public struct SectionDefinition
    {
        public string Name;
        public SectionType Type;
        public int Size;

        public SectionDefinition(string name, SectionType type, int size)
        {
            Name = name;
            Type = type;
            Size = size;
        }
    }
}


