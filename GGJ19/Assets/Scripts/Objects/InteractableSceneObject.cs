using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Objects
{
    public abstract class InteractableSceneObject : MonoBehaviour, IPointerClickHandler
    {
        protected Character _player;

        public virtual void Start()
        {
            _player = FindObjectOfType<Character>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _player.MoveTowards(this);
        }

        public abstract void Interact();
    }
}
