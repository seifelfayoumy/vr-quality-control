using UnityEngine;

public class ConveyorSpeedManager : MonoBehaviour
{
    public int errorThreshold = 2; 
    public float speedReductionAmount = 0.2f; // Adjust as needed
   // public GameObject conveyorBelt; // Assign in the inspector

    private int goldBoltErrors = 0; 
    private int goldBoltErrorsTimes = 0; 
    private float originalConveyorSpeed;
    private float timeSinceLastError = 0f;

    private bool increasedSpeed1 = false;
    private bool increasedSpeed2 = false;



    void Start()
    {
        // Assuming you have a script controlling the conveyor speed 
        //originalConveyorSpeed = conveyorBelt.GetComponent<ConveyorMovement>().conveyorSpeed;
    }
    
    private void Update()
    {
        if(BoltSpawner.gameOn)
        {
            timeSinceLastError += Time.deltaTime;
        }


        if (timeSinceLastError > 30f)
        {
            if (!increasedSpeed1)
            {
                increaseConveyorSpeed();
                increasedSpeed1 = true;
            }
            Debug.Log("activating double bolts");

            BoltSpawner.doubleBolts = true;
        }
        
        if (timeSinceLastError > 60f)
        {
            if (!increasedSpeed2)
            {
                increaseConveyorSpeed();
                increasedSpeed2 = true;
            }
            Debug.Log("activating triple bolts");

            BoltSpawner.tripleBolts = true;
        }
    }


    private void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("Bolt") && other.GetComponent<MeshRenderer>().material.color == new Color(1f, 0.84f, 0f))
        {
            Debug.Log("Error made");
            timeSinceLastError = 0.0f;
            increasedSpeed1 = false;
            increasedSpeed2 = false;

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
        Debug.Log("Reducing Speed");

        if (BoltSpawner.tripleBolts)
        {
            BoltSpawner.tripleBolts = false;
            Debug.Log("no triple bolts");
        }else if (BoltSpawner.doubleBolts)
        {
            Debug.Log("no double bolts");

            BoltSpawner.doubleBolts = false;
        }
        

        GameObject[] bolts = GameObject.FindGameObjectsWithTag("Bolt"); 
        foreach (GameObject bolt in bolts)
        {
            if (bolt.GetComponent<BoltMovement>().conveyorSpeed > speedReductionAmount)
            {
                bolt.GetComponent<BoltMovement>().conveyorSpeed -= speedReductionAmount;

            }
        }

        if (BoltSpawner.currentConveyorSpeed > speedReductionAmount)
        {
            BoltSpawner.currentConveyorSpeed -= speedReductionAmount;

        }
        
        Debug.Log("Current Speed: "+ BoltSpawner.currentConveyorSpeed);

        
    }
    
    private void increaseConveyorSpeed()
    {
        Debug.Log("Increasing Speed");
        
        

        GameObject[] bolts = GameObject.FindGameObjectsWithTag("Bolt"); 
        foreach (GameObject bolt in bolts)
        {
            if (bolt.GetComponent<BoltMovement>().conveyorSpeed < 1)
            {
                bolt.GetComponent<BoltMovement>().conveyorSpeed += speedReductionAmount;

            }
        }

        if (BoltSpawner.currentConveyorSpeed < 1)
        {
            BoltSpawner.currentConveyorSpeed += speedReductionAmount;

        }
        
        Debug.Log("Current Speed: "+ BoltSpawner.currentConveyorSpeed);

        
    }
}