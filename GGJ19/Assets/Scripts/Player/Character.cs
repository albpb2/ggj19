using System;
using Assets.Scripts.Objects;
using Assets.Scripts.Refugees;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private float _speed = 1;

        private Karma _karma;

        public PortableObject PortableObject { get; private set; }
        public InteractableSceneObject TargetObject { get; set; }

        public void Start()
        {
            _karma = GetComponent<Karma>();
        }

        public void Update()
        {
            if (TargetObject != null)
            {
                float step = _speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, TargetObject.transform.position, step);
            }
        }

        void OnTriggerEnter2D(Collider2D collider)
        {
            InteractWithTargetObjectIfIsOnTrigger(collider);
        }

        void OnTriggerStay2D(Collider2D collider)
        {
            InteractWithTargetObjectIfIsOnTrigger(collider);
        }

        public void TakePortableObject(PortableObject portableObject)
        {
            if (PortableObject == null)
            {
                PortableObject = portableObject;
            }
        }

        public void MoveTowards(InteractableSceneObject interactableSceneObject)
        {
            TargetObject = interactableSceneObject;
        }

        public void GiveObjectTo(Refugee refugee)
        {
            if (PortableObject != null)
            {
                PortableObject = null;
                _karma.Increment(5);
            }
        }

        private void InteractWithTargetObjectIfIsOnTrigger(Collider2D collider)
        {
            if (TargetObject != null && collider.gameObject == TargetObject.gameObject)
            {
                TargetObject.Interact();
                TargetObject = null;
            }
        }
    }
}
