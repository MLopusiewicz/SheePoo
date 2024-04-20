using System.Collections;
using UnityEngine;

namespace HobbitAudio
{
    public class AudioTest : MonoBehaviour
    {
        [SerializeField] private AudioContainer _container;
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetMouseButtonDown(0))
            {
                AudioInstancesManager.Instance.Play(_container);
            }
        }
    }
}