using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour {
    public TextMeshProUGUI text;
    public float timeInSeconds;
    public event Action TimesUp;

    private void Update() {
        if (timeInSeconds <= 0) return;
        timeInSeconds -= Time.deltaTime;
        TimeSpan timeSpan = new TimeSpan(0, 0, (int)timeInSeconds);
        text.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);
        if (timeInSeconds <= 0) {
            TimesUp?.Invoke();
        }

    }

}
