using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
    public Timer t;
    public List<MarkerController> markers;
    public GameObject EndScreen;

    public void Start() {
        t.TimesUp += EndGame;
        markers = new(GameObject.FindObjectsOfType<MarkerController>());
        foreach (var a in markers) {
            a.OnCompleted += () => Completed(a);
        }
    }

    private void Completed(MarkerController a) {
        markers.Remove(a);
        if (markers.Count <= 0) {
            EndGame();
        }
    }

    private void EndGame() {
        EndScreen.SetActive(true);
    }
}
