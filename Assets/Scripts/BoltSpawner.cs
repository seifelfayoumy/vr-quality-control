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

    public Boolean gameOn = false;
    void Start()
    {
        
        originalConveyorSpeed = 1.0f; // Initialize
        currentConveyorSpeed = originalConveyorSpeed;
        
        InvokeRepeating("SpawnBolt", 0f, spawnDelay);
        
        
        
    }

    public void startGameEasy()
    {
        gameOn = true;
        currentConveyorSpeed = 0.6f;
    }
    
    public void startGameMedium()
    {
        gameOn = true;
        currentConveyorSpeed = 0.8f;
    }
    public void startGameHard()
    {
        gameOn = true;
        currentConveyorSpeed = 1.0f;
    }
    
    

    void SpawnBolt()
    {
        if (gameOn)
        {
            
        
            GameObject newBolt = Instantiate(boltPrefab, spawnPoint.position, spawnPoint.rotation);

            // Assign a color (with weighted gold chance)
            float goldChance = 0.2f; // 20% chance of gold
            bool isGold = Random.value < goldChance; 

            Material boltMaterial = newBolt.GetComponent<MeshRenderer>().material;
            if (isGold) {
                boltMaterial.color = new Color(1f, 0.84f, 0f); // Your gold color 
            } else { 
                boltMaterial.color = new Color(0.8f, 0.8f, 0.8f); // Example silver color
            }
            
            newBolt.GetComponent<BoltMovement>().conveyorSpeed = currentConveyorSpeed;
            
        }
    }

}
