using Assets.Scripts.Objects;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class CharacterMovementController : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 1;

        private InputManager _inputManager;
        private Character _character;
        private Vector3? _targetDirection;
        private GameObject _targetPlaneSwitch;

        public InteractableSceneObject TargetObject { get; set; }
        
        public void Start()
        {
            _inputManager = FindObjectOfType<InputManager>();
            _character = FindObjectOfType<Character>();
        }

        public void Update()
        {
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
                _targetPlaneSwitch = null;
            }
            else if (TargetObject != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, TargetObject.transform.position, step);
            }
            else if (_targetPlaneSwitch != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, GetTargetWithXPosition(_targetPlaneSwitch.transform.position.x), step);
            }

            ProcessClickedPlaneSwitchTrigger();
        }

        private void MoveTowardsDirection(Vector3 direction, float step)
        {
            transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, step);
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            InteractWithTargetObjectIfIsOnTrigger(collider);
            InteractWithTargetPlaneSwitchIfIsOnTrigger(collider);
        }

        void OnTriggerStay2D(Collider2D collider)
        {
            InteractWithTargetObjectIfIsOnTrigger(collider);
        }

        public void MoveTowards(InteractableSceneObject interactableSceneObject)
        {
            TargetObject = interactableSceneObject;
            _targetDirection = null;
            _targetPlaneSwitch = null;
        }

        private void InteractWithTargetObjectIfIsOnTrigger(Collider2D collider)
        {
            if (TargetObject != null && collider.gameObject == TargetObject.gameObject)
            {
                TargetObject.Interact();
                TargetObject = null;
            }
        }
        private void InteractWithTargetPlaneSwitchIfIsOnTrigger(Collider2D collider)
        {
            if (_targetPlaneSwitch != null && collider.gameObject == _targetPlaneSwitch)
            {
                Debug.Log("Switch reached");
            }
        }

        private bool IsCloseHorizontally(float x1, float x2)
        {
            return Mathf.Abs(x1 - x2) < 0.1f;
        }

        private void ProcessClickedPlaneSwitchTrigger()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var hits = Physics.RaycastAll(ray, 100);

                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.gameObject.tag == Tags.PlaneSwitchTrigger)
                    {
                        _targetPlaneSwitch = hits[i].collider.gameObject;
                        _targetDirection = null;
                        TargetObject = null;
                    }
                }
            }
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
