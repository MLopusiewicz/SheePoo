using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

public class BarkStrengthVisualizer : MonoBehaviour
{
    [SerializeField] private MicrophoneBarkDetector _microphoneBark;
    [SerializeField] private float _minScale;
    [SerializeField] private float _maxScale;

    [SerializeField] private Color _barkColor;
    [SerializeField] private Color _noBarkColor;
    [SerializeField] private Image _volumeColor;
    [SerializeField] private Slider _volume;
    [SerializeField] private Slider _minBark;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(_microphoneBark.HasMicrophone && !_microphoneBark.IsWaitingForCalibration)
        {
            var val = Mathf.InverseLerp(_microphoneBark.MinBarkDB, _microphoneBark.MaxBarkDB, _microphoneBark.DBValue);
            if (val > 0f) Handles.color = Color.cyan;
            else Handles.color = Color.white;
            Handles.DrawWireDisc(transform.position, Vector3.forward, Remap(_microphoneBark.DBValue));

            Handles.color = Color.red;
            Handles.DrawWireDisc(transform.position, Vector3.forward, Remap(_microphoneBark.NoBarkDB));
            Handles.color = Color.yellow;
            Handles.DrawWireDisc(transform.position, Vector3.forward, Remap(_microphoneBark.MinBarkDB));
            Handles.color = Color.green;
            Handles.DrawWireDisc(transform.position, Vector3.forward, Remap(_microphoneBark.MaxBarkDB));
        }
    }
#endif

    private void Update()
    {
        if (_microphoneBark.HasMicrophone && !_microphoneBark.IsWaitingForCalibration)
        {
            var dbInvertedNormalized = Mathf.InverseLerp(MicrophoneBarkDetector.MinDB, MicrophoneBarkDetector.MaxDB, _microphoneBark.DBValue);
            var minInvertedNormalized = Mathf.InverseLerp(MicrophoneBarkDetector.MinDB, MicrophoneBarkDetector.MaxDB, _microphoneBark.MinBarkDB);

            _volume.value = dbInvertedNormalized;
            _minBark.value = minInvertedNormalized;

            _volumeColor.color = dbInvertedNormalized > minInvertedNormalized ? _barkColor : _noBarkColor;
        }
    }

    private float Remap(float dB)
    {
        var dbInvertedNormalized = Mathf.InverseLerp(MicrophoneBarkDetector.MinDB, MicrophoneBarkDetector.MaxDB, dB);
        return Mathf.Lerp(_minScale, _maxScale, dbInvertedNormalized);
    }
}
