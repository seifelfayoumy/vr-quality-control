using Fusion;
using UnityEngine;
using System.Collections.Generic;

public class BoltSpawnerNetwork : NetworkBehaviour {
    public Transform spawnPoint;
    public NetworkPrefabRef boltPrefabRef;
    public float spawnDelay = 2f;
    public List<Material> possibleColors;

    [Networked] public float currentConveyorSpeed { get; set; }
    [Networked] public NetworkBool gameOn { get; set; }
    [Networked] public NetworkBool doubleBolts { get; set; }
    [Networked] public NetworkBool tripleBolts { get; set; }
    [Networked] public float silverBoltsCount { get; set; }
    [Networked] public float totalTime { get; set; }

    private TickTimer spawnTimer;

    public override void Spawned() {
        if (HasStateAuthority) {
            spawnTimer = TickTimer.CreateFromSeconds(Runner, spawnDelay);
        }
    }

    public override void FixedUpdateNetwork() {
        if (HasStateAuthority && gameOn) {
            totalTime += Runner.DeltaTime;

            if (spawnTimer.Expired(Runner)) {
                SpawnBolt();
                spawnTimer = TickTimer.CreateFromSeconds(Runner, spawnDelay);
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RPC_StartGame(GameMode mode) {
        gameOn = true;
        switch (mode) {
            case GameMode.Easy:
                currentConveyorSpeed = 0.6f;
                doubleBolts = false;
                tripleBolts = false;
                break;
            case GameMode.Medium:
                currentConveyorSpeed = 0.8f;
                doubleBolts = true;
                tripleBolts = false;
                break;
            case GameMode.Hard:
                currentConveyorSpeed = 1.4f;
                doubleBolts = true;
                tripleBolts = true;
                break;
            case GameMode.VeryHard:
                currentConveyorSpeed = 1.7f;
                doubleBolts = true;
                tripleBolts = true;
                break;
        }
    }

    void SpawnBolt() {
        Vector3 position1 = new Vector3(spawnPoint.position.x, spawnPoint.position.y, -0.75f);
        Vector3 position2 = new Vector3(-6.6f, spawnPoint.position.y, -1.2f);
        Vector3 position3 = new Vector3(-6.9f, spawnPoint.position.y, -0.75f);

        SpawnBoltAtPosition(position1);
        if (doubleBolts) SpawnBoltAtPosition(position2);
        if (tripleBolts) SpawnBoltAtPosition(position3);
    }

    void SpawnBoltAtPosition(Vector3 position) {
        NetworkObject boltObject = Runner.Spawn(boltPrefabRef, position, spawnPoint.rotation, Object.InputAuthority);
        if (boltObject != null) {
            BoltProperties boltProps = boltObject.GetComponent<BoltProperties>();
            if (boltProps != null) {
                bool isGold = Random.value < 0.35f;
                boltProps.Initialize(isGold, currentConveyorSpeed);
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
    VeryHard
}