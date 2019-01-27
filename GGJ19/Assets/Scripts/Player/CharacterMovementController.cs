﻿using Assets.Scripts.Objects;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class CharacterMovementController : MonoBehaviour
    {
        private const string WalkAnimationName = "walk";

        [SerializeField]
        private float _speed = 1;
        [SerializeField]
        private float _minX = -11.53f;
        [SerializeField]
        private float _maxX = 45.48f;

        private InputManager _inputManager;
        private Character _character;
        private GameManager _gameManager;
        private Vector3? _targetDirection;
        private Vector3? _previousPosition;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;

        public InteractableSceneObject TargetObject { get; set; }
        
        public void Start()
        {
            _inputManager = FindObjectOfType<InputManager>();
            _character = FindObjectOfType<Character>();
            _gameManager = FindObjectOfType<GameManager>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Update()
        {
            if (_gameManager.Pause)
            {
                return;
            }

            float step = _speed * Time.deltaTime;
            
            if (_inputManager.MoveLeft)
            {
                MoveTowardsDirection(Vector3.left, step);
                _targetDirection = null;
                TargetObject = null;
            }
            else if (_inputManager.MoveRight)
            {
                MoveTowardsDirection(Vector3.right, step);
                _targetDirection = null;
                TargetObject = null;
            }
            else if (_inputManager.ClickedPoint.HasValue)
            {
                _targetDirection = new Vector3(
                    _inputManager.ClickedPoint.Value.x,
                    transform.position.y,
                    transform.position.z);
            }

            if (_targetDirection.HasValue)
            {
                transform.position = Vector3.MoveTowards(transform.position, _targetDirection.Value, step);
                if (IsCloseHorizontally(transform.position.x, _targetDirection.Value.x))
                {
                    _targetDirection = null;
                }

                TargetObject = null;
            }
            else if (TargetObject != null)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    GetTargetWithXPosition(TargetObject.transform.position.x),
                    step);
            }
        }

        public void FixedUpdate()
        {
            if (_previousPosition.HasValue && transform.position.x != _previousPosition.Value.x)
            {
                _animator.SetBool(WalkAnimationName, true);
                if (transform.position.x > _previousPosition.Value.x)
                {
                    _spriteRenderer.flipX = false;
                }
                else
                {
                    _spriteRenderer.flipX = true;
                }
            }
            else
            {
                _animator.SetBool(WalkAnimationName, false);
            }

            _previousPosition = transform.position;
            transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        private void MoveTowardsDirection(Vector3 direction, float step)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, step);
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            InteractWithTargetObjectIfIsOnTrigger(collider);
        }

        void OnTriggerStay2D(Collider2D collider)
        {
            InteractWithTargetObjectIfIsOnTrigger(collider);
        }

        public void MoveTowards(InteractableSceneObject interactableSceneObject)
        {
            TargetObject = interactableSceneObject;
            _targetDirection = null;
            _inputManager.OverrideThisFrame();
        }

        private void InteractWithTargetObjectIfIsOnTrigger(Collider2D collider)
        {
            if (TargetObject != null && collider.gameObject == TargetObject.gameObject)
            {
                TargetObject.Interact();
                _targetDirection = GetTargetWithXPosition(TargetObject.transform.position.x);
                TargetObject = null;
            }
        }

        private bool IsCloseHorizontally(float x1, float x2)
        {
            return Mathf.Abs(x1 - x2) < 0.1f;
        }

        private Vector3 GetTargetWithXPosition(float xPosition)
        {
            return new Vector3(
                xPosition,
                transform.position.y,
                transform.position.z);
        }
    }
}
