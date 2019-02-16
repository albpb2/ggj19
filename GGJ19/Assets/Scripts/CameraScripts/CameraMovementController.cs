using UnityEngine;

namespace Assets.Scripts.CameraScripts
{
    public class CameraMovementController : MonoBehaviour
    {
        private Vector3 _initialPosition;

        public void ResetPosition()
        {
            transform.position = _initialPosition;
        }

        void Start()
        {
            _initialPosition = transform.position;
        }
    }
}
