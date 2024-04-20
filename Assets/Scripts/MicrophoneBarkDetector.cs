using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneBarkDetector : MonoBehaviour
{
    [SerializeField] private float _barkWindowOfTime = 0.2f;

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

    private const float MinDB = -160f;

    public bool HasMicrophone { get; private set; }
    public bool IsWaitingForCalibration { get; private set; }

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
        var clip = Microphone.Start(null, true, 1, 44100);

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

            _noBarkDb = peak;
            _minBarkDb = _noBarkDb + _minBarkAdditionalDb;
            _maxBarkDb = _noBarkDb + _maxBarkAdditionalDb;

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
        _dbValue = 20f * Mathf.Log10(rmsValue);
        if (_dbValue < MinDB) _dbValue = MinDB;

        // dB as they show in the audio mixer

        var justCrossedThreshold = _dbValue > _minBarkDb && _lastDb <= _minBarkDb;
        if (_hadSilenceBefore && justCrossedThreshold)
        {
            //Debug.Log("bark chance starting...");

            _highestDb = _minBarkDb;
            // actually give a window of a few secs to see if it gets higher
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

            if(_highestDb > _minBarkDb) Bark();
            _corroutine = null;
        }

        if (_dbValue < _noBarkDb)
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

        _lastDb = _dbValue;
    }

    private void Bark()
    {
        var barkStrength = Mathf.InverseLerp(_minBarkDb, _maxBarkDb, _highestDb);
        //Debug.Log($"--- Bark at: {_highestDb}, as 0-1: {barkStrength}");
        _barkController.ForceBark(barkStrength);
    }
}


