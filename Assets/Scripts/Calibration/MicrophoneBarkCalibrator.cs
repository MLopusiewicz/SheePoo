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

    public static bool HasCalibration;
    public static float NoBarkDb = -30f;
    public static float MinBarkDb = -20f;
    public static float MaxBarkDb = 0f;

    public UnityEvent<float> OnVolumeChanged;
    public UnityEvent<float> OnMinCandidateChanged;

    public UnityEvent<bool> OnMinCandidate;
    public UnityEvent<bool> OnCalibrated;
    public UnityEvent<bool> OnNotCalibrated;
    public UnityEvent<bool> OnNoMinCandidate;
    public UnityEvent<float> OnMinTargetChanged;

    public UnityEvent OnCalibrationDone;

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
    }

    private float RangeOfVolume(float db)
    {
        return Mathf.InverseLerp(MinDB, MaxDB, db);
    }

    private IEnumerator Start()
    {
        HasCalibration = false;

        OnMinCandidate?.Invoke(false);
        OnNoMinCandidate?.Invoke(true);
        OnNotCalibrated?.Invoke(false);
        OnCalibrated?.Invoke(false);

        if (!HasMicrophone)
        {
            Debug.Log("No Mic!");
            yield break;
        }
        _audioSource = GetComponent<AudioSource>();

        _audioSource.loop = true;
        var clip = Microphone.Start(null, true, 1, AudioSettings.outputSampleRate);

        _audioSource.clip = clip;

        _audioSource.Play();

        IsWaitingForCalibration = true;


        while (IsWaitingForCalibration)
        {
            var t = 0f;
            var peak = MinDB;
            Debug.Log($"Please be quiet, calibrating base loudness...");
            while (t < _calibrationTime)
            {
                t += Time.deltaTime;

                _audioSource.GetOutputData(_samples, 0);

                var sumOfSquares = 0f;
                for (var i = 0; i < _samples.Length; i++)
                {
                    sumOfSquares += _samples[i] * _samples[i];
                }

                var rmsValue = Mathf.Sqrt(sumOfSquares / _samples.Length);
                var db = 20f * Mathf.Log10(rmsValue);

                peak = Mathf.Max(peak, db);

                if (peak + _loudnessThresholdDb >= MaxDB)
                {
                    t = 0f;
                    peak = MinDB;
                }

                CurrentVolumeDb = db;

                yield return null;
            }


            NoBarkDb = peak;
            OnMinCandidateChanged?.Invoke(RangeOfVolume(NoBarkDb));
            OnMinCandidate?.Invoke(true);
            OnNoMinCandidate?.Invoke(false);
            OnMinTargetChanged?.Invoke(RangeOfVolume(NoBarkDb + _loudnessThresholdDb));
            Debug.Log($"Base DB found: {peak}");

            t = 0f;
            var reachedMinThreshold = false;
            var lastVolume = MinDB;


            Debug.Log($"Bark very loud...");
            while (t < _calibrationTime && !reachedMinThreshold)
            {
                t += Time.deltaTime;

                _audioSource.GetOutputData(_samples, 0);

                var sumOfSquares = 0f;
                for (var i = 0; i < _samples.Length; i++)
                {
                    sumOfSquares += _samples[i] * _samples[i];
                }

                var rmsValue = Mathf.Sqrt(sumOfSquares / _samples.Length);
                var db = 20f * Mathf.Log10(rmsValue);

                CurrentVolumeDb = db;

                if (db < lastVolume && reachedMinThreshold)
                {
                    break;
                }

                if (db > NoBarkDb + _loudnessThresholdDb)
                {
                    reachedMinThreshold = true;
                }

                lastVolume = db;

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
                OnMinCandidateChanged?.Invoke(RangeOfVolume(MinDB));
                OnMinCandidate?.Invoke(false);
                OnNoMinCandidate?.Invoke(true);
                OnNotCalibrated?.Invoke(false);
                OnCalibrated?.Invoke(false);
            }
        }

        Debug.Log($"Loud DB found: yell between {MinBarkDb} and {MaxBarkDb}, calibration done");
        OnCalibrated?.Invoke(true);

        HasCalibration = true;

        IsWaitingForCalibration = false;

        yield return new WaitForSeconds(_calibrationTime);
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


