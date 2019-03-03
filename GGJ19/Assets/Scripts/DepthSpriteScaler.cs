using UnityEngine;

namespace Assets.Scripts
{
    public class DepthSpriteScaler : MonoBehaviour
    {
        [SerializeField]
        private float _maxY;
        [SerializeField]
        private float _minScale;

        private float _initialY;
        private float _initialScale;
        private float _scalePerY;

        protected void Start()
        {
            _initialScale = transform.localScale.y;
            _initialY = transform.localPosition.y;
            _scalePerY = (_minScale - _initialScale) / Mathf.Pow(_maxY - _initialY, 2);
        }

        protected void Update()
        {
            AdjustScale();
        }

        private void AdjustScale()
        {
            // Scale = scale0 + scalePerY * (deltaPosition ^ 2)
            // Reduce quadratically instead of linearly to reduce more in the second and third layer
            // because linear reduction is weird with this camp layout
            var newScale = _initialScale + Mathf.Pow(transform.position.y - _initialY, 2) * _scalePerY;

            transform.localScale = new Vector3(newScale, newScale, transform.localScale.z);
        }
    }
}
