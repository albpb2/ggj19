using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Assets.Scripts.StorageSystem
{
    public class Storage : MonoBehaviour
    {
        [SerializeField]
        private int _maxCapacity;
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
        private List<GameObject> _storageItemPrefabs;

        private List<GameObject> _selectedPrefabs;
        private List<StorageItem> _storageItems;
        private Random _random;
        private TimeTracker _timeTracker;
        private GameManager _gameManager;
        private Vector3 _bagOriginalPlace;

        public StorageItem SelectedItem { get; set; }

        public void Start()
        {
            _random = new Random();
            _selectedPrefabs = new List<GameObject>();
            _timeTracker = FindObjectOfType<TimeTracker>();
            _gameManager = FindObjectOfType<GameManager>();
            _bagOriginalPlace = _bag.Image.transform.localPosition;

            _timeTracker.onNewDayBegun += Refill;

            Refill(1);
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                CloseStorage();
            }
        }

        public void Refill(int dayNumber)
        {
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
        }

        public void OpenStorage()
        {
            _gameManager.Pause = true;

            _storeBack.gameObject.SetActive(true);
            _storeFront.gameObject.SetActive(true);
            _bag.OpenBag();
            _bag.HideCloseButton();
            _bag.Image.transform.localPosition = _storeFront.transform.localPosition + new Vector3(300, 0, 0);

            Show();
        }

        public void CloseStorage()
        {
            _gameManager.Pause = false;

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
            _selectedPrefabs.Remove(_selectedPrefabs.First(p =>
                p.GetComponent<StorageItem>().PortableObjectType == storageItem.PortableObjectType));
        }
    }
}
