using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace HobbitAudio
{
    public class AudioInstance : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        private IObjectPool<AudioInstance> _pool;
        public void Init(IObjectPool<AudioInstance> pool)
        {
            _pool = pool;
        }
        
        public void Play(AudioLayer audioLayer, float containerVolume = 1.0f)
        {
            StartCoroutine(PlaySequence(audioLayer, containerVolume));
        }

        private IEnumerator PlaySequence(AudioLayer audioLayer, float containerVolume)
        {
            _audioSource.clip = audioLayer.Clip;
            _audioSource.volume = audioLayer.Volume * containerVolume;
            _audioSource.pitch = audioLayer.Pitch;

            yield return new WaitForSeconds(audioLayer.Offset);
            _audioSource.Play();

            while(_audioSource.isPlaying) yield return new WaitForEndOfFrame();
            _pool.Release(this);
        }
    }
}
