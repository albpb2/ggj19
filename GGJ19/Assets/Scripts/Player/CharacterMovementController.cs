using Assets.Scripts.Objects;
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

        private InputManager _inputManager;
        private Character _character;
        private GameManager _gameManager;
        private Vector3? _targetDirection;
        private Vector3? _previousPosition;
        private Animator _animator;
        private SpriteRenderer _spriteRenderer;
        private bool _allowVerticalMovementUp;
        private bool _allowVerticalMovementDown;
        private bool _allowHorizontalMovementLeft;
        private bool _allowHorizontalMovementRight;

        public InteractableSceneObject TargetObject { get; set; }
        
        public void Start()
        {
            _inputManager = FindObjectOfType<InputManager>();
            _character = FindObjectOfType<Character>();
            _gameManager = FindObjectOfType<GameManager>();
            _animator = GetComponent<Animator>();
            _spriteRenderer = GetComponent<SpriteRenderer>();

            _allowVerticalMovementUp = true;
            _allowVerticalMovementDown = true;
            _allowHorizontalMovementLeft = true;
            _allowHorizontalMovementRight = true;
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
                if (_allowHorizontalMovementLeft && _inputManager.MoveLeft)
                {
                    MoveTowardsDirection(Vector3.left, step);
                    _targetDirection = null;
                    TargetObject = null;
                }
                else if (_allowHorizontalMovementRight && _inputManager.MoveRight)
                {
                    MoveTowardsDirection(Vector3.right, step);
                    _targetDirection = null;
                    TargetObject = null;
                }
                if (_allowVerticalMovementUp && _inputManager.MoveUp)
                {
                    MoveTowardsDirection(Vector3.up, step);
                    _targetDirection = null;
                    TargetObject = null;
                }
                else if (_allowVerticalMovementDown && _inputManager.MoveDown)
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
            }

            _previousPosition = transform.position;
            transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            switch (collider.tag)
            {
                case Tags.VerticalTriggerUp:
                    _allowVerticalMovementUp = false;
                    break;
                case Tags.VerticalTriggerDown:
                    _allowVerticalMovementDown = false;
                    break;
                case Tags.HorizontalTriggerLeft:
                    _allowHorizontalMovementLeft = false;
                    break;
                case Tags.HorizontalTriggerRight:
                    _allowHorizontalMovementRight = false;
                    break;
                default:
                    break;
            }

            InteractWithTargetObjectIfIsOnTrigger(collider);
        }

        void OnTriggerStay2D(Collider2D collider)
        {
            switch (collider.tag)
            {
                case Tags.VerticalTriggerUp:
                    _allowVerticalMovementUp = false;
                    break;
                case Tags.VerticalTriggerDown:
                    _allowVerticalMovementDown = false;
                    break;
                case Tags.HorizontalTriggerLeft:
                    _allowHorizontalMovementLeft = false;
                    break;
                case Tags.HorizontalTriggerRight:
                    _allowHorizontalMovementRight = false;
                    break;
                default:
                    break;
            }
            InteractWithTargetObjectIfIsOnTrigger(collider);
        }

        void OnTriggerExit2D(Collider2D collider)
        {
            switch (collider.tag)
            {
                case Tags.VerticalTriggerUp:
                    _allowVerticalMovementUp = true;
                    break;
                case Tags.VerticalTriggerDown:
                    _allowVerticalMovementDown = true;
                    break;
                case Tags.HorizontalTriggerLeft:
                    _allowHorizontalMovementLeft = true;
                    break;
                case Tags.HorizontalTriggerRight:
                    _allowHorizontalMovementRight = true;
                    break;
                default:
                    break;
            }
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
