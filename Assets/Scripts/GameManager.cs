using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using GabrielRouleau.SaveManagement;

public class GameManager : MonoBehaviour
{
    public Text text;
    public Image image;

    public Sprite hdImage;

    private void Start()
    {
        SaveManager.AllSaveFilesFromCommonDir();
        SaveManager.UseSaveFile(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(1);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            image.sprite = hdImage;
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
