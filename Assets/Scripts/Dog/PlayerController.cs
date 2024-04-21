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

    public float R;
    public float deadStickTreshhold = 0.03f;
    public float drag = 0.03f;

    public float repelImpulse = 1f;
    public AudioContainer bounceContainer;
    public CinemachineImpulseSource bounceImpulse;
    Vector2 lastInput;

    private void Awake() {
        bark.action.Enable(); 
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Sheep")) {
            rb.AddForce(collision.contacts[0].normal * repelImpulse, ForceMode.Impulse);
            collision.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            AudioInstancesManager.Instance.Play(bounceContainer, transform);
            bounceImpulse.GenerateImpulse();
        }
    }

    private void FixedUpdate() {
        var v = move.action.ReadValue<Vector2>();
        if (v.magnitude < deadStickTreshhold) {
            v = Vector2.MoveTowards(lastInput, v, drag);
        }
        Vector3 forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        var screenSpaceDir = forward * v.y + Camera.main.transform.right * v.x;
        Vector3 lastPos = rb.position;
        Vector3 pos = rb.position + screenSpaceDir * Time.deltaTime * walkSpeed;
        rb.MovePosition(pos);
        var dir = pos - lastPos;
        rb.rotation *= Quaternion.AngleAxis(dir.magnitude / R * Mathf.Rad2Deg * Random.Range(0.5f, 1), transform.InverseTransformVector(Vector3.Cross(-dir, Vector3.up)));
        lastInput = v;
    }
     
}
