using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalMovement : MonoBehaviour
{
    public float speed = 2f;
    public Vector3 startPos;
    public Vector3 endPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPos, Time.deltaTime * speed);
        if (Vector3.Distance(transform.position, endPos) < 0.001f)
        {
            // Swap the position of the cylinder.
            transform.position = startPos;
        }
    }
}
