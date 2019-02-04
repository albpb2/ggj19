using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Feet : MonoBehaviour
    {
        public bool AllowVerticalMovementUp { get; set; }
        public bool AllowVerticalMovementDown { get; set; }
        public bool AllowHorizontalMovementLeft { get; set; }
        public bool AllowHorizontalMovementRight { get; set; }

        void Start()
        {
            AllowVerticalMovementUp = true;
            AllowVerticalMovementDown = true;
            AllowHorizontalMovementLeft = true;
            AllowHorizontalMovementRight = true;
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            switch (collider.tag)
            {
                case Tags.VerticalTriggerUp:
                    AllowVerticalMovementUp = false;
                    break;
                case Tags.VerticalTriggerDown:
                    AllowVerticalMovementDown = false;
                    break;
                case Tags.HorizontalTriggerLeft:
                    AllowHorizontalMovementLeft = false;
                    break;
                case Tags.HorizontalTriggerRight:
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
                case Tags.VerticalTriggerUp:
                    AllowVerticalMovementUp = false;
                    break;
                case Tags.VerticalTriggerDown:
                    AllowVerticalMovementDown = false;
                    break;
                case Tags.HorizontalTriggerLeft:
                    AllowHorizontalMovementLeft = false;
                    break;
                case Tags.HorizontalTriggerRight:
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
                case Tags.VerticalTriggerUp:
                    AllowVerticalMovementUp = true;
                    break;
                case Tags.VerticalTriggerDown:
                    AllowVerticalMovementDown = true;
                    break;
                case Tags.HorizontalTriggerLeft:
                    AllowHorizontalMovementLeft = true;
                    break;
                case Tags.HorizontalTriggerRight:
                    AllowHorizontalMovementRight = true;
                    break;
                default:
                    break;
            }
        }
    }
}
