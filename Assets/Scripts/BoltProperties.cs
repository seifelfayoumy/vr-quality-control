using Fusion;
using UnityEngine;

public class BoltProperties : NetworkBehaviour {
    [Networked] public NetworkBool IsGold { get; set; }
    [Networked] public float ConveyorSpeed { get; set; }
    [Networked] public NetworkBool OnConveyor { get; set; }
    private BoltSpawnerNetwork boltSpawner;

    private MeshRenderer meshRenderer;

    public void SetBoltSpawner(BoltSpawnerNetwork spawner) {
        boltSpawner = spawner;
    }

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

    public void release() {
        RPC_ReleaseBolt();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_GrabBolt() {

        Debug.Log("Grabbed any bolt!");
        // Debug.Log(this.GetComponent<Fusion.XR.Shared.Grabbing.NetworkGrabbable>().IsGrabbed);

        // Debug.Log(this.GetComponent<NetworkHandColliderGrabbable>().);
        //GetComponent<NetworkObject>().AssignInputAuthority(Runner.LocalPlayer);
       // GetComponent<NetworkObject>().RequestStateAuthority();
        Debug.Log(Runner.LocalPlayer);
        Debug.Log(GetComponent<NetworkObject>().HasInputAuthority);
        Debug.Log(GetComponent<NetworkObject>().InputAuthority);
        Debug.Log(GetComponent<NetworkObject>().HasStateAuthority);
        Debug.Log(GetComponent<NetworkObject>().StateAuthority);




        if (!IsGold) {
            // Handle error logic here
            Debug.Log("Grabbed silver bolt - Error!");
            Debug.LogWarning("try incrementing error");

            boltSpawner.RPC_incrementErrors();
            // You might want to call a method on a manager object to handle the error
            // For example: GameManager.Instance.MadeError();
        }
        //Runner.Despawn(Object);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ReleaseBolt() {

        Debug.Log("released any bolt!");
        // Debug.Log(this.GetComponent<Fusion.XR.Shared.Grabbing.NetworkGrabbable>().IsGrabbed);

        // Debug.Log(this.GetComponent<NetworkHandColliderGrabbable>().);
        //<NetworkObject>().AssignInputAuthority(Runner.LocalPlayer);
        //GetComponent<NetworkObject>().ReleaseStateAuthoirty();
        OnConveyor = false;

    }

    public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Conveyor")) {
            OnConveyor = true;
        }

    }

    public void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.CompareTag("Wall")) {
            Debug.LogWarning("hitingggg");
            if (!IsGold) {
                Debug.LogWarning("try incrementing silver");

                boltSpawner.RPC_incrementSilverBolts();
            }
        }
    }

    public void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Conveyor")) {
            OnConveyor = false;
        }
    }

}