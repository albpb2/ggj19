using Assets.Scripts.Objects;
using FMODUnity;
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
        [SerializeField]
        private float _minY = 3.3f;
        [SerializeField]
        private float _maxY = 12.6f;
        [SerializeField]
        private Feet _feet;
        [SerializeField]
        private StudioEventEmitter _fmodStudioEventEmitter;

        private InputManager _inputManager;
        private Character _character;
        private GameManager _gameManager;
        private Vector3? _targetDirection;
        private Vector3? _previousPosition;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private Vector3 _initialPosition;
        private Vector3 _initialScale;

        public InteractableSceneObject TargetObject { get; set; }
        
        public void Start()
        {
            _inputManager = FindObjectOfType<InputManager>();
            _character = FindObjectOfType<Character>();
            _gameManager = FindObjectOfType<GameManager>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _initialPosition = transform.position;
            _initialScale = transform.localScale;
        }

        public void Update()
        {
            if (_gameManager.GameFreezed)
            {
                return;
            }

            float step = _speed * Time.deltaTime;

            if (_inputManager.MoveWithKeys)
            {
                if (_feet.AllowHorizontalMovementLeft && _inputManager.MoveLeft)
                {
                    MoveTowardsDirection(Vector3.left, step);
                    _targetDirection = null;
                    TargetObject = null;
                }
                else if (_feet.AllowHorizontalMovementRight && _inputManager.MoveRight)
                {
                    MoveTowardsDirection(Vector3.right, step);
                    _targetDirection = null;
                    TargetObject = null;
                }
                if (_feet.AllowVerticalMovementUp && _inputManager.MoveUp)
                {
                    MoveTowardsDirection(Vector3.up, step);
                    _targetDirection = null;
                    TargetObject = null;
                }
                else if (_feet.AllowVerticalMovementDown && _inputManager.MoveDown)
                {
                    MoveTowardsDirection(Vector3.down, step);
                    _targetDirection = null;
                    TargetObject = null;
                }
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
            if (IsMoving())
            {
                _animator.SetBool(WalkAnimationName, true);
                StartStepsSound();

                if (_inputManager.MoveLeft)
                {
                    _spriteRenderer.flipX = true;
                }
                else if (_inputManager.MoveRight)
                {
                    _spriteRenderer.flipX = false;
                }
                else if(transform.position.x > _previousPosition.Value.x)
                {
                    _spriteRenderer.flipX = false;
                }
                else if (transform.position.x < _previousPosition.Value.x)
                {
                    _spriteRenderer.flipX = true;
                }
                
            }
            else
            {
                _animator.SetBool(WalkAnimationName, false);
                StopStepsSound();
            }

            _previousPosition = transform.position;
            transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        public void ResetInitialPositionAndScale()
        {
            transform.position = _initialPosition;
            transform.localScale = _initialScale;
            _spriteRenderer.flipX = false;
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

        private void MoveTowards(Vector3 direction, float step)
        {
            transform.position = Vector3.MoveTowards(transform.position, direction, step);
        }

        private void MoveTowardsDirection(Vector3 direction, float step)
        {
            var targetPosition = FixTargetPosition(transform.position + direction);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
        }

        private Vector3 FixTargetPosition(Vector3 targetPosition)
        {
            (float x, float y, float z) = (targetPosition.x, targetPosition.y, targetPosition.z);
            if (targetPosition.y > _maxY)
            {
                y = _maxY;
            }
            else if (targetPosition.y < _minY)
            {
                y = _minY;
            }
            return new Vector3(x, y, z);
        }

        private bool IsMoving()
        {
            return !_gameManager.GameFreezed && _previousPosition.HasValue &&
                (TargetObject != null || 
                 _inputManager.MoveLeft || 
                 _inputManager.MoveRight ||
                 _inputManager.MoveUp ||
                 _inputManager.MoveDown ||
                 _targetDirection != null);
        }

        private void InteractWithTargetObjectIfIsOnTrigger(Collider2D collider)
        {
            if (TargetObject != null && collider.gameObject == TargetObject.gameObject)
            {
                TargetObject.Interact();
                _targetDirection = null;
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

        private void StartStepsSound()
        {
            _fmodStudioEventEmitter.gameObject.SetActive(true);
        }

        private void StopStepsSound()
        {
            _fmodStudioEventEmitter.gameObject.SetActive(false);
        }
    }
}
