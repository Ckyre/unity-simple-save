using UnityEngine;
using Newtonsoft.Json;
using GabrielRouleau.SaveManagement;

public class TransformSavable : Savable
{
    [System.Serializable]
    private class TransformData 
    {
        public float[] position;
        public float[] eulerAngles;
        public float[] localScale;
    }

    public override string Capture()
    {
        TransformData data = new TransformData()
        {
            position = new float[3] { transform.position.x, transform.position.y, transform.position.z },
            eulerAngles = new float[3] { transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z },
            localScale = new float[3] { transform.localScale.x, transform.localScale.y, transform.localScale.z }
        };

        string json = JsonConvert.SerializeObject(data);
        return json;
    }

    public override void Restore(string json)
    {
        TransformData data = JsonConvert.DeserializeObject<TransformData>(json);

        transform.position = new Vector3(data.position[0], data.position[1], data.position[2]);
        transform.eulerAngles = new Vector3(data.eulerAngles[0], data.eulerAngles[1], data.eulerAngles[2]);
        transform.localScale = new Vector3(data.localScale[0], data.localScale[1], data.localScale[2]);
    }
}
