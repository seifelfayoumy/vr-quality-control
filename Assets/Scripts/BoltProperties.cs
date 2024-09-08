using Fusion;
using UnityEngine;

public class BoltProperties : NetworkBehaviour {
    [Networked] public NetworkBool IsGold { get; set; }
    [Networked] public float ConveyorSpeed { get; set; }
    [Networked] public NetworkBool OnConveyor { get; set; }

    private MeshRenderer meshRenderer;

    public override void Spawned() {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateVisuals();
    }

    public void Initialize(bool isGold, float conveyorSpeed) {
        IsGold = isGold;
        ConveyorSpeed = conveyorSpeed;
        UpdateVisuals();
    }

    public override void FixedUpdateNetwork() {
        if (OnConveyor) {
            this.transform.Translate(Vector3.right * ConveyorSpeed * Runner.DeltaTime);
        }
    }

    public override void Render() {
        UpdateVisuals();
    }

    private void UpdateVisuals() {
        if (meshRenderer != null) {
            meshRenderer.material.color = IsGold ? new Color(1f, 0.84f, 0f) : new Color(0.8f, 0.8f, 0.8f);
        }
    }

    public void grab() {
        RPC_GrabBolt();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_GrabBolt() {

        NetworkObject networkObject = this.GetComponent<NetworkObject>();

        Debug.LogWarning(networkObject.HasInputAuthority);
        Debug.LogWarning(networkObject.InputAuthority);

        if (!IsGold) {
            // Handle error logic here
            Debug.Log("Grabbed silver bolt - Error!");
            // You might want to call a method on a manager object to handle the error
            // For example: GameManager.Instance.MadeError();
        }
        //Runner.Despawn(Object);
    }

    public  void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Conveyor")) {
            OnConveyor = true;
        }
    }

    public  void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Conveyor")) {
            OnConveyor = false;
        }
    }
}