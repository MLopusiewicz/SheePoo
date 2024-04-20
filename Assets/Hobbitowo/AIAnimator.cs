using System.Collections;
using System.Collections.Generic;
using Hobbitowo;
using UnityEngine;

public class AIAnimator : MonoBehaviour
{
    [SerializeField] private AIController aiController;
    [SerializeField] private AnimationCurve hoppingCurve;
    [SerializeField, Range(0.1f, 10.0f)] private float animationSpeed = 1.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float animationIntensity = 0.2f;
    [SerializeField] private Transform root;
    private float _timer;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        _timer += Time.deltaTime * aiController.AgentVelocityFactor;
        var offset = hoppingCurve.Evaluate(_timer * animationSpeed) * animationIntensity * aiController.AgentVelocityFactor;
        root.localPosition = new Vector3(0, offset, 0);
    }
}
