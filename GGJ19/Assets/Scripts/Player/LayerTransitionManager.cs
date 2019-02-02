using System.Collections.Generic;
using System.Linq;
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
        private Dictionary<string, List<GameObject>> _verticalCollidersPerSortingLayer;
        private List<GameObject> _firstLayerVerticalColliders;
        private List<GameObject> _secondLayerVerticalColliders;
        private List<GameObject> _thirdLayerVerticalColliders;

        public void Start()
        {
            _character = FindObjectOfType<Character>();
            _characterSpriteRenderer = _character.GetComponent<SpriteRenderer>();

            FindVerticalColliders();
            DisableBackLayersColliders();
            InitializeLayerRelationships();
        }

        public void FixedUpdate()
        {
            SwitchIfNewPositionIsInDifferentLayer(_character.transform.position);
            _previousPosition = _character.transform.position;
        }

        private void FindVerticalColliders()
        {
            _firstLayerVerticalColliders = GetLayerVerticalColliders(Layers.FirstFloor);
            _secondLayerVerticalColliders = GetLayerVerticalColliders(Layers.SecondFloor);
            _thirdLayerVerticalColliders = GetLayerVerticalColliders(Layers.ThirdFloor);
        }

        private List<GameObject> GetLayerVerticalColliders(Layers layer)
        {
            var colliders = GameObject
                .FindGameObjectsWithTag(Tags.VerticalTriggerUp)
                .Where(o => o.layer == (int)layer).ToList();
            colliders.AddRange(GameObject
                .FindGameObjectsWithTag(Tags.VerticalTriggerDown)
                .Where(o => o.layer == (int)layer));

            return colliders;
        }

        private void DisableBackLayersColliders()
        {
            foreach (var collider in _secondLayerVerticalColliders)
            {
                collider.SetActive(false);
            }
            foreach (var collider in _thirdLayerVerticalColliders)
            {
                collider.SetActive(false);
            }
        }

        private void InitializeLayerRelationships()
        {
            _verticalCollidersPerSortingLayer = new Dictionary<string, List<GameObject>>
            {
                [SortingLayers.Floor1] = _firstLayerVerticalColliders,
                [SortingLayers.Floor2] = _secondLayerVerticalColliders,
                [SortingLayers.Floor3] = _thirdLayerVerticalColliders,
            };
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
            DisableVerticalColliders(previousLayer);
            EnableVerticalColliders(newLayer);
        }

        private void EnableVerticalColliders(string sortingLayer)
        {
            var colliders = _verticalCollidersPerSortingLayer[sortingLayer];
            foreach (var collider in colliders)
            {
                collider.gameObject.SetActive(true);
            }
        }

        private void DisableVerticalColliders(string sortingLayer)
        {
            var colliders = _verticalCollidersPerSortingLayer[sortingLayer];
            foreach (var collider in colliders)
            {
                collider.gameObject.SetActive(false);
            }
        }
    }
}
