using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Hobbitowo
{
    public class AIController : MonoBehaviour
    {
        [field: SerializeField] public NavMeshAgent Agent { get; private set; }
        [field: SerializeField] public Animator Animator { get; private set; }

        public float AgentVelocityFactor => Agent.velocity.magnitude / Agent.speed;
        
        void Start()
        {
            StartCoroutine(GoToPosition(AIManager.Instance.SampleRandomDestination()));
        }
    
        void Update()
        {
        
        }

        private IEnumerator GoToPosition(Vector3 destination)
        {
            yield return new WaitForSeconds(1);
            Agent.SetDestination(destination);
            while (Agent.remainingDistance > Agent.stoppingDistance + Agent.radius + 0.01f)
            {
                yield return new WaitForEndOfFrame();
            }
            //transform.position = destination;
            //yield return new WaitForSeconds(waitTime);
            StartCoroutine(GoToPosition(AIManager.Instance.SampleRandomDestination()));
        }
    }
}
