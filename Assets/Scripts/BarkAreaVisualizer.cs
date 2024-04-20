using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkAreaVisualizer : MonoBehaviour
{
    [SerializeField] private BarkController _barkController;
    [SerializeField] private float _multipier = 2f;

    void Update()
    {
        transform.localScale = Vector3.one * _barkController.BarkMaxArea * _multipier;
    }
}
