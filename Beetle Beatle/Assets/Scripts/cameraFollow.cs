using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraFollow : MonoBehaviour
{

    public Transform playerTransform;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if(playerTransform != null)
        {

            transform.position = Vector2.Lerp(transform.position, playerTransform.position, speed);
        }
    }

}
