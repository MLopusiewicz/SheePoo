using System.Collections;
using UnityEngine;

namespace HobbitAudio
{
    [System.Serializable]
    public class AudioLayer
    {
        [field: SerializeField] public AudioClip Clip { get; private set; }
        [field: SerializeField, Range(0.0f, 5.0f)] public float Offset { get; private set; }
        [field: SerializeField, Range(0.0f, 1.0f)] public float Volume { get; private set; } = 1.0f;
        [field: SerializeField, Range(0.25f, 3.0f)] public float Pitch { get; private set; } = 1.0f;
    }
}