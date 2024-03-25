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

    void Start()
    {
        InvokeRepeating("SpawnBolt", 0f, spawnDelay);
    }

    void SpawnBolt()
    {
        GameObject newBolt = Instantiate(boltPrefab, spawnPoint.position, spawnPoint.rotation);

        // Assign a random color
        int randomIndex = Random.Range(0, possibleColors.Count);
        //newBolt.GetComponent<MeshRenderer>().material = possibleColors[randomIndex];
    }
}
