using System;
using UnityEngine;

public class BarkController : MonoBehaviour
{
    [SerializeField] private KeyCode _debugBarkKey;
    [SerializeField] private float _maxBarkPressTime = 1f;

    public event Action<float> OnBark;

    private float _barkPressTime;

    private void Update()
    {
        if (Input.GetKeyUp(_debugBarkKey))
        {
            var barkStrength = Mathf.InverseLerp(0f, _maxBarkPressTime, _barkPressTime);
            OnBark.Invoke(barkStrength);

            _barkPressTime = 0f;
        }

        if (Input.GetKey(_debugBarkKey))
        {
            _barkPressTime += Time.deltaTime;
        }
    }
}
