using System;
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
        public CollisionController CollisionController { get; private set; }

        [SerializeField] private float stuckThreshold = 0.05f;

        private bool IsIdle =>
            Agent.remainingDistance < Agent.stoppingDistance + Agent.radius + 0.01f;

        public float AgentVelocityFactor => Agent.velocity.magnitude / Agent.speed;

        private Vector3 lastPosition;
        private float lastTime;

        private bool MightBeStuck()
        {
            var vel = (transform.position - lastPosition).magnitude / (Time.time - lastTime);
            //Debug.Log($"Sheep {name}, vel: {vel}");

            lastPosition = transform.position;
            lastTime = Time.time;

            return vel < stuckThreshold;
        }

        private void Start()
        {
            _trailRenderer = GetComponentInChildren<TrailRenderer>();
            _trailRenderer.enabled = false;
            CollisionController = GetComponent<CollisionController>();
        }

        private void Update()
        {
            if(!Agent || !Agent.enabled) return;
            RandomMovementBehaviour();
        }

        private void RandomMovementBehaviour()
        {
            if (!IsIdle && Agent.pathStatus != NavMeshPathStatus.PathInvalid && !MightBeStuck())return;
            var sampledPosition = AIManager.Instance.SampleRandomDestination();
            if(sampledPosition != Vector3.positiveInfinity) GoToPosition(sampledPosition);
        }

        private void GoToPosition(Vector3 destination)
        {
            Agent.SetDestination(destination);
        }

        public void ToggleAgent(bool toggle)
        {
            Animator.enabled = toggle;
            Agent.enabled = toggle;
        }

        public void LaunchSheep()
        {
            ToggleAgent(false);
            CollisionController.isBarked = false;
            CollisionController.TryReactivateAIComponents(gameObject);
            Rigidbody.isKinematic = true;
            _trailRenderer.enabled = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, Agent.destination);
        }
    }
}
