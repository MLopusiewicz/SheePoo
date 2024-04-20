using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hobbitowo.PoolScoring
{
    public class TableManager : MonoBehaviour
    {
        public int score;
        private List<TableHole> _holes;
        private List<AIController> _aiControllers;
        public Action OnScoreAdded;

        private void Start()
        {
            _holes = new List<TableHole>(GetComponentsInChildren<TableHole>());
            _aiControllers = new List<AIController>(GetComponentsInChildren<AIController>());
            var ratio = _aiControllers.Count / _holes.Count; 
            _holes.ForEach((h) => h.InitializeTable(this, ratio));
        }

        public void AddScore()
        {
            score++;
            //Debug.Log($"Score: {score}");
        }
    }
}