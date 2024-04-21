using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneBarkCalibrator : MonoBehaviour
{
    [Header("Auto calibrate")]
    [SerializeField] private float _calibrationTime = 0.2f;
    [SerializeField] private float _loudnessThresholdDb = 20f;
    [SerializeField] private float _maxAllowedSilenceDb = -30f;
    [SerializeField] private float _downLerpStrength = 10f;

    public static bool HasCalibration;
    public static float NoBarkDb = -30f;
    public static float MinBarkDb = -20f;
    public static float MaxBarkDb = 0f;

    public UnityEvent<float> OnVolumeChanged;
    public UnityEvent<float> OnMinTargetChanged;
    public UnityEvent<float> OnSilenceTargetChanged;

    public UnityEvent OnCalibrationSilenceStarted;
    public UnityEvent OnCalibrationBarkStarted;
    public UnityEvent OnCalibrationBarkEnded;
    public UnityEvent OnCalibrationDone;
    public UnityEvent OnCalibrationFailed;

    public UnityEvent<float> OnCountDownUpdated;
    public UnityEvent<string> OnCountDownTextUpdated;


    public float CurrentVolumeDb 
    { 
        get => currentVolumeDb;
        internal set 
        { 
            currentVolumeDb = value;
            var normalized = RangeOfVolume(value);
            OnVolumeChanged?.Invoke(normalized);
        }
    }
    private float currentVolumeDb;

    private AudioSource _audioSource;
    private float[] _samples = new float[1024];

    private Coroutine _corroutine;
    public const float MinDB = -160f;
    public const float MaxDB = 20f;

    public bool HasMicrophone { get; private set; }
    public bool IsWaitingForCalibration { get; private set; }


    private void Awake()
    {
        HasMicrophone = Microphone.devices.Length > 0;
        Debug.Log($"HAS MIC: {HasMicrophone}");
    }

    private float RangeOfVolume(float db)
    {
        return Mathf.InverseLerp(MinDB, MaxDB, db);
    }

    private IEnumerator Start()
    {
        HasCalibration = false;

        OnSilenceTargetChanged?.Invoke(RangeOfVolume(_maxAllowedSilenceDb));
        OnCalibrationSilenceStarted?.Invoke();

        if (!HasMicrophone)
        {
            Debug.Log("No Mic!");
            yield break;
        }
        _audioSource = GetComponent<AudioSource>();

        _audioSource.loop = true;
        var clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);

        while(Microphone.GetPosition(null) <= 0)
        {
            yield return null;
        }

        _audioSource.clip = clip;

        _audioSource.Play();

        IsWaitingForCalibration = true;

        var t = 0f;

        while (IsWaitingForCalibration)
        {
            t = 0f;
            var db = MinDB;
            var currentDb = MinDB;
            var silenceCandidate = MinDB;
            Debug.Log($"Please be quiet, calibrating base loudness...");
            while (t < _calibrationTime)
            {
                t += Time.deltaTime;
                var remainingt = _calibrationTime - t;

                OnSilenceTargetChanged?.Invoke(RangeOfVolume(_maxAllowedSilenceDb));
                OnCountDownUpdated?.Invoke(remainingt);
                OnCountDownTextUpdated?.Invoke($"{remainingt:F0}");

                _audioSource.GetOutputData(_samples, 0);

                var sumOfSquares = 0f;
                for (var i = 0; i < _samples.Length; i++)
                {
                    sumOfSquares += _samples[i] * _samples[i];
                }

                var rmsValue = Mathf.Sqrt(sumOfSquares / _samples.Length);
                currentDb = 20f * Mathf.Log10(rmsValue);

                if (currentDb + _loudnessThresholdDb < MaxDB && currentDb > db)
                {
                    db = currentDb;
                }
                else db = Mathf.Lerp(db, currentDb, Time.deltaTime * _downLerpStrength);

                CurrentVolumeDb = db;

                yield return null;
            }


            NoBarkDb = db;

            if(NoBarkDb > _maxAllowedSilenceDb)
            {
                OnCalibrationFailed?.Invoke();
                continue;
            }

            OnMinTargetChanged?.Invoke(RangeOfVolume(NoBarkDb + _loudnessThresholdDb));
            OnCalibrationBarkStarted?.Invoke();

            Debug.Log($"Base DB found: {silenceCandidate}");

            t = 0f;
            var reachedMinThreshold = false;
            var lastVolume = MinDB;


            Debug.Log($"Bark very loud...");
            while (t < _calibrationTime && !reachedMinThreshold)
            {
                t += Time.deltaTime;
                var remainingt = _calibrationTime - t;
                OnCountDownUpdated?.Invoke(remainingt);
                OnCountDownTextUpdated?.Invoke($"{remainingt:F0}");

                _audioSource.GetOutputData(_samples, 0);

                var sumOfSquares = 0f;
                for (var i = 0; i < _samples.Length; i++)
                {
                    sumOfSquares += _samples[i] * _samples[i];
                }

                var rmsValue = Mathf.Sqrt(sumOfSquares / _samples.Length);
                currentDb = 20f * Mathf.Log10(rmsValue);

                CurrentVolumeDb = currentDb;

                if (currentDb < lastVolume && reachedMinThreshold)
                {
                    break;
                }

                if (currentDb > NoBarkDb + _loudnessThresholdDb)
                {
                    reachedMinThreshold = true;
                }

                lastVolume = currentDb;

                yield return null;
            }


            if (lastVolume > NoBarkDb && reachedMinThreshold)
            {
                IsWaitingForCalibration = false;
                MaxBarkDb = lastVolume;
                MinBarkDb = NoBarkDb + 0.3f * (MaxBarkDb - NoBarkDb);
            }
            else
            {
                Debug.Log("Those values don't really work, try to be quiet first and loud when requested");
                OnCalibrationFailed?.Invoke();
            }
        }

        Debug.Log($"Loud DB found: yell between {MinBarkDb} and {MaxBarkDb}, calibration done");
        OnCalibrationBarkEnded?.Invoke();

        HasCalibration = true;

        IsWaitingForCalibration = false;
        t = 0f;
        while (t < _calibrationTime * 0.5f)
        {
            t += Time.deltaTime;

            var remainingt = (_calibrationTime * 0.5f) - t;

            OnSilenceTargetChanged?.Invoke(RangeOfVolume(_maxAllowedSilenceDb));
            OnCountDownUpdated?.Invoke(remainingt);
            OnCountDownTextUpdated?.Invoke($"{remainingt:F0}");

            yield return null;
        }

        OnCalibrationDone?.Invoke();
    }

    private void OnDisable()
    {
        if (!HasMicrophone) return;

        _audioSource.Stop();
        _audioSource.clip = null;

        Microphone.End(null);
    }

    private void OnDestroy()
    {
        if (_corroutine != null) StopCoroutine(_corroutine);
        _corroutine = null;
    }
}


