using UnityEngine;

namespace Assets.Scripts.Player
{
    public class LayerTransitionManager : MonoBehaviour
    {
        [SerializeField]
        private float _firstLayerTopY;
        [SerializeField]
        private float _secondLayerTopY;

        private Vector3? _previousPosition;
        private Character _character;
        private SpriteRenderer _characterSpriteRenderer;

        public void Start()
        {
            _character = FindObjectOfType<Character>();
            _characterSpriteRenderer = _character.GetComponent<SpriteRenderer>();
        }

        public void FixedUpdate()
        {
            SwitchIfNewPositionIsInDifferentLayer(_character.transform.position);
            _previousPosition = _character.transform.position;
        }

        private void SwitchIfNewPositionIsInDifferentLayer(Vector3 newPosition)
        {
            if (!_previousPosition.HasValue)
            {
                return;
            }

            var previousLayer = GetPositionSortingLayer(_previousPosition.Value);
            var newLayer = GetPositionSortingLayer(newPosition);

            if (newLayer != previousLayer)
            {
                SwitchToLayer(newLayer, previousLayer);
            }
        }

        private string GetPositionSortingLayer(Vector3 position)
        {
            if (position.y < _firstLayerTopY)
            {
                return SortingLayers.Floor1;
            }

            if (position.y < _secondLayerTopY)
            {
                return SortingLayers.Floor2;
            }

            return SortingLayers.Floor3;
        }

        private void SwitchToLayer(string newLayer, string previousLayer)
        {
            _characterSpriteRenderer.sortingLayerName = newLayer;
        }
    }
}
