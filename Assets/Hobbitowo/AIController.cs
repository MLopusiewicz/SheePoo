using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Hobbitowo
{
    public class AIController : MonoBehaviour
    {
        [field: SerializeField] public NavMeshAgent Agent { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }

        private bool IsIdle =>
            Agent.remainingDistance < Agent.stoppingDistance + Agent.radius + 0.01f;

        public float AgentVelocityFactor => Agent.velocity.magnitude / Agent.speed;

        private void Start()
        {

        }

        private void Update()
        {
            if(!Agent) return;
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
            Agent.isStopped = !toggle;
            Agent.updatePosition = toggle;
            Agent.updateRotation = toggle;
        }
    }
}
