using Assets.Scripts.Audio;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    public class HoverableButton : MonoBehaviour, IPointerEnterHandler
    {
        private SoundPlayer _soundPlayer;

        public void Awake()
        {
            _soundPlayer = FindObjectOfType<SoundPlayer>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _soundPlayer.Play(Sound.ButtonHover);
        }
    }
}
