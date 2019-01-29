using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Objects
{
    public abstract class InteractableSceneObject : MonoBehaviour
    {
        protected CharacterMovementController _characterMovementController;
        protected Character _player;
        protected InputManager _inputManager;
        protected GameManager _gameManager;

        public virtual void Start()
        {
            _characterMovementController = FindObjectOfType<CharacterMovementController>();
            _player = FindObjectOfType<Character>();
            _inputManager = FindObjectOfType<InputManager>();
            _gameManager = FindObjectOfType<GameManager>();
        }

        public void OnMouseEnter()
        {
            if (GetComponent<SpriteRenderer>() != null && 
                GetComponent<SpriteRenderer>().sortingLayerName != FindObjectOfType<Character>().GetComponent<SpriteRenderer>().sortingLayerName)
            {
                return;
            }

            if (!_gameManager.GameFreezed)
            {
                _inputManager.SetSelectableCursor();
            }
        }

        public void OnMouseExit()
        {
            if (!_gameManager.GameFreezed)
            {
                _inputManager.SetDefaultCursor();
            }
        }

        public void OnMouseDown()
        {
            if (GetComponent<SpriteRenderer>() != null &&
                GetComponent<SpriteRenderer>().sortingLayerName != FindObjectOfType<Character>().GetComponent<SpriteRenderer>().sortingLayerName)
            {
                return;
            }

            _characterMovementController.MoveTowards(this);
        }

        public abstract void Interact();
    }
}
