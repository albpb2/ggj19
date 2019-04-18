using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.StorageSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class Bag : MonoBehaviour, IUIHideable
    {
        public const int MaxItems = 2;

        [SerializeField]
        private Image _storageBagImage;
        [SerializeField]
        private Image _dialogBagImage;
        [SerializeField]
        private GameObject _storageSpacePrefab;
        [SerializeField]
        private Image _closeButton;
        [SerializeField]
        private Sprite _storageBagFullWaterSprite;
        [SerializeField]
        private Sprite _storageBagEmptyWaterSprite;
        [SerializeField]
        private Sprite _dialogBagFullWaterSprite;
        [SerializeField]
        private Sprite _dialogBagEmptyWaterSprite;
        [SerializeField]
        private Vector2 _firstStorageSpacePosition = new Vector2(-142, -41);
        [SerializeField]
        private Vector2 _secondStorageSpacePosition = new Vector2(17, -30);
        [SerializeField]
        private Vector2 _firstDialogSpacePosition = new Vector2(-142, -41);
        [SerializeField]
        private Vector2 _secondDialogSpacePosition = new Vector2(17, -30);

        private StorageItemPrefabProvider _storageItemPrefabProvider;
        private GameManager _gameManager;
        private Character _character;
        private GameObject _bottleNotification;

        public List<PortableObject> Items { get; set; } = new List<PortableObject>();

        public List<StorageSpace> Spaces { get; set; } = new List<StorageSpace>();

        public Image Image => _storageBagImage.gameObject.activeSelf ? _storageBagImage : _dialogBagImage;

        public bool WaterFull { get; private set; }

        public bool IsOpen => _storageBagImage.gameObject.activeSelf || _dialogBagImage.gameObject.activeSelf;

        public void Start()
        {
            _storageItemPrefabProvider = FindObjectOfType<StorageItemPrefabProvider>();
            _gameManager = FindObjectOfType<GameManager>();
            _character = GetComponent<Character>();
            _bottleNotification = GameObject.FindGameObjectWithTag("water-fill-notification");

        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.Escape) && Image.gameObject.activeSelf)
            {
                CloseBag();
            }
        }

        public void OpenStorageBag()
        {
            _storageBagImage.gameObject.SetActive(true);

            PaintItems();
        }

        public void OpenDialogBag()
        {
            _dialogBagImage.gameObject.SetActive(true);

            PaintItems();
        }

        public void CloseBag()
        {
            ClearBag();
            Image.gameObject.SetActive(false);
        }

        public void AddItem(StorageItem storageItem)
        {
            Items.Add(new PortableObject
            {
                Type = storageItem.PortableObjectType
            });
            var storageSpace = Spaces.First();
            Spaces.Remove(storageSpace);

            ClearBag();
            PaintItems();
        }

        public void PlaceAt(Vector3 position)
        {
            transform.localPosition = position;
        }

        public void HideCloseButton()
        {
            _closeButton.gameObject.SetActive(false);
        }

        public void ShowCloseButton()
        {
            _closeButton.gameObject.SetActive(true);
        }

        public void FillWater()
        {
            WaterFull = true;
            _storageBagImage.GetComponent<Image>().sprite = _storageBagFullWaterSprite;
            _dialogBagImage.GetComponent<Image>().sprite = _dialogBagFullWaterSprite;
            _bottleNotification.GetComponent<Animator>().SetTrigger("fill");

        }

        public void GiveWaterToRefugee()
        {
            if (WaterFull)
            {
                WaterFull = false;
                _storageBagImage.GetComponent<Image>().sprite = _storageBagEmptyWaterSprite;
                _dialogBagImage.GetComponent<Image>().sprite = _dialogBagEmptyWaterSprite;
                _character.GiveObjectToRefugee(PortableObjectType.Water);
                CloseBag();
            }
        }

        public void HideUIElement()
        {
            if (IsOpen)
            {
                CloseBag();
            }
        }

        private void ShowStorageSpace(int position)
        {
            var storageSpace = Instantiate(_storageSpacePrefab, _storageBagImage.transform).GetComponent<StorageSpace>();
            var localPosition = GetSpacePosition(position);
            storageSpace.transform.localPosition = new Vector3(
                localPosition.x,
                localPosition.y,
                1);
            storageSpace.Bag = this;
            storageSpace.SetStorage(null);
            Spaces.Add(storageSpace);
        }

        public void RemoveItem(StorageItem item)
        {
            Items.Remove(Items.First(i => i.Type == item.PortableObjectType));
            ClearBag();
            PaintItems();
        }

        private void PaintItems()
        {
            if (Items.Count == 0)
            {
                ShowStorageSpace(0);
                ShowStorageSpace(1);
            }
            else if (Items.Count == 1)
            {
                ShowStorageItem(Items.First(), 0);
                ShowStorageSpace(1);
            }
            else
            {
                ShowStorageItem(Items.First(), 0);
                ShowStorageItem(Items.Last(), 1);
            }
        }

        private void ShowStorageItem(PortableObject portableObject, int position)
        {
            var storageItem = StorageItem.Create(_storageItemPrefabProvider.GetPrefab(portableObject.Type), Image);
            var localPosition = GetSpacePosition(position);
            storageItem.transform.localPosition = new Vector3(
                localPosition.x,
                localPosition.y,
                1);
        }

        private Vector2 GetSpacePosition(int position)
        {
            if (_storageBagImage.gameObject.activeSelf)
            {
                return position == 0 ? _firstStorageSpacePosition : _secondStorageSpacePosition;
            }

            return position == 0 ? _firstDialogSpacePosition : _secondDialogSpacePosition;
        }

        private void ClearBag()
        {
            foreach (Transform child in Image.transform)
            {
                if (child.gameObject.GetComponent<StorageItem>() != null)
                {
                    Destroy(child.gameObject);
                }
                else if (child.gameObject.GetComponent<StorageSpace>() != null)
                {
                    Destroy(child.gameObject);
                    Spaces.Remove(child.gameObject.GetComponent<StorageSpace>());
                }
            }
        }
    }
}
