using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extensions;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Assets.Scripts.StorageSystem
{
    public class Storage : MonoBehaviour, IUIHideable
    {
        [SerializeField]
        private int _maxCapacity;
        [SerializeField]
        private int _maxGiftCapacity;
        [SerializeField]
        private int _minCapacity;
        [SerializeField]
        private GameObject _storageSpacePrefab;
        [SerializeField]
        private Image _storeFront;
        [SerializeField]
        private Image _storeBack;
        [SerializeField]
        private Bag _bag;
        [SerializeField]
        private int _columns;
        [SerializeField]
        private float _spaceBetweenColumns;
        [SerializeField]
        private float _spaceBetweenRows;
        [SerializeField]
        private Vector2 _firstPosition;
        [SerializeField]
        private Vector2 _firstGiftPosition;
        [SerializeField]
        private List<GameObject> _storageItemPrefabs;
        [SerializeField]
        private float _bagDistanceToStorageInScreenPercentage = 0.3f;

        private List<GameObject> _selectedPrefabs;
        private List<StorageItem> _storageItems;
        private Random _random;
        private TimeTracker _timeTracker;
        private GameManager _gameManager;
        private Vector3 _bagOriginalPlace;
        private StorageItemPrefabProvider _prefabProvider;

        public StorageItem SelectedItem { get; set; }

        public List<PortableObjectType> Gifts { get; set; } = new List<PortableObjectType>();

        public void Start()
        {
            _random = new Random();
            _selectedPrefabs = new List<GameObject>();
            _timeTracker = FindObjectOfType<TimeTracker>();
            _gameManager = FindObjectOfType<GameManager>();
            _prefabProvider = FindObjectOfType<StorageItemPrefabProvider>();
            _bagOriginalPlace = _bag.Image.transform.localPosition;

            _timeTracker.onNewDayBegun += Refill;

            Refill(1);

            AddGift(GenerateRandomGift());
            AddGift(GenerateRandomGift());
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.Escape) && _storeBack.gameObject.activeSelf)
            {
                CloseStorage();
            }
        }

        public void Refill(int dayNumber)
        {
            if (_storeBack.gameObject.activeSelf)
            {
                CloseStorage();
                OpenStorage();
            }
            var numberOfObjects = _random.Next(_minCapacity, _maxCapacity + 1);
            for (var i = _selectedPrefabs.Count; i < numberOfObjects; i++)
            {
                var randomObject = _random.Next(0, _storageItemPrefabs.Count);
                _selectedPrefabs.Add(_storageItemPrefabs[randomObject]);
            }
        }

        public void Show()
        {
            var currentColumn = 0;
            var currentX = _firstPosition.x;
            var currentY = _firstPosition.y;
            _storageItems = new List<StorageItem>();

            foreach (var selectedPrefab in _selectedPrefabs)
            {
                if (_storageItems.Count == _maxCapacity)
                {
                    break;
                }

                var storageItem = StorageItem.Create(selectedPrefab, _storeFront);
                storageItem.transform.localPosition = new Vector3(currentX, currentY, 1);
                storageItem.SetStorage(this);
                _storageItems.Add(storageItem);
                currentX += _spaceBetweenColumns;
                currentColumn++;
                if (currentColumn == _columns)
                {
                    currentColumn = 0;
                    currentX = _firstPosition.x;
                    currentY -= _spaceBetweenRows;
                }
            }

            for (var i = _storageItems.Count; i < _maxCapacity; i++)
            {
                var storageSpace = Instantiate(_storageSpacePrefab, _storeFront.transform).GetComponent<StorageSpace>();
                storageSpace.transform.localPosition = new Vector3(currentX, currentY, 1);
                currentX += _spaceBetweenColumns;
                currentColumn++;
                if (currentColumn == _columns)
                {
                    currentColumn = 0;
                    currentX = _firstPosition.x;
                    currentY -= _spaceBetweenRows;
                }
            }

            currentX = _firstGiftPosition.x;
            currentY = _firstGiftPosition.y;
            var displayedGifts = 0;
            foreach (var gift in Gifts)
            {
                if (displayedGifts >= _maxGiftCapacity)
                {
                    break;
                }

                var storageItem = StorageItem.Create(_prefabProvider.GetPrefab(gift), _storeFront);
                storageItem.transform.localPosition = new Vector3(currentX, currentY, 1);
                storageItem.SetStorage(this);
                _storageItems.Add(storageItem);
                currentX += _spaceBetweenColumns;
                currentColumn++;
                if (currentColumn == _columns)
                {
                    currentColumn = 0;
                    currentX = _firstPosition.x;
                    currentY -= _spaceBetweenRows;
                }
            }
        }

        public void OpenStorage()
        {
            if (IsOpen())
            {
                return;
            }

            _gameManager.GameFreezed = true;

            _storeBack.gameObject.SetActive(true);
            _storeFront.gameObject.SetActive(true);
            _bag.OpenBag();
            _bag.HideCloseButton();

            _bag.Image.transform.localPosition =
                _storeFront.transform.localPosition.AddX((float)Screen.width * _bagDistanceToStorageInScreenPercentage);

            Show();
        }

        public void CloseStorage()
        {
            if (!IsOpen())
            {
                return;
            }

            _gameManager.GameFreezed = false;

            foreach (Transform child in _storeFront.transform)
            {
                Destroy(child.gameObject);
            }

            _storeBack.gameObject.SetActive(false);
            _storeFront.gameObject.SetActive(false);
            _bag.ShowCloseButton();
            _bag.Image.transform.localPosition = _bagOriginalPlace;
            _bag.CloseBag();

            _storageItems = new List<StorageItem>();
        }

        public void RemoveItem(StorageItem storageItem, Vector3 localPosition)
        {
            var storageSpace = Instantiate(_storageSpacePrefab, _storeFront.transform).GetComponent<StorageSpace>();
            storageSpace.transform.localPosition = localPosition;

            _storageItems.Remove(storageItem);
            var selectedPrefab = _selectedPrefabs.FirstOrDefault(p =>
                p.GetComponent<StorageItem>().PortableObjectType == storageItem.PortableObjectType);
            if (selectedPrefab != null)
            {
                _selectedPrefabs.Remove(selectedPrefab);
            }
            else
            {
                Gifts.Remove(Gifts.First(gift => gift == storageItem.PortableObjectType));
            }
        }

        public void AddGift(PortableObjectType objectType)
        {
            Gifts.Add(objectType);
        }

        public void HideUIElement()
        {
            if (IsOpen())
            {
                CloseStorage();
            }
        }

        private bool IsOpen()
        {
            return _storeBack.gameObject.activeSelf;
        }

        private PortableObjectType GenerateRandomGift()
        {
            return Objects.InteractableSceneObjects.Gifts.GetGiftPortableObjectTypes().GetRandomElement();
        }
    }
}
