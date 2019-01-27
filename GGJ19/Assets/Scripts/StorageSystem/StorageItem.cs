﻿using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.StorageSystem
{
    public class StorageItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField]
        private PortableObjectType _portableObjectType;

        private Button _button;
        private Storage _storage;
        private Vector3 _initialPosition;
        private Transform _initialParent;

        public PortableObjectType PortableObjectType => _portableObjectType;

        public static StorageItem Create(
            GameObject prefab,
            Image parentImage)
        {
            var storageItem = Instantiate(prefab, parentImage.transform).GetComponent<StorageItem>();

            return storageItem;
        }

        public void Start()
        {
            _button = GetComponent<Button>();
        }

        public void OnClick()
        {

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_storage == null)
            {
                CancelDrag();
            }

            var bag = FindObjectOfType<Bag>();
            if (bag.DropStorageItem(this))
            {
                _storage.RemoveItem(this, _initialPosition);
            }
            else
            {
                CancelDrag();
            }
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {

        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = new Vector3(
                eventData.position.x,
                eventData.position.y,
                2);
        }

        public void SetStorage(Storage storage)
        {
            _storage = storage;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _initialPosition = this.transform.localPosition;
            _initialParent = transform.parent;
            transform.SetParent(transform.parent.parent);
        }

        private void CancelDrag()
        {
            this.transform.SetParent(_initialParent);
            this.transform.localPosition = _initialPosition;
        }
    }
}
