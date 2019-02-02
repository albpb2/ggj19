using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.CameraScripts
{
    public class CharacterTracker : MonoBehaviour
    {
        [SerializeField]
        private float _horizontalMarginPercentage = 20;
        [SerializeField]
        private float _verticalMarginPercentage = 20;
        [SerializeField]
        private float _minCameraY = 5.5f;

        private CharacterMovementController _characterMovementController;
        private float _characterPreviousX;
        private float _characterPreviousY;

        public void Start()
        {
            _characterMovementController = FindObjectOfType<CharacterMovementController>();
            _characterPreviousX = _characterMovementController.transform.position.x;
            _characterPreviousY = _characterMovementController.transform.position.y;
        }

        public void Update()
        {
            if (IsCharacterMovingRight(_characterPreviousX, _characterMovementController.transform.position.x))
            {
                if (IsCharacterCloseToRightEdge(_characterMovementController.transform.position))
                {
                    MoveCameraHorizontally(_characterMovementController.transform.position.x - _characterPreviousX);
                }
            }
            else if (IsCharacterMovingLeft(_characterPreviousX, _characterMovementController.transform.position.x))
            {
                if (IsCharacterCloseToLeftEdge(_characterMovementController.transform.position))
                {
                    MoveCameraHorizontally(_characterMovementController.transform.position.x - _characterPreviousX);
                }
            }

            if (IsCharacterMovingUp(_characterPreviousY, _characterMovementController.transform.position.y))
            {
                if (IsCharacterCloseToTopEdge(_characterMovementController.transform.position))
                {
                    MoveCameraVertically(_characterMovementController.transform.position.y - _characterPreviousY);
                }
            }
            else if (IsCharacterMovingDown(_characterPreviousY, _characterMovementController.transform.position.y))
            {
                if (IsCharacterCloseToBottomEdge(_characterMovementController.transform.position) && 
                    Camera.main.transform.position.y > _minCameraY)
                {
                    MoveCameraVertically(_characterMovementController.transform.position.y - _characterPreviousY);
                }
            }

            _characterPreviousX = _characterMovementController.transform.position.x;
            _characterPreviousY = _characterMovementController.transform.position.y;
        }

        private bool IsCharacterMovingRight(float previousX, float currentX)
        {
            return currentX > previousX;
        }

        private bool IsCharacterMovingLeft(float previousX, float currentX)
        {
            return currentX < previousX;
        }

        private bool IsCharacterMovingUp(float previousY, float currentY)
        {
            return currentY > previousY;
        }

        private bool IsCharacterMovingDown(float previousY, float currentY)
        {
            return currentY < previousY;
        }

        private bool IsCharacterCloseToRightEdge(Vector3 characterPosition)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(characterPosition);

            return screenPos.x > Screen.width * (100 - _horizontalMarginPercentage) / 100;
        }

        private bool IsCharacterCloseToLeftEdge(Vector3 characterPosition)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(characterPosition);

            return screenPos.x < Screen.width * _horizontalMarginPercentage / 100;
        }

        private bool IsCharacterCloseToTopEdge(Vector3 characterPosition)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(characterPosition);

            return screenPos.y > Screen.height * (100 - _verticalMarginPercentage) / 100;
        }

        private bool IsCharacterCloseToBottomEdge(Vector3 characterPosition)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(characterPosition);

            return screenPos.y < Screen.height * _verticalMarginPercentage / 100;
        }

        private void MoveCameraHorizontally(float deltaX)
        {
            Camera.main.transform.position = new Vector3(
                Camera.main.transform.position.x + deltaX,
                Camera.main.transform.position.y,
                Camera.main.transform.position.z);
        }

        private void MoveCameraVertically(float deltaY)
        {
            Camera.main.transform.position = new Vector3(
                Camera.main.transform.position.x,
                Camera.main.transform.position.y + deltaY,
                Camera.main.transform.position.z);
        }
    }
}
