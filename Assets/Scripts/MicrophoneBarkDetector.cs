using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneBarkDetector : MonoBehaviour
{
    [SerializeField] private float _barkWindowOfTime = 0.2f;
    [SerializeField] private float _downLerpStrength = 10f;

    [Header("Auto calibrate")]
    [SerializeField] private bool _autoCalibrate;
    [SerializeField] private float _calibrationTime = 0.2f;
    [SerializeField] private float _minBarkAdditionalDb = 20f;
    [SerializeField] private float _maxBarkAdditionalDb = 40f;

    [Header("Hardcoded calibration")]
    [SerializeField] private float _noBarkDb = -30f;
    [SerializeField] private float _minBarkDb = -20f;
    [SerializeField] private float _maxBarkDb = 0f;

    [Header("Bark Controller")]
    [SerializeField] private BarkController _barkController;

    private AudioSource _audioSource;
    private float[] _samples = new float[1024];

    private float _dbValue;
    private float _lastDb;
    private float _highestDb;

    private bool _hadSilenceBefore = true;

    private Coroutine _corroutine;

    private float _detectedMinDb;
    private float _minBarkDbInUse = -20f;
    private float _maxBarkDbInUse = 0f;
    private float _noBarkDbInUse = 0f;

    public const float MinDB = -160f;
    public const float MaxDB = 20f;

    public bool HasMicrophone { get; private set; }
    public bool IsWaitingForCalibration { get; private set; }

    public float MinBarkDB => _minBarkDbInUse;
    public float MaxBarkDB => _maxBarkDbInUse;
    public float DBValue => _dbValue;
    public float DBValueNormalizedToBarkRange => Mathf.InverseLerp(_minBarkDbInUse, _maxBarkDbInUse, _dbValue);

    public float NoBarkDB => _noBarkDb;

    private void Awake()
    {
        HasMicrophone = Microphone.devices.Length > 0;
    }

    private IEnumerator Start()
    {
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

        if (_autoCalibrate)
        {
            IsWaitingForCalibration = true;

            var t = 0f;
            var peak = MinDB;
            while(t < _calibrationTime)
            {
                Debug.Log($"Pos: {Microphone.GetPosition(null)}");

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

                yield return null;
            }

            Debug.Log($"Max DB found: {peak}");

            _detectedMinDb = peak;

            IsWaitingForCalibration = false;
        }
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

    private void Update()
    {
        if (!HasMicrophone || IsWaitingForCalibration) return;

        _audioSource.GetOutputData(_samples, 0);

        var sumOfSquares = 0f;
        for (var i = 0; i < _samples.Length; i++)
        {
            sumOfSquares += _samples[i] * _samples[i];
        }

        var rmsValue = Mathf.Sqrt(sumOfSquares / _samples.Length);
        var dbValue = 20f * Mathf.Log10(rmsValue);
        if (dbValue < MinDB) dbValue = MinDB;

        // dB as they show in the audio mixer

        _minBarkDbInUse = _minBarkDb;
        _maxBarkDbInUse = _maxBarkDb;
        _noBarkDbInUse = _noBarkDb;

        if (_autoCalibrate)
        {
            _noBarkDbInUse = _detectedMinDb;
            _minBarkDbInUse = _noBarkDbInUse + _minBarkAdditionalDb;
            _maxBarkDbInUse = _noBarkDbInUse + _maxBarkAdditionalDb;
        }

        var justCrossedThreshold = dbValue > _minBarkDbInUse && _lastDb <= _minBarkDbInUse;
        if (_hadSilenceBefore && justCrossedThreshold)
        {
            Debug.Log($"bark chance starting...: {dbValue}");

            _highestDb = _minBarkDbInUse;
            // actually give a window of time to see if it gets higher
            _hadSilenceBefore = false;
            _corroutine = StartCoroutine(WaitForPeak());
        }

        IEnumerator WaitForPeak()
        {
            var t = 0f;
            while(t < _barkWindowOfTime)
            {
                t += Time.deltaTime;
                _highestDb = Mathf.Max(_highestDb, _dbValue);
                yield return null;
            }

            if(_highestDb > _minBarkDbInUse) Bark();
            _corroutine = null;
        }

        if (_dbValue < _noBarkDbInUse)
        {
            if (_corroutine != null) StopCoroutine(_corroutine);
            _corroutine = null;

            if (!_hadSilenceBefore)
            {
                Bark();
            }

            _highestDb = MinDB;
            _hadSilenceBefore = true;
        }

        if (dbValue > _dbValue) _dbValue = dbValue;
        else _dbValue = Mathf.Lerp(_dbValue, dbValue, Time.deltaTime * _downLerpStrength);

        _lastDb = _dbValue;
    }

    private void Bark()
    {
        var barkStrength = Mathf.InverseLerp(_minBarkDbInUse, _maxBarkDbInUse, _highestDb);
        //Debug.Log($"--- Bark at: {_highestDb}, as 0-1: {barkStrength}");
        _barkController.ForceBark(barkStrength);
    }
}


