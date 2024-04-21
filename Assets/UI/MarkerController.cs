using Hobbitowo.PoolScoring;
using System;
using TMPro;
using UnityEngine;

public class MarkerController : MonoBehaviour {
    public TextMeshProUGUI text;
    public int requirement;
    public int score = 0;
    public Animator anim;
    TableHole hole;
    public event Action OnCompleted;

    private void Awake() {
        hole = GetComponentInParent<TableHole>();
        hole.OnScored += AddScore;
        text.text = $"{score}/{requirement}";
    }

    public void AddScore() {
        score++;
        text.text = $"{score}/{requirement}";
        if (score >= requirement) {
            anim.Play("Completed");
            OnCompleted?.Invoke();
        }
    }


}
