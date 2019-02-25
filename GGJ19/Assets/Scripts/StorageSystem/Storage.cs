using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extensions;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.Player;
using Assets.Scripts.Refugees;
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
        private GameObject _breadItemPrefab;
        [SerializeField]
        private GameObject _bottleItemPrefab;
        [SerializeField]
        private GameObject _coatItemPrefab;
        [SerializeField]
        private GameObject _pillItemPrefab;
        [SerializeField]
        private float _bagDistanceToStorageInScreenPercentage = 0.3f;
        [SerializeField]
        private RefugeesSettings _refugeesSettings;
        
        private List<GameObject> _selectedPrefabs;
        private List<StorageItem> _storageItems;
        private List<GameObject> _itemPrefabs;
        private Random _random;
        private TimeTracker _timeTracker;
        private GameManager _gameManager;
        private Vector3 _bagOriginalPlace;
        private StorageItemPrefabProvider _prefabProvider;

        public StorageItem SelectedItem { get; set; }

        public List<PortableObjectType> Gifts { get; set; } = new List<PortableObjectType>();

        public Image Front => _storeFront;

        public bool HasFreeItemsSpace => _storageItems.Count < _maxCapacity;

        public bool HasFreeGiftsSpace => Gifts.Count < _maxGiftCapacity;

        public bool IsOpen => _storeBack.gameObject.activeSelf;

        public void Start()
        {
            _random = new Random();
            _selectedPrefabs = new List<GameObject>();
            _timeTracker = FindObjectOfType<TimeTracker>();
            _gameManager = FindObjectOfType<GameManager>();
            _prefabProvider = FindObjectOfType<StorageItemPrefabProvider>();
            _bagOriginalPlace = _bag.Image.transform.localPosition;
            _itemPrefabs = new List<GameObject>
            {
                _breadItemPrefab, _bottleItemPrefab, _coatItemPrefab, _pillItemPrefab
            };

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
                AddRandomItemToStorage();
            }
        }

        public void OpenStorage()
        {
            if (IsOpen)
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

            PaintItems();
        }

        public void CloseStorage()
        {
            if (!IsOpen)
            {
                return;
            }

            _gameManager.GameFreezed = false;

            ClearItems();

            _storeBack.gameObject.SetActive(false);
            _storeFront.gameObject.SetActive(false);
            _bag.ShowCloseButton();
            _bag.Image.transform.localPosition = _bagOriginalPlace;
            _bag.CloseBag();

            _storageItems = new List<StorageItem>();
        }

        public bool HasRoomForStorageItem(StorageItem item)
        {
            return item.PortableObjectType.IsOfGiftType() ?
                HasFreeGiftsSpace : HasFreeItemsSpace;
        }

        public void AddItem(StorageItem item)
        {
            if (item.PortableObjectType.IsOfGiftType())
            {
                if (HasFreeGiftsSpace)
                {
                    Gifts.Add(item.PortableObjectType);
                }
            }
            else
            {
                if (HasFreeItemsSpace)
                {
                    var prefabToAdd = _itemPrefabs.FirstOrDefault(p =>
                        p.GetComponent<StorageItem>().PortableObjectType == item.PortableObjectType);
                    _selectedPrefabs.Add(prefabToAdd);
                }
            }
            ClearItems();
            PaintItems();
        }

        public void RemoveItem(StorageItem storageItem)
        {
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
            ClearItems();
            PaintItems();
        }

        public void AddGift(PortableObjectType objectType)
        {
            Gifts.Add(objectType);
        }

        public void HideUIElement()
        {
            if (IsOpen)
            {
                CloseStorage();
            }
        }

        private void AddRandomItemToStorage()
        {
            var totalProbability = _refugeesSettings.HungerItemProbability
                                   + _refugeesSettings.ThirstResolvedPoints
                                   + _refugeesSettings.ColdItemProbability
                                   + _refugeesSettings.IllnessItemProbability;

            var number = _random.Next(0, 100);
            var hungerItemProbability = _refugeesSettings.HungerItemProbability * 100 / totalProbability;
            var thirstItemProbability = _refugeesSettings.ThirstItemProbability * 100 / totalProbability
                                        + hungerItemProbability;
            var coldItemProbability = _refugeesSettings.ColdItemProbability * 100 / totalProbability
                                      + thirstItemProbability;

            if (number < hungerItemProbability)
            {
                _selectedPrefabs.Add(_breadItemPrefab);
            }
            else if (number < thirstItemProbability)
            {
                _selectedPrefabs.Add(_bottleItemPrefab);
            }
            else if (number < coldItemProbability)
            {
                _selectedPrefabs.Add(_coatItemPrefab);
            }
            else
            {
                _selectedPrefabs.Add(_pillItemPrefab);
            }
        }

        private void PaintItems()
        {
            PaintStorageItems();
            PaintGifts();
        }

        private void PaintStorageItems()
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
        }

        private void PaintGifts()
        {
            var currentColumn = 0;
            var currentX = _firstGiftPosition.x;
            var currentY = _firstGiftPosition.y;
            var displayedGifts = 0;

            foreach (var gift in Gifts)
            {
                if (displayedGifts >= _maxGiftCapacity)
                {
                    break;
                }

                var storageItem = StorageItem.Create(_prefabProvider.GetPrefab(gift), _storeFront);
                storageItem.transform.localPosition = new Vector3(currentX, currentY, 1);
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

        private PortableObjectType GenerateRandomGift()
        {
            return Objects.InteractableSceneObjects.Gifts.GetGiftPortableObjectTypes().GetRandomElement();
        }

        private void ClearItems()
        {
            foreach (Transform child in _storeFront.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
}
