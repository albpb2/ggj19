using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class SceneSoundPlayer : MonoBehaviour
    {
        private SoundPlayer _soundPlayer;
        
        public void Awake()
        {
            _soundPlayer = FindObjectOfType<SoundPlayer>();
        }

        public void Play(Sound sound)
        {
            _soundPlayer.Play(sound);
        }

        public void Stop(Sound sound)
        {
            _soundPlayer.Stop(sound);
        }

        public void PlayButtonClickSound()
        {
            _soundPlayer.PlayButtonClickSound();
        }
    }
}
