using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BarkVFXSpawner : MonoBehaviour {

    public GameObject prefab;

    public void Bark(float f) {
        var go = GameObject.Instantiate(prefab);
        go.transform.localScale = Vector3.one * f;
        go.transform.position = Vector3.ProjectOnPlane(transform.position, Vector3.up) + Vector3.up * 0.01f;

    }
}
