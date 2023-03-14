using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public float speed = 1.0f; 
    public float distance = 1.2f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float newPosition = Mathf.Sin(Time.time * speed) * distance;
        transform.position = startPos + new Vector3(newPosition, 0, 0);
    }
}
