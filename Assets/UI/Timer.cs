using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour {
    public TextMeshProUGUI text;
    public float timeInSeconds;
    public event Action TimesUp;
    public void Start() {

    }
    private void Update() {
        timeInSeconds -= Time.deltaTime;
        if (timeInSeconds <= 0) {
            TimesUp?.Invoke();
        }

    }

}
