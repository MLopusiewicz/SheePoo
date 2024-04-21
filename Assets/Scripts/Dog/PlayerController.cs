using System.Collections.Generic;
using HobbitAudio;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerController : MonoBehaviour {

    public InputActionReference move;
    public InputActionReference bark;

    Rigidbody rb;
    public float walkSpeed = 1;
    List<Transform> sheeps = new();

    BarkVisual barkVisual;
    public float force;
    public float R;

    public float repelImpulse = 1f;
    public AudioContainer bounceContainer;
    public CinemachineImpulseSource bounceImpulse;

    private void Awake() {
        bark.action.Enable();
        barkVisual = GetComponentInChildren<BarkVisual>();
        bark.action.performed += (x) => Bark(1);
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Sheep"))
        {
            rb.AddForce(collision.contacts[0].normal * repelImpulse, ForceMode.Impulse);
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            AudioInstancesManager.Instance.Play(bounceContainer, transform);
            bounceImpulse.GenerateImpulse();
        }
    }

    private void FixedUpdate() {
        var v = move.action.ReadValue<Vector2>();
        Vector3 forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        var screenSpaceDir = forward * v.y + Camera.main.transform.right * v.x;
        Vector3 lastPos = rb.position;
        Vector3 pos = rb.position + screenSpaceDir * Time.deltaTime * walkSpeed;
        rb.MovePosition(pos);
        var dir = pos - lastPos;
        rb.rotation *= Quaternion.AngleAxis(dir.magnitude / R * Mathf.Rad2Deg * Random.Range(0.5f, 1), transform.InverseTransformVector(Vector3.Cross(-dir, Vector3.up)));
    }

    public void Bark(float size) {
        barkVisual.Bark(1);

        float distance = size * 5;
        foreach (var a in sheeps)
            if ((a.transform.position - transform.position).magnitude < distance)
                Debug.Log("BARK"); //BARK AT IT

    }
}
