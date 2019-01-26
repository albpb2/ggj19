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
        private bool _transitioningToNextLayer;
        private float _startTime;
        private float _distance;
        private Vector3 _initialCameraPosition;
        private Vector3 _finalCameraPosition;
        private Vector3 _initialCharacterPosition;
        private Vector3 _finalCharacterPosition;
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

            float distCovered = (Time.time - _startTime) * _transitionSpeed;

            float fracJourney = distCovered / _distance;

            Camera.main.transform.position = Vector3.Lerp(_initialCameraPosition, _finalCameraPosition, fracJourney);
            _characterMovementController.transform.position =
                Vector3.Lerp(_initialCharacterPosition, _finalCharacterPosition, fracJourney);

            if (Mathf.Abs(Camera.main.transform.position.z - _finalCameraPosition.z) < 0.01f)
            {
                _isTransitioning = false;

                SwitchLayer();
            }
        }

        public void TransitionToNextLayer()
        {
            if (_currentLayer == LastLayer)
            {
                return;
            }

            _isTransitioning = true;
            _transitioningToNextLayer = true;
            _startTime = Time.time;

            _initialCameraPosition = Camera.main.transform.position;
            _finalCameraPosition = GetNewPosition(
                Camera.main.transform.position,
                _yDifferenceBetweenLayers,
                _zDifferenceBetweenLayers);

            _initialCharacterPosition = _characterMovementController.transform.position;
            _finalCharacterPosition = GetNewPosition(
                _characterMovementController.transform.position,
                _yDifferenceBetweenLayers,
                _zDifferenceBetweenLayers);

            _distance = Vector3.Distance(
                _initialCameraPosition,
                _finalCameraPosition);
        }

        public void TransitionToPreviousLayer()
        {
            if (_currentLayer == FirstLayer)
            {
                return;
            }

            _isTransitioning = true;
            _transitioningToNextLayer = false;
            _startTime = Time.time;

            _initialCameraPosition = Camera.main.transform.position;
            _finalCameraPosition = GetNewPosition(
                Camera.main.transform.position,
                - _yDifferenceBetweenLayers,
                - _zDifferenceBetweenLayers);

            _initialCharacterPosition = _characterMovementController.transform.position;
            _finalCharacterPosition = GetNewPosition(
                _characterMovementController.transform.position,
                -_yDifferenceBetweenLayers,
                -_zDifferenceBetweenLayers);

            _distance = Vector3.Distance(
                _initialCameraPosition,
                _finalCameraPosition);
        }

        private Vector3 GetNewPosition(Vector3 initialPosition, float yDifference, float zDifference)
        {
            return new Vector3(
                initialPosition.x,
                initialPosition.y + yDifference,
                initialPosition.z + zDifference);
        }
        private void SwitchLayer()
        {
            _currentLayer = _transitioningToNextLayer ? _currentLayer + 1 : _currentLayer - 1;

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
        }

        private void ApplyLayerToTentLayer(GameObject tentLayer, int layer)
        {
            tentLayer.layer = layer;
            foreach (Transform group in tentLayer.transform)
            {
                foreach (Transform transform in group.transform)
                {
                    transform.gameObject.layer = layer;
                }
            }
        }
    }
}
