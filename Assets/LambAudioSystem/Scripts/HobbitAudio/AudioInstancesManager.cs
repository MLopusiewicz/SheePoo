using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace HobbitAudio
{
    public class AudioInstancesManager : Singleton<AudioInstancesManager>
    {
        [SerializeField] private AudioInstance _audioInstanceTemplate;

        private IObjectPool<AudioInstance> _pool;

        private void Start()
        {
            _pool = new ObjectPool<AudioInstance>(Create, OnGetHandler, OnReleaseHandler);
        }

        #region Pool
        private AudioInstance Get()
        {
            return _pool.Get();
        }

        private AudioInstance Create()
        {
            var element = Instantiate(_audioInstanceTemplate, transform);
            element.Init(_pool);
            return element;
        }

        private void OnGetHandler(AudioInstance element)
        {
            element.gameObject.SetActive(true);
        }

        private void OnReleaseHandler(AudioInstance element)
        {
            element.gameObject.SetActive(false);
            element.transform.parent = transform;
            element.transform.localPosition = Vector3.zero;
        }
        #endregion
        public void Play(AudioContainer audioContainer)
        {
            foreach (var t in audioContainer.Layers)
            {
                var audioInstance = Get();
                audioInstance.Play(t, audioContainer.MasterVolume);
            }
        }
        
        public void Play(AudioContainer audioContainer, Transform origin)
        {
            foreach (var t in audioContainer.Layers)
            {
                var audioInstance = Get();
                audioInstance.transform.parent = origin;
                audioInstance.transform.localPosition = Vector3.zero;
                audioInstance.Play(t, audioContainer.MasterVolume);
            }
        }
    }
}