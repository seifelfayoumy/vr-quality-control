using TMPro;
using UnityEngine;

public class ConveyorSpeedManager : MonoBehaviour
{
    public static int errorThreshold = 2;
    public static float speedReductionAmount = 0.2f; // Adjust as needed
                                                     // public GameObject conveyorBelt; // Assign in the inspector

    private static int goldBoltErrors = 0;
    private static int goldBoltErrorsTimes = 0;
    private static float originalConveyorSpeed;
    private static float timeSinceLastError = 0f;
    private static float gameTime = 0f;


    private static bool increasedSpeed1 = false;
    private static bool increasedSpeed2 = false;
    private static bool increasedSpeed3 = false;

    public static bool adaptive = true;
    public static bool hardVersion = false;
    public GameObject experiencePanel;
    public GameObject startPanel;

    public static float errorsMadeByUser = 0.0f;
    public static float silverBoltsPassed = 0.0f;
    public static float score = 100.0f;

    [SerializeField] public GameObject scoreText;

    public static bool boltsSpawning = false;
    public static int boltsTouching = 0;

    void Start()
    {

     

        if (!adaptive)
        {
            experiencePanel.SetActive(false);
            startPanel.SetActive(true);

        }
        // Assuming you have a script controlling the conveyor speed 
        //originalConveyorSpeed = conveyorBelt.GetComponent<ConveyorMovement>().conveyorSpeed;
    }



    private void Update()
    {
        if(boltsTouching == 0 && boltsSpawning) {
           // scoreText.active = true;
        }

        if (BoltSpawner.gameOn)
        {
            gameTime += Time.deltaTime;

        }
        if (adaptive) {


            if (BoltSpawner.gameOn) {
                timeSinceLastError += Time.deltaTime;
            }

            if (timeSinceLastError >= 15f) {
                timeSinceLastError = 0.0f;
                increaseConveyorSpeed();
                if (!BoltSpawner.doubleBolts) {
                    Debug.Log("activating double bolts");

                    BoltSpawner.doubleBolts = true;
                } else if (!BoltSpawner.tripleBolts) {
                    Debug.Log("activating triple bolts");

                    BoltSpawner.tripleBolts = true;
                }
            }
        }
        else
        {
            if (hardVersion) {
                BoltSpawner.startGameHard();
            } else {
                if (gameTime > 50f) {
                    BoltSpawner.startGameHard();

                } else if (gameTime > 25f) {
                    BoltSpawner.startGameMedium();
                }
            }

            

     
        }

        if(gameTime >= 75.0f) {
            BoltSpawner.gameOn = false;
            scoreText.active = true;
        }

        score = ((silverBoltsPassed - errorsMadeByUser) / BoltSpawner.silverBoltsCount)*100.0f;


        scoreText.GetComponent<TextMeshProUGUI>().text =  "Score: " + score;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bolt") && other.GetComponent<MeshRenderer>().material.color == new Color(1f, 0.84f, 0f))
        {
            boltsSpawning = true;
            madeError();
            errorsMadeByUser += 1.0f;
        } else if(other.CompareTag("Bolt")) {
            boltsSpawning = true;
            silverBoltsPassed += 1.0f;
        }

        Destroy(other.gameObject);
    }

    public static void pupiSizeIncrease()
    
    {
        if (adaptive) {
            Debug.LogWarning("Pupil Size has been high for some time, decreasing speed");

            ReduceConveyorSpeed();
        }


    }
    public static void pupiSizeDecrease()
    {
        if (adaptive) {
            Debug.LogWarning("Pupil Size has been low for some time, increasing speed");

            increaseConveyorSpeed();
            if (!BoltSpawner.doubleBolts) {
                BoltSpawner.doubleBolts = true;
            } else if (!BoltSpawner.tripleBolts) {
                BoltSpawner.tripleBolts = true;

            }
        }

    }

    public static void madeError()
    {
        Debug.Log("Error made");

        if (adaptive)
        {


            timeSinceLastError = 0.0f;
            increasedSpeed1 = false;
            increasedSpeed2 = false;
            increasedSpeed3 = false;

            goldBoltErrors++;

            if (goldBoltErrors >= errorThreshold)
            {
                ReduceConveyorSpeed();
                goldBoltErrors = 0;
                goldBoltErrorsTimes += 1;
            }
        }
    }


    private static void ReduceConveyorSpeed()
    {
        if (adaptive)
        {


            Debug.Log("Reducing Speed from " + BoltSpawner.currentConveyorSpeed);

            if (BoltSpawner.tripleBolts)
            {
                BoltSpawner.tripleBolts = false;
                Debug.Log("no triple bolts");
            }
            else if (BoltSpawner.doubleBolts)
            {
                Debug.Log("no double bolts");

                BoltSpawner.doubleBolts = false;
            }


            GameObject[] bolts = GameObject.FindGameObjectsWithTag("Bolt");
            foreach (GameObject bolt in bolts)
            {
                if (bolt.GetComponent<BoltMovement>().conveyorSpeed - 0.01f > speedReductionAmount)
                {
                    bolt.GetComponent<BoltMovement>().conveyorSpeed -= speedReductionAmount;

                }
            }

            if (BoltSpawner.currentConveyorSpeed - 0.01f > speedReductionAmount)
            {
                BoltSpawner.currentConveyorSpeed -= speedReductionAmount;

            }

            Debug.Log("to new Speed: " + BoltSpawner.currentConveyorSpeed);

        }
    }

    private static void increaseConveyorSpeed()
    {
        if (adaptive)
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

            Debug.Log("Current Speed: " + BoltSpawner.currentConveyorSpeed);

        }

    }
}