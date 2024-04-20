using System;
using System.Collections;
using UnityEngine;

namespace Hobbitowo.PoolScoring
{
    public class TableHole : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particles;
        public int LambsNeeded { get; private set; }
        private TableManager _tableManager;
        private AudioSource _audioSource;
        public void InitializeTable(TableManager tableManager, int lambsNeeded)
        {
            _tableManager = tableManager;
            LambsNeeded = lambsNeeded;
            _audioSource = GetComponent<AudioSource>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Sheep")) return;
            var lamb = other.GetComponent<AIController>();
            if(!lamb) return;
            //other.gameObject.SetActive(false);
            //_tableManager.AddScore();
            StartCoroutine(ScoreAnimation(lamb));
        }

        private IEnumerator ScoreAnimation(AIController lamb)
        {
            lamb.ToggleAgent(false);
            _audioSource.Play();
            particles.Play();
            while (Vector3.Distance(lamb.transform.position, transform.position + new Vector3(0,20,0)) > 0.125f)
            {
                lamb.transform.position = Vector3.Lerp(lamb.transform.position, transform.position + new Vector3(0,20,0), Time.deltaTime * 3);
                yield return new WaitForEndOfFrame();
            }
            lamb.gameObject.SetActive(false);
            _tableManager.AddScore();
            yield return null;
        }
    }
}