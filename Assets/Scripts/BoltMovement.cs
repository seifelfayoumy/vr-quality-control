using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltMovement : MonoBehaviour
{
    public float conveyorSpeed = 1.0f; 
    private bool onConveyor = false;

    void Update()
    {
        if (onConveyor) 
        {
            transform.Translate(Vector3.right * conveyorSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Conveyor")) 
        {
            onConveyor = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Conveyor")) 
        {
            onConveyor = false;
        }
    }
    
}
