using Fusion;
using UnityEngine;
using System.Collections.Generic;
using TMPro;


public class BoltSpawnerNetwork : NetworkBehaviour {
    public Transform spawnPoint;
    public NetworkPrefabRef boltPrefabRef;
    public float spawnDelay = 2f;
    public List<Material> possibleColors;

    [Networked] public float currentConveyorSpeed { get; set; }
    [Networked] public NetworkBool gameOn { get; set; }
    [Networked] public NetworkBool spawning { get; set; }

    [Networked] public NetworkBool doubleBolts { get; set; }
    [Networked] public NetworkBool tripleBolts { get; set; }
    [Networked] public float silverBoltsCount { get; set; }
    [Networked] public float errorsMadeByUser { get; set; }
    [Networked] public float silverBoltsPassed { get; set; }
    [Networked] public float score { get; set; }

    [Networked] public float totalTime { get; set; }

    public GameObject scoreText;



    private TickTimer spawnTimer;

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_incrementErrors() {
        // if (Object.HasStateAuthority) {
        Debug.LogWarning("incrementing error");

            errorsMadeByUser += 1.0f;
       // }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_incrementSilverBolts() {
        // if (Object.HasStateAuthority) {
        Debug.LogWarning("incrementing silver");

        silverBoltsPassed += 1.0f;
      //  }
      
    }

    public override void Spawned() {
        spawning = true;
        if (HasStateAuthority) {
            spawnTimer = TickTimer.CreateFromSeconds(Runner, spawnDelay);
        }
    }

    public override void FixedUpdateNetwork() {
        if (HasStateAuthority && gameOn) {
            totalTime += Runner.DeltaTime;
            
            if (totalTime > 85f) {
                RPC_StartGame(GameMode.Stop);
                scoreText.SetActive(true);

            } else if (totalTime > 60f) {
                RPC_StartGame(GameMode.Hard);

            } else if (totalTime > 57f) {
                RPC_StartGame(GameMode.Stop);
            } else if (totalTime > 32f) {
                RPC_StartGame(GameMode.Medium);
            } else if (totalTime > 25f) {
                RPC_StartGame(GameMode.Stop);
            }


            //RPC_StartGame(GameMode.Hard);

            if (spawnTimer.Expired(Runner)) {
                SpawnBolt();
                spawnTimer = TickTimer.CreateFromSeconds(Runner, spawnDelay);
            }
        }
        //   Debug.Log("silver bolts passed: "+silverBoltsPassed);
        //  Debug.Log("errors made by user: " + errorsMadeByUser);
        //  Debug.Log("silver bolts count: " + silverBoltsCount);

      //  scoreText.SetActive(true);
        if (silverBoltsCount > 0) {
            score = Mathf.Clamp(((silverBoltsPassed - errorsMadeByUser) / silverBoltsCount) * 100.0f, 0f, 100f);
        } else {
            score = 0f;
        }

        

    }

    public void Update() {
        scoreText.GetComponent<TextMeshProUGUI>().text = $"Score: {score:F2}%";

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_StartGame(GameMode mode) {
        gameOn = true;
        switch (mode) {
            case GameMode.Stop:
                spawning = false;
                break;
            case GameMode.Easy:
                spawning = true;
                currentConveyorSpeed = 0.6f;
                doubleBolts = false;
                tripleBolts = false;
                break;
            case GameMode.Medium:
                spawning = true;
                currentConveyorSpeed = 1.0f;
                doubleBolts = true;
                tripleBolts = false;
                break;
            case GameMode.Hard:
                spawning = true;
                currentConveyorSpeed = 1.4f;
                doubleBolts = true;
                tripleBolts = true;
                break;
            case GameMode.VeryHard:
                spawning = true;
                currentConveyorSpeed = 1.7f;
                doubleBolts = true;
                tripleBolts = true;
                break;
        }
    }

    void SpawnBolt() {
        if (spawning) {

        
        Vector3 position1 = new Vector3(spawnPoint.position.x, spawnPoint.position.y, -0.75f);
        Vector3 position2 = new Vector3(-6.6f, spawnPoint.position.y, -1.2f);
        Vector3 position3 = new Vector3(-6.9f, spawnPoint.position.y, -0.75f);
        Vector3 position4 = new Vector3(-7.5f, spawnPoint.position.y, -1.2f);


        SpawnBoltAtPosition(position1);
        SpawnBoltAtPosition(position2);
        // if (doubleBolts) SpawnBoltAtPosition(position2);
        if (tripleBolts) {
            SpawnBoltAtPosition(position3);
            SpawnBoltAtPosition(position4);
        }
        }
    }

    void SpawnBoltAtPosition(Vector3 position) {
        NetworkObject boltObject = Runner.Spawn(boltPrefabRef, position, spawnPoint.rotation,Runner.LocalPlayer);
        if (boltObject != null) {
            BoltProperties boltProps = boltObject.GetComponent<BoltProperties>();
            if (boltProps != null) {
                bool isGold = Random.value < 0.35f;
                boltProps.Initialize(isGold, currentConveyorSpeed);
                boltProps.SetBoltSpawner(this);
                if (!isGold) {
                    silverBoltsCount += 1;
                }
            }
        }
    }
}

public enum GameMode {
    Easy,
    Medium,
    Hard,
    VeryHard,
    Stop
}