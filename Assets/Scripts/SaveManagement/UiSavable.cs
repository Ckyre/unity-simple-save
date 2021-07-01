using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using GabrielRouleau.SaveManagement;

public class UiSavable : Savable
{
    [System.Serializable]
    private class UiData
    {
        public string textContent;
        public byte[] imageTexture;
        public int textureFormat;
    }

    public Text text;
    public Image image;

    public override string Capture()
    {
        UiData data = new UiData()
        {
            textContent = text.text,
            imageTexture = image.sprite.texture.GetRawTextureData(),
            textureFormat = (int)image.sprite.texture.format
        };

        string json = JsonConvert.SerializeObject(data);
        return json;
    }

    public override void Restore(string json)
    {
        UiData data = JsonConvert.DeserializeObject<UiData>(json);

        text.text = data.textContent;

        Texture2D tex = new Texture2D(300, 168, (TextureFormat)data.textureFormat, false);
        tex.LoadRawTextureData(data.imageTexture);
        tex.Apply();
        image.sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
}
