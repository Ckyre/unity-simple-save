using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Text text;
    public Image image;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //SaveManager.Save();
            SceneManager.LoadScene(1);
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
