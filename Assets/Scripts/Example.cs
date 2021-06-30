using UnityEngine;
using FilePacker;
using UnityEngine.UI;

public class Example : MonoBehaviour
{
    public Text text;
    public Image image;

    private PackedFile file;

    public void OnLoadButton()
    {
        file = PackedFile.Load(Application.dataPath + "/file.save");

        text.text = file.GetString("text");

        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(file.GetBytes("image"));
        texture.Apply();
        image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }

    public void OnSaveButton()
    {
        file.Save();
    }

}
