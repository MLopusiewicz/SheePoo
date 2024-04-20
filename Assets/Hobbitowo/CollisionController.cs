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
            // things that are agents are 
            if(other.collider.TryGetComponent<CollisionController>(out var otherCollisionController))
            {
                SetAIComponentsActive(otherCollisionController, true);
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
        //Debug.Log($"This sheep is barked: {isBarked}");
    }

    private void SetAIComponentsActive(CollisionController collisionController, bool barkedStatus)
    {
        collisionController.SetBarkedStatus(barkedStatus);
        aIController.ToggleAgent(!barkedStatus);
    }

    private void TryReactivateAIComponents(GameObject gameObject)
    {
        if (rb.velocity.magnitude < rbVelocity)
        {
            SetAIComponentsActive(this, false);
        }
    }
}
