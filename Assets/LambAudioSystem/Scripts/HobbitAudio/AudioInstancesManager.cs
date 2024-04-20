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
        }
        #endregion
        public void Play(AudioContainer audioContainer)
        {
            AudioInstance audioInstance;
            for (int i = 0; i < audioContainer.Layers.Length; i++)
            {
                audioInstance = Get();
                audioInstance.Play(audioContainer.Layers[i], audioContainer.MasterVolume);
            }
        }
    }
}