using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoltSpawner : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject boltPrefab;
    public float spawnDelay = 2f;
    public List<Material> possibleColors; // Assign correct and incorrect materials in the Inspector

    public static float currentConveyorSpeed; 
    private float originalConveyorSpeed;

    public static Boolean gameOn = false;
    
    public static Boolean doubleBolts = true;
    public static Boolean tripleBolts = true;

    public static float silverBoltsCount = 0.0f;


    void Start()
    {
        
        originalConveyorSpeed = 1.0f; // Initialize
        currentConveyorSpeed = originalConveyorSpeed;
        
        InvokeRepeating("SpawnBolt", 0f, spawnDelay);
        
        
        
    }

    public static void startGameEasy()
    {
        gameOn = true;
        currentConveyorSpeed = 0.6f;
        doubleBolts = false;
        tripleBolts = false;
    }
    
    public static void startGameMedium()
    {
        gameOn = true;
        currentConveyorSpeed = 0.8f;
        doubleBolts = true;
        tripleBolts = false;
    }
    public static void startGameHard()
    {
        gameOn = true;
        currentConveyorSpeed = 1.0f;
        doubleBolts = true;
        tripleBolts = true;
    }
    
    

    void SpawnBolt()
    {
        if (gameOn)
        {

            Vector3 position1 = new Vector3(spawnPoint.position.x, spawnPoint.position.y, -0.75f);
            Vector3 position2 = new Vector3(-6.6f, spawnPoint.position.y, -1.2f);
            Vector3 position3 = new Vector3(-6.9f, spawnPoint.position.y, -0.75f);


            GameObject newBolt = Instantiate(boltPrefab, position1, spawnPoint.rotation);


            // Assign a color (with weighted gold chance)
            float goldChance = 0.2f; // 20% chance of gold
            bool isGold = Random.value < goldChance; 

            Material boltMaterial = newBolt.GetComponent<MeshRenderer>().material;
            if (isGold) {
                boltMaterial.color = new Color(1f, 0.84f, 0f); // Your gold color 
            } else { 
                boltMaterial.color = new Color(0.8f, 0.8f, 0.8f); // Example silver color
                silverBoltsCount += 1.0f;
            }
            
            newBolt.GetComponent<BoltMovement>().conveyorSpeed = currentConveyorSpeed;


            if (doubleBolts)
            {
                GameObject newBolt2 = Instantiate(boltPrefab, position2, spawnPoint.rotation);

                float goldChance2 = 0.25f; // 20% chance of gold

                bool isGold2 = Random.value < goldChance2; 

                Material boltMaterial2 = newBolt2.GetComponent<MeshRenderer>().material;
                if (isGold2) {
                    boltMaterial2.color = new Color(1f, 0.84f, 0f); // Your gold color 
                } else { 
                    boltMaterial2.color = new Color(0.8f, 0.8f, 0.8f); // Example silver color
                    silverBoltsCount += 1.0f;
                }
                
                newBolt2.GetComponent<BoltMovement>().conveyorSpeed = currentConveyorSpeed;

            }
            
            if (tripleBolts)
            {
                GameObject newBolt3 = Instantiate(boltPrefab, position3, spawnPoint.rotation);

                float goldChance3 = 0.3f; // 20% chance of gold

                bool isGold3 = Random.value < goldChance3; 

                Material boltMaterial2 = newBolt3.GetComponent<MeshRenderer>().material;
                if (isGold3) {
                    boltMaterial2.color = new Color(1f, 0.84f, 0f); // Your gold color 
                } else { 
                    boltMaterial2.color = new Color(0.8f, 0.8f, 0.8f); // Example silver color
                    silverBoltsCount += 1.0f;
                }
                
                newBolt3.GetComponent<BoltMovement>().conveyorSpeed = currentConveyorSpeed;

            }

        }
    }

}
