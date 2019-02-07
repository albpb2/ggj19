using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField]
        private PermanentAudioSource _permanentAudioSourcePrefab;

        private PermanentAudioSource _permanentAudioSource;

        public void Awake()
        {
            _permanentAudioSource = FindObjectOfType<PermanentAudioSource>();

            if (_permanentAudioSource == null)
            {
                _permanentAudioSource = Instantiate(_permanentAudioSourcePrefab).GetComponent<PermanentAudioSource>();
            }
        }

        public void PlayOneShot(AudioClip audioClip)
        {
            _permanentAudioSource.PlayOneShot(audioClip);
        }
    }
}
