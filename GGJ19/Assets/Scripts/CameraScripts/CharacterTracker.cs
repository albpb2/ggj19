using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.CameraScripts
{
    public class CharacterTracker : MonoBehaviour
    {
        [SerializeField]
        private float _marginPercentage = 20;

        private CharacterMovementController _characterMovementController;
        private float _characterPreviousX;

        public void Start()
        {
            _characterMovementController = FindObjectOfType<CharacterMovementController>();
            _characterPreviousX = _characterMovementController.transform.position.x;
        }

        public void Update()
        {
            if (IsCharacterMovingRight(_characterPreviousX, _characterMovementController.transform.position.x))
            {
                if (IsCharacterCloseToRightEdge(_characterMovementController.transform.position))
                {
                    MoveCamera(_characterMovementController.transform.position.x - _characterPreviousX);
                }
            }
            else if (IsCharacterMovingLeft(_characterPreviousX, _characterMovementController.transform.position.x))
            {
                if (IsCharacterCloseToLefttEdge(_characterMovementController.transform.position))
                {
                    MoveCamera(_characterMovementController.transform.position.x - _characterPreviousX);
                }
            }

            _characterPreviousX = _characterMovementController.transform.position.x;
        }

        private bool IsCharacterMovingRight(float previousX, float currentX)
        {
            return currentX > previousX;
        }

        private bool IsCharacterMovingLeft(float previousX, float currentX)
        {
            return currentX < previousX;
        }

        private bool IsCharacterCloseToRightEdge(Vector3 characterPosition)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(characterPosition);

            return screenPos.x > Screen.width * (100 - _marginPercentage) / 100;
        }

        private bool IsCharacterCloseToLefttEdge(Vector3 characterPosition)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(characterPosition);

            return screenPos.x < Screen.width * _marginPercentage / 100;
        }

        private void MoveCamera(float deltaX)
        {
            Camera.main.transform.position = new Vector3(
                Camera.main.transform.position.x + deltaX,
                Camera.main.transform.position.y,
                Camera.main.transform.position.z);
        }
    }
}
