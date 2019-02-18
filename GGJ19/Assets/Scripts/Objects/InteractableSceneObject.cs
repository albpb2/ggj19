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
        protected LayerTransitionManager _layerTransitionManager;

        private bool _mouseExitWhileOnPause;
        private bool _mouseEnterWhileOnPause;

        public virtual void Start()
        {
            _characterMovementController = FindObjectOfType<CharacterMovementController>();
            _player = FindObjectOfType<Character>();
            _inputManager = FindObjectOfType<InputManager>();
            _gameManager = FindObjectOfType<GameManager>();
            _layerTransitionManager = FindObjectOfType<LayerTransitionManager>();
        }

        public virtual void Update()
        {
            if (_mouseExitWhileOnPause)
            {
                _inputManager.SetDefaultCursor();
                _mouseExitWhileOnPause = false;
            }

            if (_mouseEnterWhileOnPause)
            {
                _inputManager.SetSelectableCursor();
                _mouseEnterWhileOnPause = false;
            }
        }

        public void OnMouseEnter()
        {
            if (!IsOnCharacterLayer())
            {
                return;
            }

            if (!_gameManager.GameFreezed)
            {
                _inputManager.SetSelectableCursor();
            }
            else
            {
                _mouseEnterWhileOnPause = true;
                _mouseExitWhileOnPause = false;
            }
        }

        public void OnMouseExit()
        {
            if (!_gameManager.GameFreezed)
            {
                _inputManager.SetDefaultCursor();
            }
            else
            {
                _mouseExitWhileOnPause = true;
                _mouseEnterWhileOnPause = false;
            }
        }

        public void OnMouseDown()
        {
            if (_gameManager.GameFreezed || !IsOnCharacterLayer())
            {
                return;
            }

            _characterMovementController.MoveTowards(this);
        }

        public abstract void Interact();

        private bool IsOnCharacterLayer()
        {
            return GetComponent<SpriteRenderer>() != null &&
                  GetComponent<SpriteRenderer>().sortingLayerName == _layerTransitionManager.CurrentLayer;
        }
    }
}
