using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class UiSavable : Savable
{
    [System.Serializable]
    private class UiData
    {
        public string textContent;
        public byte[] imageTexture;
    }

    public Text text;
    public Image image;

    public override string Capture()
    {
        byte[] imageData = image.sprite.texture.GetRawTextureData();

        UiData data = new UiData()
        {
            textContent = text.text,
            imageTexture = imageData
        };

        string json = JsonConvert.SerializeObject(data);
        return json;
    }

    public override void Restore(string json)
    {
        UiData data = JsonConvert.DeserializeObject<UiData>(json);

        text.text = data.textContent;

        Texture2D tex = new Texture2D(1, 1);
        tex.LoadImage(data.imageTexture);
        tex.Apply();
        image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0, 0));
    }
}
