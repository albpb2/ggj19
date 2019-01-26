using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private Plane _characterPlane;
        [SerializeField]
        public Texture2D _selectedCursorTexture;

        private Character _character;
        private CursorMode _cursorMode = CursorMode.Auto;
        private Vector2 _hotSpot = Vector2.zero;
        private Texture2D _defaultCursor;

        public Vector3? ClickedPoint { get; set; }

        public bool MoveRight { get; set; }

        public bool MoveLeft { get; set; }

        public void Start()
        {
            _character = FindObjectOfType<Character>();
        }

        public void Update()
        {
            ResetValues();

            DetectMovementClick();

            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                MoveRight = true;
            }
            else if(Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                MoveLeft = true;
            }
        }

        public void SetSelectableCursor()
        {
            Cursor.SetCursor(_selectedCursorTexture, _hotSpot, _cursorMode);
        }

        public void SetDefaultCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, _cursorMode);
        }

        private void ResetValues()
        {
            ClickedPoint = null;
            MoveRight = false;
            MoveLeft = false;
        }

        private void DetectMovementClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var hits = Physics.RaycastAll(ray, 100);

                for (int i = 0; i < hits.Length; i++)
                {
                    if (hits[i].collider.gameObject.tag == Tags.CharacterPlane)
                    {
                        ClickedPoint = hits[i].point;
                    }
                }
            }
        }
    }
}
