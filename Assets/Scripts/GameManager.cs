using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text text;
    public Image image;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SaveManager.Save();
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            SaveManager.Load();
        }
    }

    public void OnLoadButton()
    {
        SaveManager.Load();
    }

    public void OnSaveButton()
    {
        SaveManager.Save();
    }
}
