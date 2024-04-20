using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BarkController : MonoBehaviour
{
    [SerializeField] private InputActionReference _barkInputAction;
    [SerializeField] private float _maxBarkPressTime = 1f;
    [SerializeField] private float _barkCooldownTime = 0.5f;
    [SerializeField] private BarkRepel _repel;
    [SerializeField] private MicrophoneBarkDetector _mic;

    private float _elapsedTime;

    public UnityEvent<float> MicrophoneVolumeUpdate;
    public UnityEvent<float> OnKeyboardBarkPressProgress;

    public UnityEvent<float> OnBarkEvent;
    public UnityEvent OnBarkRecoveryEvent;
    public UnityEvent<float> CooldownUpdate;

    // Progress bar of the keyboard input
    public float BarkPressTimeNormalized { get; private set; }

    // Cooldown of the bark
    public float CooldownTimer => _cooldownTimer;

    // Max area of influence of the bark
    public float BarkMaxArea => _repel.BarkMaxArea;

    public float MicrophoneVolume => _mic.DBValue;

    public event Action<float> OnBark;
    public event Action OnBarkRecovery;

    private float _cooldownTimer = 0f;


    private void OnEnable()
    {
        _barkInputAction.action.started += OnBarkStart;
        _barkInputAction.action.canceled += OnBarkEnd;
    }

    private void OnDisable()
    {
        _barkInputAction.action.started -= OnBarkStart;
        _barkInputAction.action.canceled -= OnBarkEnd;
    }

    private void Update()
    {
        if(_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
        }
        if (_cooldownTimer < 0f)
        {
            FireRecovery();
            _cooldownTimer = 0f;
        }

        if (_barkInputAction.action.IsInProgress())
        {
            _elapsedTime += Time.deltaTime;
            BarkPressTimeNormalized = Mathf.InverseLerp(0f, _maxBarkPressTime, _elapsedTime);
            OnKeyboardBarkPressProgress?.Invoke(BarkPressTimeNormalized);
        }
        else
        {
            BarkPressTimeNormalized = Mathf.Lerp(BarkPressTimeNormalized, 0f, Time.deltaTime * 10f);
            OnKeyboardBarkPressProgress?.Invoke(BarkPressTimeNormalized);
        }

        MicrophoneVolumeUpdate?.Invoke(_mic.DBValue);
        CooldownUpdate?.Invoke(_cooldownTimer);
    }

    private void FireRecovery()
    {
        OnBarkRecovery?.Invoke();
        OnBarkRecoveryEvent?.Invoke();
    }

    private void OnBarkStart(InputAction.CallbackContext context)
    {
        if (_cooldownTimer > 0f) return;
        _elapsedTime = 0f;
    }

    private void OnBarkEnd(InputAction.CallbackContext context)
    {
        if (_cooldownTimer > 0f) return;

        var barkStrength = Mathf.InverseLerp(0f, _maxBarkPressTime, (float)context.duration);
        
        Debug.Log($"Bark from keyboard at strength: {barkStrength}");
        FireBark(barkStrength);
    }

    public void ForceBark(float barkStrength)
    {
        if (_cooldownTimer > 0f) return;

        Debug.Log($"Bark from other input at strength: {barkStrength}");
        FireBark(barkStrength);
    }

    private void FireBark(float barkStrength)
    {
        OnBark?.Invoke(barkStrength);
        OnBarkEvent?.Invoke(barkStrength);

        _repel.DoBark(barkStrength);

        _cooldownTimer = _barkCooldownTime;
        _elapsedTime = 0f;
        BarkPressTimeNormalized = 0f;
    }
}
