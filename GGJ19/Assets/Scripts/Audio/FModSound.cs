using FMOD.Studio;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class FModSound : MonoBehaviour
    {
        [FMODUnity.EventRef]
        [SerializeField]
        private string _eventName;
        private EventInstance _event;

        public void Start()
        {
            _event = FMODUnity.RuntimeManager.CreateInstance(_eventName);
        }

        public void Play()
        {
            _event.start();
        }
    }
}
