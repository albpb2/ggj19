using Assets.Scripts.Objects;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts
{
    public class InputManager : MonoBehaviour
    {
        [SerializeField]
        private Plane _characterPlane;

        private Character _character;

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

        public Vector3? ClickedPoint { get; set; }

        public bool MoveRight { get; set; }

        public bool MoveLeft { get; set; }

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
                    if (hits[i].collider.gameObject.tag == "CharacterPlane")
                    {
                        ClickedPoint = hits[i].point;
                    }
                }
            }
        }
    }
}
