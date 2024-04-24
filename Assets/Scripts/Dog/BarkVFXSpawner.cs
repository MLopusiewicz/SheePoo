using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BarkVFXSpawner : MonoBehaviour {

    public GameObject prefab;
    public float offset = 0.5f;
    public BarkController controller;
    public void Bark(float f) {
        //Debug.Log("Barked at: " + f);
        var go = GameObject.Instantiate(prefab);
        go.transform.localScale = Vector3.one * controller.BarkMaxArea * f;
        go.transform.position = transform.position - Vector3.up * offset;
        go.transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up), Vector3.up);
        StartCoroutine(DelayedDestroy(go));
    }
    IEnumerator DelayedDestroy(GameObject go) {
        yield return new WaitForSeconds(2f);
        Destroy(go);
    }
}
