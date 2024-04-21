using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {

    public InputActionReference move;
    public InputActionReference bark;

    Rigidbody rb;
    public float walkSpeed = 1;
    List<Transform> sheeps = new();

    BarkVisual barkVisual;
    public float force;
    public float R;
    public Vector2 lastInput;
    public float fff = 0.03f;
    public float drag = 0.03f;
    private void Awake() {
        bark.action.Enable();
        barkVisual = GetComponentInChildren<BarkVisual>();
        bark.action.performed += (x) => Bark(1);
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        var v = move.action.ReadValue<Vector2>();
        if (v.magnitude < fff) {
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

    public void Bark(float size) {
        barkVisual.Bark(1);

        float distance = size * 5;
        foreach (var a in sheeps)
            if ((a.transform.position - transform.position).magnitude < distance)
                Debug.Log("BARK"); //BARK AT IT

    }
}
