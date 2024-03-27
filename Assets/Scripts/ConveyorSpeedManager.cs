using UnityEngine;

public class ConveyorSpeedManager : MonoBehaviour
{
    public int errorThreshold = 2; 
    public float speedReductionAmount = 0.2f; // Adjust as needed
   // public GameObject conveyorBelt; // Assign in the inspector

    private int goldBoltErrors = 0; 
    private int goldBoltErrorsTimes = 0; 
    private float originalConveyorSpeed;

    void Start()
    {
        // Assuming you have a script controlling the conveyor speed 
        //originalConveyorSpeed = conveyorBelt.GetComponent<ConveyorMovement>().conveyorSpeed;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Bolt") && other.GetComponent<MeshRenderer>().material.color == new Color(1f, 0.84f, 0f))
        {
            Debug.Log("Error made");

            goldBoltErrors++;
            Destroy(other.gameObject); 

            if (goldBoltErrors >= errorThreshold && goldBoltErrorsTimes < 4)
            {
                ReduceConveyorSpeed();
                goldBoltErrors = 0;
                goldBoltErrorsTimes += 1;
            }
        }
    }

    private void ReduceConveyorSpeed()
    {
        GameObject[] bolts = GameObject.FindGameObjectsWithTag("Bolt"); 
        foreach (GameObject bolt in bolts)
        {
            bolt.GetComponent<BoltMovement>().conveyorSpeed -= speedReductionAmount;
        }

        BoltSpawner.currentConveyorSpeed -= speedReductionAmount;
    }
}