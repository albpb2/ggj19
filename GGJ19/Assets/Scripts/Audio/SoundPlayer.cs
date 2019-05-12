using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class SoundPlayer : MonoBehaviour
    {
        [FMODUnity.EventRef]
        [SerializeField]
        private string _fillBottleEventName;
        [FMODUnity.EventRef]
        [SerializeField]
        private string _openBagEventName;
        [FMODUnity.EventRef]
        [SerializeField]
        private string _endOfDayEventName;

        private EventInstance _fillBottleEvent;
        private EventInstance _openBagEvent;
        private EventInstance _endOfDayEvent;

        private Dictionary<Sound, EventInstance> _eventPerSound;

        public void Start()
        {
            _fillBottleEvent = FMODUnity.RuntimeManager.CreateInstance(_fillBottleEventName);
            _openBagEvent = FMODUnity.RuntimeManager.CreateInstance(_openBagEventName);
            _endOfDayEvent = FMODUnity.RuntimeManager.CreateInstance(_endOfDayEventName);

            _eventPerSound = new Dictionary<Sound, EventInstance>
            {
                [Sound.FillWatter] = _fillBottleEvent,
                [Sound.OpenBag] = _openBagEvent,
                [Sound.EndOfDay] = _endOfDayEvent
            };
        }

        public void Play(Sound sound)
        {
            if (_eventPerSound.TryGetValue(sound, out var soundEvent))
            {
                soundEvent.start();
            }
            else
            {
                throw new System.Exception($"Sound {sound} not found");
            }
        }

        public void Stop(Sound sound)
        {
            if (_eventPerSound.TryGetValue(sound, out var soundEvent))
            {
                soundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }
            else
            {
                throw new System.Exception($"Sound {sound} not found");
            }
        }
    }
}
