using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Hobbitowo.PoolScoring
{
    public class TableManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text scoreText;

        [Header("Settings")]
        [SerializeField] private float timeLimit = 121f;
        [SerializeField] private int sheepsAmount;
        
        public int score;
        private List<TableHole> _holes;
        private List<AIController> _aiControllers;
        public Action OnScoreAdded;

        private void Start()
        {
            _holes = new List<TableHole>(GetComponentsInChildren<TableHole>());
            _aiControllers = new List<AIController>(GetComponentsInChildren<AIController>());
            var ratio = _holes.Count == 0? 0: _aiControllers.Count / _holes.Count; 
            _holes.ForEach((h) => h.InitializeTable(this, ratio));
            sheepsAmount = FindObjectsOfType<AIController>().Length;
            scoreText.text = $"{score}/ {sheepsAmount}";
        }

        private void Update()
        {
            timeLimit -= Time.deltaTime;
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeLimit);
            timeText.text = string.Format("{0:00}:{1:00}", timeSpan.Minutes, timeSpan.Seconds);

            if (timeLimit <= 0)
            {
                gameOverPanel.SetActive(true);
            }
        }

        public void AddScore()
        {
            score++;
            scoreText.text = $"{score}/ {sheepsAmount}";
            //Debug.Log($"Score: {score}");
        }
    }
}