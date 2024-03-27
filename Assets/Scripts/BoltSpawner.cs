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
    void Start()
    {
        
        originalConveyorSpeed = 1.0f; // Initialize
        currentConveyorSpeed = originalConveyorSpeed;
        
        InvokeRepeating("SpawnBolt", 0f, spawnDelay);
        
        
        
    }

    void SpawnBolt()
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
