using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class PermanentAudioSource : MonoBehaviour
    {
        public static PermanentAudioSource instance;

        private AudioSource _audioSource;

        public void Awake()
        {
            _audioSource = GetComponent<AudioSource>();

            if (instance == null)
            {
                DontDestroyOnLoad(gameObject);
                instance = this;
            }
            else if (instance != this)
                Destroy(gameObject);
        }

        public void PlayOneShot(AudioClip audioClip)
        {
            _audioSource.PlayOneShot(audioClip);
        }
    }
}
