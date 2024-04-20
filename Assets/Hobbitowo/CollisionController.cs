using System.Collections;
using System.Collections.Generic;
using Hobbitowo;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    [SerializeField] private LayerMask AgentLayer;
    private AIController aIController;

    [SerializeField] private bool isBarked = false;
    
    private void Awake()
    {
        aIController = GetComponent<AIController>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isBarked && other.gameObject.CompareTag("Agent"))
        {
            CollisionController collisionController = other.collider.GetComponent<CollisionController>();
            collisionController.SetBarked(true);

            if (other.gameObject.CompareTag("Agent"))
            {
                collisionController.SetBarked(true);
                SetAIComponentsActive(other, false);
            }
        }
    }

    // To be called if the dog barking area hits the agent
    public void SetBarked(bool value)
    {
        isBarked = value;
    }

    private void SetAIComponentsActive(Collision collision, bool status)
    {
        AIController agent = collision.collider.GetComponent<AIController>();
        agent.Agent.enabled = status;
        agent.enabled = status;
    }
}
