using Fusion;
using UnityEngine;

public class BoltProperties : NetworkBehaviour {
    [Networked] public NetworkBool IsGold { get; set; }
    [Networked] public float ConveyorSpeed { get; set; }
    [Networked] public NetworkBool OnConveyor { get; set; }
    [Networked] public NetworkBool IsGrabbed { get; set; }

    private MeshRenderer meshRenderer;
    private Rigidbody rb;

    public override void Spawned() {
        meshRenderer = GetComponent<MeshRenderer>();
        rb = GetComponent<Rigidbody>();
        UpdateVisuals();
    }

    public void Initialize(bool isGold, float conveyorSpeed) {
        IsGold = isGold;
        ConveyorSpeed = conveyorSpeed;
        UpdateVisuals();
    }

    public override void FixedUpdateNetwork() {
        if (OnConveyor && !IsGrabbed) {
            transform.Translate(Vector3.right * ConveyorSpeed * Runner.DeltaTime);
        }

        if (Object.HasStateAuthority) {
            // Sync physics properties
            rb.position = transform.position;
            rb.rotation = transform.rotation;
        }
    }

    public void grab() {
        RPC_GrabBolt(Runner.LocalPlayer);
    }
    public void release() {
        RPC_ReleaseBolt(new Vector3(0,0,0));
    }
    public override void Render() {
        UpdateVisuals();
    }

    private void UpdateVisuals() {
        if (meshRenderer != null) {
            meshRenderer.material.color = IsGold ? new Color(1f, 0.84f, 0f) : new Color(0.8f, 0.8f, 0.8f);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_GrabBolt(PlayerRef player) {
        if (Object.HasStateAuthority) {
            IsGrabbed = true;
            Object.AssignInputAuthority(player);
            rb.isKinematic = true;

            if (!IsGold) {
                Debug.Log("Grabbed silver bolt - Error!");
                // Handle error logic here
            }
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_ReleaseBolt(Vector3 releaseVelocity) {
        if (Object.HasStateAuthority) {
            IsGrabbed = false;
           // Object.InputAuthority = PlayerRef.None;
            rb.isKinematic = false;
            rb.velocity = releaseVelocity;
        }
    }

    public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Conveyor")) {
            OnConveyor = true;
        }
    }

    public void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Conveyor")) {
            OnConveyor = false;
        }
    }
}