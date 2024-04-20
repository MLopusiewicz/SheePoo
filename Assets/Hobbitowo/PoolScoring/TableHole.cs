using System;
using System.Collections;
using UnityEngine;
using Cinemachine;
using HobbitAudio;

namespace Hobbitowo.PoolScoring
{
    public class TableHole : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particles;
        [SerializeField] private AudioContainer audioContainer;
        
        public int LambsNeeded { get; private set; }
        private TableManager _tableManager;
        private Vector3 _launchPosition;
        private CinemachineImpulseSource _cinemachineImpulseSource;
        public void InitializeTable(TableManager tableManager, int lambsNeeded)
        {
            _tableManager = tableManager;
            LambsNeeded = lambsNeeded;
            _cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
            _launchPosition = transform.position + new Vector3(0, 50.0f, 0);
        }

        private void OnTriggerEnter(Collider other)
        {
            var lamb = other.GetComponent<AIController>();
            if(!lamb) return;
            //other.gameObject.SetActive(false);
            //_tableManager.AddScore();
            StartCoroutine(ScoreAnimation(lamb));
        }

        private IEnumerator ScoreAnimation(AIController lamb)
        {
            lamb.LaunchSheep();
            _cinemachineImpulseSource.GenerateImpulseAtPositionWithVelocity(transform.position, transform.up / 2);
            AudioInstancesManager.Instance.Play(audioContainer, transform);
            particles.Play();
            while (Vector3.Distance(lamb.transform.position, _launchPosition) > 0.125f)
            {
                lamb.transform.position = Vector3.LerpUnclamped(lamb.transform.position, _launchPosition, Time.deltaTime * 5);
                yield return new WaitForEndOfFrame();
            }
            lamb.gameObject.SetActive(false);
            _tableManager.AddScore();
        }
    }
}