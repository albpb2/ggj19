using System;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts
{
    public class LayerTransitionManager : MonoBehaviour
    {
        private const int FirstLayer = 0;
        private const int LastLayer = 2;

        [SerializeField]
        private float _yDifferenceBetweenLayers = 0.04f;
        [SerializeField]
        private float _zDifferenceBetweenLayers = 0.65f;
        [SerializeField]
        private float _transitionSpeed = 1f;
        [SerializeField]
        private GameObject _firstTentLayer;
        [SerializeField]
        private GameObject _secondTentLayer;
        [SerializeField]
        private GameObject _thirdTentLayer;

        private int _currentLayer = 0;
        private bool _isTransitioning;
        private float _startTime;
        private float _distance;
        private Vector3 _initialPosition;
        private Vector3 _finalPosition;
        private int _postProcessingLayer;
        private int _focusLayer;
        private CharacterMovementController _characterMovementController;

        public void Start()
        {
            _characterMovementController = FindObjectOfType<CharacterMovementController>();

            _postProcessingLayer = (int) Layers.PostProcessing;
            _focusLayer = (int) Layers.Focus;
        }

        public void Update()
        {
            if (!_isTransitioning)
            {
                return;
            }

            // Distance moved = time * speed.
            float distCovered = (Time.time - _startTime) * _transitionSpeed;

            // Fraction of journey completed = current distance divided by total distance.
            float fracJourney = distCovered / _distance;

            // Set our position as a fraction of the distance between the markers.
            Camera.main.transform.position = Vector3.Lerp(_initialPosition, _finalPosition, fracJourney);

            if (Mathf.Abs(Camera.main.transform.position.z - _finalPosition.z) < 0.01f)
            {
                _isTransitioning = false;
                _currentLayer++;
                switch (_currentLayer)
                {
                    case 0:
                        ApplyLayerToTentLayer(_firstTentLayer, _focusLayer);
                        ApplyLayerToTentLayer(_secondTentLayer, _postProcessingLayer);
                        break;
                    case 1:
                        ApplyLayerToTentLayer(_firstTentLayer, _postProcessingLayer);
                        ApplyLayerToTentLayer(_secondTentLayer, _focusLayer);
                        ApplyLayerToTentLayer(_thirdTentLayer, _postProcessingLayer);
                        break;
                    case 2:
                        ApplyLayerToTentLayer(_secondTentLayer, _postProcessingLayer);
                        ApplyLayerToTentLayer(_thirdTentLayer, _focusLayer);
                        break;
                    default:
                        throw new IndexOutOfRangeException();
                        break;
                }
                _characterMovementController.transform.position =
                    GetNewPosition(_characterMovementController.transform.position, _yDifferenceBetweenLayers,
                        _zDifferenceBetweenLayers);
            }
        }

        public void TransitionToNextLayer()
        {
            _isTransitioning = true;
            _startTime = Time.time;

            _initialPosition = Camera.main.transform.position;
            _finalPosition = GetNewPosition(
                Camera.main.transform.position,
                _yDifferenceBetweenLayers,
                _zDifferenceBetweenLayers);

            _distance = Vector3.Distance(
                _initialPosition,
                _finalPosition);
        }

        private Vector3 GetNewPosition(Vector3 initialPosition, float yDifference, float zDifference)
        {
            return new Vector3(
                initialPosition.x,
                initialPosition.y + yDifference,
                initialPosition.z + zDifference);
        }

        private void ApplyLayerToTentLayer(GameObject tentLayer, int layer)
        {
            tentLayer.layer = layer;
            foreach (Transform tent in tentLayer.transform)
            {
                tent.gameObject.layer = layer;
            }
        }
    }
}
