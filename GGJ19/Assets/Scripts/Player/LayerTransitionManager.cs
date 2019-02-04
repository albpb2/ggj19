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
        private Dictionary<string, List<GameObject>> _mainCollidersPerLayer; // Colliders to activate when the character is in the layer
        private Dictionary<string, List<GameObject>> _secondaryCollidersPerLayer; // Colliders to activate when the character is NOT in the layer

        public void Start()
        {
            _character = FindObjectOfType<Character>();
            _characterSpriteRenderer = _character.GetComponent<SpriteRenderer>();

            _mainCollidersPerLayer = new Dictionary<string, List<GameObject>>();
            _secondaryCollidersPerLayer = new Dictionary<string, List<GameObject>>();

            FindColliders();
            DisableBackLayersColliders();
        }

        public void FixedUpdate()
        {
            SwitchIfNewPositionIsInDifferentLayer(_character.transform.position);
            _previousPosition = _character.transform.position;
        }

        private void FindColliders()
        {
            var allObjectsWithColliders = GameObject
                .FindGameObjectsWithTag(Tags.VerticalTriggerUp)
                .Select(c => c.transform.parent.gameObject);

            FindLayerColliders(SortingLayers.Floor1, allObjectsWithColliders);
            FindLayerColliders(SortingLayers.Floor2, allObjectsWithColliders);
            FindLayerColliders(SortingLayers.Floor3, allObjectsWithColliders);
        }

        private void FindLayerColliders(string sortingLayer, IEnumerable<GameObject> allObjectsWithColliders)
        {
            var collidersInLayer = allObjectsWithColliders
                .Where(o => o.GetComponent<SpriteRenderer>().sortingLayerName == sortingLayer)
                .SelectMany(o => o.GetComponentsInChildren<Collider2D>())
                .Select(c => c.gameObject)
                .ToList();

            _mainCollidersPerLayer[sortingLayer] = collidersInLayer.Where(c => c.tag == Tags.VerticalTriggerUp)
                    .Concat(collidersInLayer.Where(c => c.tag == Tags.HorizontalTriggerLeft))
                    .Concat(collidersInLayer.Where(c => c.tag == Tags.HorizontalTriggerRight)).ToList();

            _secondaryCollidersPerLayer[sortingLayer] = collidersInLayer.Where(c => c.tag == Tags.VerticalTriggerDown).ToList();
        }

        private void DisableBackLayersColliders()
        {
            EnableMainColliders(SortingLayers.Floor1);
            DisableMainColliders(SortingLayers.Floor2);
            DisableMainColliders(SortingLayers.Floor3);
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
            DisableMainColliders(previousLayer);
            EnableMainColliders(newLayer);
        }

        private void EnableMainColliders(string sortingLayer)
        {
            foreach (var colliderToEnable in _mainCollidersPerLayer[sortingLayer])
            {
                colliderToEnable.gameObject.SetActive(true);
            }
            
            foreach (var colliderToDisable in _secondaryCollidersPerLayer[sortingLayer])
            {
                colliderToDisable.gameObject.SetActive(false);
            }
        }

        private void DisableMainColliders(string sortingLayer)
        {
            foreach (var colliderToDisable in _mainCollidersPerLayer[sortingLayer])
            {
                colliderToDisable.gameObject.SetActive(false);
            }

            foreach (var colliderToEnable in _secondaryCollidersPerLayer[sortingLayer])
            {
                colliderToEnable.gameObject.SetActive(true);
            }
        }
    }
}
