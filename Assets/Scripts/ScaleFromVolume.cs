using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ScaleFromVolume : MonoBehaviour
{
    [SerializeField] private MicrophoneBarkDetector _mic;
    [SerializeField] private Transform _minToBark;
    [SerializeField] private Transform _volumeIndicator;

    [SerializeField] private Transform _areaEffect;

    [SerializeField] private float _minScale;
    [SerializeField] private float _maxScale;
    [SerializeField] private float _minScaleBarkVisual;
    [SerializeField] private float _maxScaleBarkVisual;

    bool _tweenControlled;

    private void Awake()
    {
        _areaEffect.gameObject.SetActive(false);
        _areaEffect.localScale = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (_tweenControlled) return;
        _minToBark.localScale = Vector3.one * Remap(_mic.MinBarkDB);
        _volumeIndicator.localScale = Vector3.one * Remap(_mic.DBValue);
    }

    private float Remap(float dB)
    {
        var dbInvertedNormalized = Mathf.InverseLerp(MicrophoneBarkDetector.MinDB, MicrophoneBarkDetector.MaxDB, dB);
        return Mathf.Lerp(_minScale, _maxScale, dbInvertedNormalized);
    }

    public void OnBarkRecovery()
    {
        Debug.Log("Bark recovery");

        DOTween.Kill(this);

        DOTween.Sequence()
            .AppendCallback(() =>
            {
                _areaEffect.gameObject.SetActive(false);

                _minToBark.localScale = Vector3.zero;
                _minToBark.gameObject.SetActive(true);

                _volumeIndicator.gameObject.SetActive(true);
                _volumeIndicator.localScale = Vector3.zero;

                _minToBark.localScale = Vector3.one * Remap(_mic.MinBarkDB);
                _volumeIndicator.localScale = Vector3.one * Remap(_mic.DBValue);

            })
            .Append(_minToBark.DOScale(Vector3.one * Remap(_mic.MinBarkDB), 0.2f))
            .Join(_volumeIndicator.DOScale(Vector3.one * Remap(_mic.DBValue), 0.2f))
            .AppendCallback(() => 
            {
                _tweenControlled = false;
            })
            .SetId(this);
    }

    public void OnWtf(float barkStrength)
    {
        DOTween.Kill(this);
        _tweenControlled = true;

        DOTween.Sequence()
            .AppendCallback(()=>
            {
                _areaEffect.gameObject.SetActive(true);
                _minToBark.gameObject.SetActive(false);
                _volumeIndicator.gameObject.SetActive(false);
            })
            .Append(_areaEffect.DOScale(Vector3.one * Mathf.Lerp(_minScaleBarkVisual, _maxScaleBarkVisual, barkStrength), 0.2f))
            .Append(_areaEffect.DOScale(Vector3.zero, 0.4f))
            .SetId(this);
    }
}
