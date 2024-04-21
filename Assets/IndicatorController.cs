using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorController : MonoBehaviour {
    MeshRenderer rend;
    public float mltp;
    private void Awake() {
        rend = GetComponent<MeshRenderer>();
    }
    public void SetDB(float v) {
        rend.materials[0].SetFloat("_displacementScale", (-v * mltp));

    }
}
