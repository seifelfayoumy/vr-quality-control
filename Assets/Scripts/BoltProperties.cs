using Fusion;
using UnityEngine;

public class BoltProperties : NetworkBehaviour {
    [Networked] public NetworkBool IsGold { get; set; }
    [Networked] public float ConveyorSpeed { get; set; }
    [Networked] public NetworkBool OnConveyor { get; set; }

    private MeshRenderer meshRenderer;
    private NetworkRigidbody networkRigidbody;
    private Rigidbody rb;
    private Vector3 lastPosition;
    private float releaseVelocityMultiplier = 1f; // Adjust this value to control release velocity

    public override void Spawned() {
        meshRenderer = GetComponent<MeshRenderer>();
        networkRigidbody = GetComponent<NetworkRigidbody>();
        rb = GetComponent<Rigidbody>();
        UpdateVisuals();
    }

    public void Initialize(bool isGold, float conveyorSpeed) {
        IsGold = isGold;
        ConveyorSpeed = conveyorSpeed;
        UpdateVisuals();
    }

    public override void FixedUpdateNetwork() {
        if (OnConveyor) {
            rb.MovePosition(rb.position + Vector3.right * ConveyorSpeed * Runner.DeltaTime);
        }

        // Store the current position for velocity calculation
        lastPosition = transform.position;
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
        Debug.Log("Grabbed bolt");
        Object.RequestStateAuthority();
        if (!Object.HasStateAuthority) return;

        rb.isKinematic = true;
        networkRigidbody.GetComponent<Rigidbody>().isKinematic = true;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ReleaseBolt() {
        Debug.Log("Released bolt");
        if (!Object.HasStateAuthority) return;


        rb.isKinematic = false;
        networkRigidbody.GetComponent<Rigidbody>().isKinematic = false;

        // Calculate release velocity based on recent movement
        Vector3 releaseVelocity = (transform.position - lastPosition) / Runner.DeltaTime * releaseVelocityMultiplier;
        rb.velocity = releaseVelocity;
        networkRigidbody.GetComponent<Rigidbody>().velocity = releaseVelocity;

        Object.ReleaseStateAuthoirty();
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