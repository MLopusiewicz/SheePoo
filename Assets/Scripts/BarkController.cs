using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BarkController : MonoBehaviour
{
    [SerializeField] private InputActionReference _barkInputAction;
    [SerializeField] private float _maxBarkPressTime = 1f;
    [SerializeField] private float _barkCooldownTime = 0.5f;

    private float _lastBarkTime;

    public event Action<float> OnBark;

    private void OnEnable()
    {
        _barkInputAction.action.canceled += OnBarkEnd;
    }

    private void OnDisable()
    {
        _barkInputAction.action.canceled -= OnBarkEnd;
    }

    private void OnBarkEnd(InputAction.CallbackContext context)
    {
        var barkStrength = Mathf.InverseLerp(0f, _maxBarkPressTime, (float)context.duration);
        
        Debug.Log($"Bark from keyboard at strength: {barkStrength}");
        OnBark?.Invoke(barkStrength);
    }

    public void ForceBark(float barkStrength)
    {
        if ((Time.time - _lastBarkTime) < _barkCooldownTime) return;

        Debug.Log($"Bark from other input at strength: {barkStrength}");
        OnBark?.Invoke(barkStrength);

        _lastBarkTime = Time.time;
    }
}
