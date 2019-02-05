using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Feet : MonoBehaviour
    {
        private Vector3 originalPosition;
        private Quaternion originalRotation;

        public bool AllowVerticalMovementUp { get; set; }
        public bool AllowVerticalMovementDown { get; set; }
        public bool AllowHorizontalMovementLeft { get; set; }
        public bool AllowHorizontalMovementRight { get; set; }

        public void Start()
        {
            AllowVerticalMovementUp = true;
            AllowVerticalMovementDown = true;
            AllowHorizontalMovementLeft = true;
            AllowHorizontalMovementRight = true;

            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
        }

        public void Update()
        {
            transform.localPosition = originalPosition;
            transform.localRotation = originalRotation;
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            switch (collider.tag)
            {
                case Tags.BottomObjectTrigger:
                    AllowVerticalMovementUp = false;
                    break;
                case Tags.TopObjectTrigger:
                    AllowVerticalMovementDown = false;
                    break;
                case Tags.RightObjectTrigger:
                    AllowHorizontalMovementLeft = false;
                    break;
                case Tags.LeftObjectTrigger:
                    AllowHorizontalMovementRight = false;
                    break;
                default:
                    break;
            }
        }

        void OnTriggerStay2D(Collider2D collider)
        {
            switch (collider.tag)
            {
                case Tags.BottomObjectTrigger:
                    AllowVerticalMovementUp = false;
                    break;
                case Tags.TopObjectTrigger:
                    AllowVerticalMovementDown = false;
                    break;
                case Tags.RightObjectTrigger:
                    AllowHorizontalMovementLeft = false;
                    break;
                case Tags.LeftObjectTrigger:
                    AllowHorizontalMovementRight = false;
                    break;
                default:
                    break;
            }
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            switch (collider.tag)
            {
                case Tags.BottomObjectTrigger:
                    AllowVerticalMovementUp = true;
                    break;
                case Tags.TopObjectTrigger:
                    AllowVerticalMovementDown = true;
                    break;
                case Tags.RightObjectTrigger:
                    AllowHorizontalMovementLeft = true;
                    break;
                case Tags.LeftObjectTrigger:
                    AllowHorizontalMovementRight = true;
                    break;
                default:
                    break;
            }
        }
    }
}
