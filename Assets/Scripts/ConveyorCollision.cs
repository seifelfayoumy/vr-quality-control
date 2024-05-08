using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorCollision : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Bolt")) {
            ConveyorSpeedManager.boltsTouching++;
        }
    }

    public void OnTriggerExit(Collider other) {
        if (other.CompareTag("Bolt")) {
            ConveyorSpeedManager.boltsTouching--;
        }
    }
}
