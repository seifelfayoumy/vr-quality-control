using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltMovement : MonoBehaviour
{
    public float conveyorSpeed = 1.0f; 
    private bool onConveyor = false;
    public GameObject conveyorWall;

    void Update()
    {

        if (onConveyor) 
        {
            transform.position += (Vector3.right * conveyorSpeed * Time.deltaTime);
        }

        conveyorSpeed = BoltSpawner.currentConveyorSpeed;
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

    public void hold()
    {
        if (this.GetComponent<MeshRenderer>().material.color != new Color(1f, 0.84f, 0f))
        {
            Debug.Log("hold silver bolt");
            ConveyorSpeedManager.madeError();
        }
    }
    
    
}
