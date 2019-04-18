using Assets.Scripts.Extensions;
using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.Player;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.StorageSystem
{
    public class StorageItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [SerializeField]
        private PortableObjectType _portableObjectType;

        private Button _button;
        private Storage _storage;
        private BagHandler _bag;
        private Vector3 _initialPosition;
        private Transform _initialParent;
        private Character _character;

        public PortableObjectType PortableObjectType => _portableObjectType;

        public bool FromStorage;

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
            _character = FindObjectOfType<Character>();
            _bag = _character.GetComponent<BagHandler>();
            _storage = FindObjectOfType<Storage>();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (FromStorage)
            {
                DropFromStorage();
            }
            else
            {
                DropFromBag();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = new Vector3(
                eventData.position.x,
                eventData.position.y,
                2);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _initialPosition = this.transform.localPosition;
            _initialParent = transform.parent;
            transform.SetParent(transform.parent.parent);
            FromStorage = PointerOnStorage();
        }

        private void CancelDrag()
        {
            this.transform.SetParent(_initialParent);
            this.transform.localPosition = _initialPosition;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (IsInGiveMode())
            {
                _character.GiveObjectToRefugee(PortableObjectType);
                Destroy(gameObject);
                _bag.CloseBag();
                _character.EndInteraction();
            }
        }

        private bool IsInGiveMode()
        {
            return !_storage.IsOpen && _character.InteractingWith != null;
        }

        private bool PointerOnBag()
        {
            Rect bagRect = _bag.Image.GetComponent<RectTransform>().GetScreenSpaceRect();

            return bagRect.Contains(Input.mousePosition);
        }

        private bool PointerOnStorage()
        {
            Rect storageRect = _storage.Front.GetComponent<RectTransform>().GetScreenSpaceRect();

            return storageRect.Contains(Input.mousePosition);
        }

        private void DropFromStorage()
        {
            if (PointerOnBag() && _bag.Spaces.Any())
            {
                _bag.AddItem(this);
                _storage.RemoveItem(this);
                Destroy(this.gameObject);
            }
            else
            {
                CancelDrag();
            }
        }

        private void DropFromBag()
        {
            if (PointerOnStorage() && _storage.HasRoomForStorageItem(this))
            {
                _storage.AddItem(this);
                _bag.RemoveItem(this);
                Destroy(this.gameObject);
            }
            else
            {
                CancelDrag();
            }
        }
    }
}
