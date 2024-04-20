using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Hobbitowo;
using UnityEngine;

public class CollisionController : MonoBehaviour
{
    private AIController aIController;
    private Rigidbody rb;
    [SerializeField] private float rbVelocity = 0.06f;

    [SerializeField] private bool isBarked = false;
    
    private void Awake()
    {
        aIController = GetComponent<AIController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isBarked)
        {
            TryReactivateAIComponents(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isBarked)
        {
            CollisionController collisionController = other.collider.GetComponent<CollisionController>();
            if (other.gameObject.CompareTag("Agent"))
            {
                SetAIComponentsActive(collisionController, true);
            }
        }
    }

    public void RepelFrom(Vector3 epicenter, float intensity)
    {
        SetBarkedStatus(true);
        SetAIComponentsActive(this, true);
        var dir = rb.position - epicenter;
        rb.AddForce(dir * intensity, ForceMode.Impulse);
    }

    // To be called if the dog barking area hits the agent
    public void SetBarkedStatus(bool value)
    {
        isBarked = value;
    }

    private void SetAIComponentsActive(CollisionController collisionController, bool barkedStatus)
    {
        collisionController.SetBarkedStatus(barkedStatus);
        AIController agent = collisionController.GetComponent<AIController>();
        agent.Agent.enabled = !barkedStatus;
        agent.enabled = !barkedStatus;
    }

    private void TryReactivateAIComponents(GameObject gameObject)
    {
        if (rb.velocity.magnitude < rbVelocity)
        {
            SetAIComponentsActive(this, false);
        }
    }
}
