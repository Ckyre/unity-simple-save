using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPosition : MonoBehaviour
{
    public float moveRate = 2;

    private float nextMove;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(Time.time > nextMove)
        {
            nextMove = Time.time + moveRate;
            transform.position = new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), 0f);

            rb.angularVelocity = Random.Range(-1000f, 1000f);
        }
    }
}
