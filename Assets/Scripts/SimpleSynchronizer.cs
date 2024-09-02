using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimpleSynchronizer : NetworkBehaviour {
    [SerializeField] private string gameSceneName = "GameScene";

    public static SimpleSynchronizer Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    public void StartGame() {
        if (Object.HasStateAuthority) {
            Runner.LoadScene(gameSceneName);
        }
    }

    public override void Spawned() {
        if (Object.HasStateAuthority) {
            foreach (var networkObject in FindObjectsOfType<NetworkObject>()) {
                if (networkObject.gameObject.scene == SceneManager.GetActiveScene() && !networkObject.Runner) {
                    Runner.Spawn(networkObject);
                }
            }
        }
    }
}