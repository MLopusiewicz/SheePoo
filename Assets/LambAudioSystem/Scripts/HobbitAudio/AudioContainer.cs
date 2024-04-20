using UnityEngine;

namespace HobbitAudio
{
    [CreateAssetMenu(fileName = "NewAudioContainer", menuName = "Audio/Audio Container")]
    public class AudioContainer : ScriptableObject
    {
        [field: SerializeField, Range(0.0f, 1.0f)] public float MasterVolume { get; private set; } = 1.0f;
        [field: SerializeField] public AudioLayer[] Layers { get; private set; }
    }
}
