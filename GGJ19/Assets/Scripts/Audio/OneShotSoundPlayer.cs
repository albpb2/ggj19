﻿using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Audio
{
    public class OneShotSoundPlayer : MonoBehaviour
    {
        [FMODUnity.EventRef]
        [SerializeField]
        private string _fillBottleEventName;
        private EventInstance _fillBottleEvent;

        private Dictionary<Sound, EventInstance> _eventPerSound;

        public void Start()
        {
            _fillBottleEvent = FMODUnity.RuntimeManager.CreateInstance(_fillBottleEventName);

            _eventPerSound = new Dictionary<Sound, EventInstance>
            {
                [Sound.FillWatter] = _fillBottleEvent
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
    }
}