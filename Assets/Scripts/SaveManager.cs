using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public string relativePath;
    private SaveFile file;


    private void Start()
    {
        Load();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Save();
        }
    }


    public void Save()
    {
        file.Save();
    }

    public void Load()
    {
        file = new SaveFile(Application.dataPath + relativePath);
        UIManager.instance.UpdateUI(file.Data);
    }
}
