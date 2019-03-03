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
        private string _currentLayer;

        public string CurrentLayer => _currentLayer;

        public void Start()
        {
            _character = FindObjectOfType<Character>();
            _characterSpriteRenderer = _character.GetComponent<SpriteRenderer>();
            _currentLayer = GetPositionSortingLayer(_character.transform.position);
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

            var newLayer = GetPositionSortingLayer(newPosition);

            if (newLayer != _currentLayer)
            {
                SwitchToLayer(newLayer, _currentLayer);
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
            _currentLayer = newLayer;
            _characterSpriteRenderer.sortingLayerName = newLayer;
        }
    }
}
