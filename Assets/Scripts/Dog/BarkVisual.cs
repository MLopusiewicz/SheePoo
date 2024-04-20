using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class BarkVisual : MonoBehaviour {
    public Animator anim;
    public float barkValue;
    public float barkAnim;
    public Transform t;
    public void Bark(float v) {
        barkValue = v;
        Debug.Log("Barking");
        anim.Play("Bark");
    }
    private void Update() {
        t.localScale = Vector3.one * barkValue * barkAnim;

    }
}
