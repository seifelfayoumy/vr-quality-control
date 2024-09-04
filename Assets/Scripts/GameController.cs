using Fusion;
using UnityEngine;

public class GameController : NetworkBehaviour {
    public GameObject uiGameObject;
    public BoltSpawnerNetwork boltSpawnerNetwork;

    [Networked]
    public NetworkBool IsGameStarted { get; set; }

    public void StartGame() {
        if (Object.HasStateAuthority) {
            RpcStartGame();
        }
    }

    

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RpcStartGame() {
        IsGameStarted = true;
        uiGameObject.SetActive(false);
        boltSpawnerNetwork.RPC_StartGame(GameMode.Easy);
    }

    public override void Spawned() {
        // Ensure the UI state is correct when a new player joins
        uiGameObject.SetActive(!IsGameStarted);
    }
}