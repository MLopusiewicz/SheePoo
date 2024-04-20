using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarkRepel : MonoBehaviour
{
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _minImpulse = 1f;
    [SerializeField] private float _minArea = 1f;
    [SerializeField] private float _maxImpulse = 10f;
    [SerializeField] private float _maxArea = 5f;
    
    private Collider[] _results = new Collider[20];

    public float BarkMaxArea => _maxArea;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_playerTransform.position, _minArea);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_playerTransform.position, _maxArea);
    }

    public void DoBark(float barkIntensity)
    {
        var hitCount = Physics.OverlapSphereNonAlloc(_playerTransform.position, Mathf.Lerp(_minArea, _maxArea, barkIntensity), _results, _layerMask);

        var intensity = Mathf.Lerp(_minImpulse, _maxImpulse, barkIntensity);

        for (int i = 0; i < hitCount; i++)
        {
            var h = _results[i];
            if(h.TryGetComponent<CollisionController>(out var collisionController))
            {
                collisionController.RepelFrom(_playerTransform.position, intensity);
            }
        }
    }
}
