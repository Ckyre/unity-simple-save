using UnityEngine;
using Newtonsoft.Json;

public class Rigidbody2DSavable : Savable
{
    [System.Serializable]
    private class Rigidbody2DData
    {
        public float[] velocity;
        public float angularVelocity;
    }

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override string Capture()
    {
        Rigidbody2DData data = new Rigidbody2DData()
        {
            velocity = new float[2] { rb.velocity.x, rb.velocity.y },
            angularVelocity = rb.angularVelocity,
        };

        string json = JsonConvert.SerializeObject(data);
        return json;
    }

    public override void Restore(string json)
    {
        Rigidbody2DData data = JsonConvert.DeserializeObject<Rigidbody2DData>(json);

        rb.velocity = new Vector2(data.velocity[0], data.velocity[1]);
        rb.angularDrag = data.angularVelocity;
    }
}
