using System.Collections;
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
    private void Awake() {
        bark.action.Enable();
        barkVisual = GetComponentInChildren<BarkVisual>(); ;
        bark.action.performed += (x) => Bark(1);
        rb = GetComponent<Rigidbody>();
    }
     
    private void FixedUpdate() {
        var v = move.action.ReadValue<Vector2>();
        Vector3 forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up);
        var screenSpaceDir = forward * v.y + Camera.main.transform.right * v.x;
        rb.MovePosition(rb.position + screenSpaceDir * walkSpeed * Time.deltaTime);
    }

    public void Bark(float size) {
        barkVisual.Bark();

        float distance = size * 5;
        foreach (var a in sheeps)
            if ((a.transform.position - transform.position).magnitude < distance)
                Debug.Log("BARK"); //BARK AT IT

    }
}
