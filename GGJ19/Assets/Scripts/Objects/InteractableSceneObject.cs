using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Objects
{
    public abstract class InteractableSceneObject : MonoBehaviour, IPointerClickHandler
    {
        protected CharacterMovementController _characterMovementController;
        protected Character _player;

        public virtual void Start()
        {
            _characterMovementController = FindObjectOfType<CharacterMovementController>();
            _player = FindObjectOfType<Character>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _characterMovementController.MoveTowards(this);
        }

        public abstract void Interact();
    }
}
