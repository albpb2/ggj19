using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Objects
{
    public abstract class InteractableSceneObject : MonoBehaviour, IPointerClickHandler
    {
        protected CharacterMovementController _characterMovementController;
        protected Character _player;
        protected InputManager _inputManager;

        public virtual void Start()
        {
            _characterMovementController = FindObjectOfType<CharacterMovementController>();
            _player = FindObjectOfType<Character>();
            _inputManager = FindObjectOfType<InputManager>();
        }

        public void OnMouseEnter()
        {
            _inputManager.SetSelectableCursor();
        }

        public void OnMouseExit()
        {
            _inputManager.SetDefaultCursor();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _characterMovementController.MoveTowards(this);
        }

        public abstract void Interact();
    }
}
