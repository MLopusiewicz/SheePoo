using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace HobbitAudio
{
    public class AudioInstancesManager : Singleton<AudioInstancesManager>
    {
        [SerializeField] private AudioInstance _audioInstanceTemplate3D;
        [SerializeField] private AudioInstance _audioInstanceTemplate2D;

        private IObjectPool<AudioInstance> _pool3D;
        private IObjectPool<AudioInstance> _pool2D;

        private void Start()
        {
            _pool2D = new ObjectPool<AudioInstance>(Create2D, OnGetHandler, OnReleaseHandler);
            _pool3D = new ObjectPool<AudioInstance>(Create3D, OnGetHandler, OnReleaseHandler);
        }
        
        public void Play(AudioContainer audioContainer)
        {
            foreach (var t in audioContainer.Layers)
            {
                var audioInstance = Get2D();
                audioInstance.Play(t, audioContainer.MasterVolume);
            }
        }
        
        public void Play(AudioContainer audioContainer, Transform origin)
        {
            foreach (var t in audioContainer.Layers)
            {
                var audioInstance = Get3D();
                audioInstance.transform.parent = origin;
                audioInstance.transform.localPosition = Vector3.zero;
                audioInstance.Play(t, audioContainer.MasterVolume);
            }
        }
        
        #region Pool
        private AudioInstance Get2D()
        {
            return _pool2D.Get();
        }
        private AudioInstance Get3D()
        {
            return _pool3D.Get();
        }
        private AudioInstance Create2D()
        {
            var element = Instantiate(_audioInstanceTemplate2D, transform);
            element.Init(_pool2D);
            return element;
        }

        private AudioInstance Create3D()
        {
            var element = Instantiate(_audioInstanceTemplate3D, transform);
            element.Init(_pool3D);
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
    }
}