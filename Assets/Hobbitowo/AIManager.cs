using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Hobbitowo
{
    public class AIManager : MonoBehaviour
    {
        public static AIManager Instance;
        [field: SerializeField] public NavMeshSurface Surface { get; private set; }

        private void Awake()
        {
            Instance = this; //TODO: SINGLETON GURWA -DDDD
        }

        public Vector3 SampleRandomDestination()
        {
            var xOffset = Surface.center.x + Surface.transform.position.x;
            var yOffset = Surface.transform.position.y + Surface.size.y;
            var zOffset = Surface.center.z + Surface.transform.position.z;
            var randX = Random.Range(-Surface.size.x, Surface.size.x) / 2 + xOffset;
            var randZ = Random.Range(-Surface.size.z, Surface.size.z) / 2 + zOffset;
            var randomPosition = new Vector3(randX, yOffset, randZ);

            return NavMesh.SamplePosition(randomPosition, out var hit, Surface.size.y * 2, NavMesh.AllAreas) ? hit.position :
                Vector3.positiveInfinity;
        }
    }
}