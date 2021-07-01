using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using GabrielRouleau.SaveManagement;

public class ImageSavable : Savable
{
    [System.Serializable]
    private class ImageData
    {
        public float[] color;
        public int textureWidth, textureHeight;
        public int textureFormat;
        public byte[] textureData;
    }

    private Image image;

    private void Start()
    {
        image = GetComponent<Image>();
    }

    public override string Capture()
    {
        ImageData data = new ImageData()
        {
            color = new float[4] { image.color.r, image.color.g, image.color.b, image.color.a },
            textureWidth = image.sprite.texture.width,
            textureHeight = image.sprite.texture.height,
            textureFormat = (int)image.sprite.texture.format,
            textureData = image.sprite.texture.GetRawTextureData()
        };

        string json = JsonConvert.SerializeObject(data);
        return json;
    }

    public override void Restore(string json)
    {
        ImageData data = JsonConvert.DeserializeObject<ImageData>(json);

        image.color = new Color(data.color[0], data.color[1], data.color[2], data.color[3]);
        
        Texture2D texture = new Texture2D(data.textureWidth, data.textureHeight, (TextureFormat)data.textureFormat, false);
        texture.LoadRawTextureData(data.textureData);
        texture.Apply();
        image.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
    }
}
