using System;
using System.Collections;
using UnityEngine;
using Cinemachine;

namespace Hobbitowo.PoolScoring
{
    public class TableHole : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particles;
        public int LambsNeeded { get; private set; }
        private TableManager _tableManager;
        private AudioSource _audioSource;
        private Vector3 _launchPosition;
        private CinemachineImpulseSource _cinemachineImpulseSource;
        public void InitializeTable(TableManager tableManager, int lambsNeeded)
        {
            _tableManager = tableManager;
            LambsNeeded = lambsNeeded;
            _audioSource = GetComponent<AudioSource>();
            _cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
            _launchPosition = transform.position + new Vector3(0, 50.0f, 0);
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
            _cinemachineImpulseSource.GenerateImpulseAtPositionWithVelocity(transform.position, transform.up);
            _audioSource.Play();
            particles.Play();
            while (Vector3.Distance(lamb.transform.position, _launchPosition) > 0.125f)
            {
                lamb.transform.position = Vector3.Lerp(lamb.transform.position, _launchPosition, Time.deltaTime * 10);
                yield return new WaitForEndOfFrame();
            }
            lamb.gameObject.SetActive(false);
            _tableManager.AddScore();
            yield return null;
        }
    }
}