using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Hobbitowo
{
    public class AIController : MonoBehaviour
    {
        [field: SerializeField] public NavMeshAgent Agent { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        private TrailRenderer _trailRenderer;

        private bool IsIdle =>
            Agent.remainingDistance < Agent.stoppingDistance + Agent.radius + 0.01f;

        public float AgentVelocityFactor => Agent.velocity.magnitude / Agent.speed;

        private void Start()
        {
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
            _trailRenderer.enabled = false;
        }

        private void Update()
        {
            if(!Agent || !Agent.enabled) return;
            RandomMovementBehaviour();
        }

        private void RandomMovementBehaviour()
        {
            if (IsIdle)
            {
                GoToPosition(AIManager.Instance.SampleRandomDestination());
            }
        }

        private void GoToPosition(Vector3 destination)
        {
            Agent.SetDestination(destination);
        }

        public void ToggleAgent(bool toggle)
        {
            Animator.enabled = toggle;
            Agent.enabled = toggle;
            // Agent.isStopped = !toggle;
            // Agent.updatePosition = toggle;
            // Agent.updateRotation = toggle;
        }

        public void LaunchSheep()
        {
            ToggleAgent(false);
            Rigidbody.isKinematic = true;
            _trailRenderer.enabled = true;
        }
    }
}
