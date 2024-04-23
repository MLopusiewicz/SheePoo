using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour {
    public Timer t;
    public List<MarkerController> markers;
    public GameObject EndScreen;

    public Image win, lose;
    public Transform sheepContainer;
    bool gameEnded = false;
    float timer = 2f;
    public void Start() {
        t.TimesUp += () => EndGame(false);
        markers = new(GameObject.FindObjectsOfType<MarkerController>());
        foreach (var a in markers) {
            a.OnCompleted += () => Completed(a);
        }

    }
    private void Update() {
        timer -= Time.deltaTime;
        if (timer <= 0) {
            if (!HasAnyMoreSheeps())
                EndGame(false);
        }
    }

    private void Completed(MarkerController a) {
        markers.Remove(a);
        if (markers.Count <= 0) {
            EndGame(true);
        }
    }

    private void EndGame(bool isWin) {
        if (gameEnded) return;
        gameEnded = false;

        EndScreen.SetActive(true);
        win.gameObject.SetActive(isWin);
        lose.gameObject.SetActive(!isWin);

    }
    public void GoToMenu() {
        SceneManager.LoadScene(0);
    }

    public bool HasAnyMoreSheeps() {
        for (int i = 0; i < sheepContainer.childCount; i++)
            if (sheepContainer.GetChild(i).gameObject.activeInHierarchy)
                return true;

        return false;
    }
}
