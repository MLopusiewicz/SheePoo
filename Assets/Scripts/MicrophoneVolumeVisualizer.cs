using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicrophoneVolumeVisualizer : MonoBehaviour
{
    [SerializeField] private BarkController _barkController;
    [SerializeField] private float _multipier = 1f;

    void LateUpdate()
    {
        transform.localScale = Vector3.one * _barkController.MicrophoneVolume * _multipier;
    }
}
