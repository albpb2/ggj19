using Assets.Scripts.Extensions;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Conversation
{
    public class DialogBoxSoundManager : MonoBehaviour
    {
        [SerializeField]
        private List<StudioEventEmitter> _shortMessageStudioEventEmitters;
        [SerializeField]
        private List<StudioEventEmitter> _mediumMessageStudioEventEmitters;
        [SerializeField]
        private List<StudioEventEmitter> _longMessageStudioEventEmitters;

        private StudioEventEmitter _currentEventEmitter;

        public void PlayShortMessageSound()
        {
            PlayMessageSound(_shortMessageStudioEventEmitters);
        }

        public void PlayMediumMessageSound()
        {
            PlayMessageSound(_mediumMessageStudioEventEmitters);
        }

        public void PlayLongMessageSound()
        {
            PlayMessageSound(_longMessageStudioEventEmitters);
        }

        public void StopCurrentSound()
        {
            if (_currentEventEmitter != null)
            {
                _currentEventEmitter.enabled = false;
            }
        }

        private void PlayMessageSound(List<StudioEventEmitter> validEventEmitters)
        {
            StopCurrentSound();

            _currentEventEmitter = validEventEmitters.GetRandomElement();

            PlayCurrentSound();
        }

        private void PlayCurrentSound()
        {
            if (_currentEventEmitter != null)
            {
                _currentEventEmitter.enabled = true;
            }
        }
    }
}
