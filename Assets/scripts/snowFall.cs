using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class snowFall : MonoBehaviour
{
    private void Update()
    {
        if (transform.position.y <= -10)
        {
            transform.position += Vector3.up* 20;
        }
        transform.position += Vector3.down *Time.deltaTime;
    }
}
