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
        private Image _bagImage;
        [SerializeField]
        private GameObject _storageSpacePrefab;
        [SerializeField]
        private Image _closeButton;
        [SerializeField]
        private Sprite _waterFullSprite;
        [SerializeField]
        private Sprite _waterEmptySprite;
        [SerializeField]
        private Vector2 _firstSpacePosition = new Vector2(-142, -41);
        [SerializeField]
        private Vector2 _secondSpacePosition = new Vector2(17, -30);

        private StorageItemPrefabProvider _storageItemPrefabProvider;
        private GameManager _gameManager;
        private Character _character;

        public List<PortableObject> Items { get; set; } = new List<PortableObject>();

        public List<StorageSpace> Spaces { get; set; } = new List<StorageSpace>();

        public Image Image => _bagImage;

        public bool WaterFull { get; private set; }

        public bool IsOpen => _bagImage?.gameObject.activeSelf ?? false;

        public void Start()
        {
            _storageItemPrefabProvider = FindObjectOfType<StorageItemPrefabProvider>();
            _gameManager = FindObjectOfType<GameManager>();
            _character = GetComponent<Character>();
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.Escape) && _bagImage.gameObject.activeSelf)
            {
                CloseBag();
            }
        }

        public void OpenBag()
        {
            _bagImage.gameObject.SetActive(true);

            PaintItems();
        }

        public void CloseBag()
        {
            ClearBag();
            _bagImage.gameObject.SetActive(false);

            _gameManager.GameFreezed = false;
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
            _bagImage.GetComponent<Image>().sprite = _waterFullSprite;
        }

        public void GiveWaterToRefugee()
        {
            if (WaterFull)
            {
                WaterFull = false;
                _bagImage.GetComponent<Image>().sprite = _waterEmptySprite;
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
            var storageSpace = Instantiate(_storageSpacePrefab, _bagImage.transform).GetComponent<StorageSpace>();
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
            var storageItem = StorageItem.Create(_storageItemPrefabProvider.GetPrefab(portableObject.Type), _bagImage);
            var localPosition = GetSpacePosition(position);
            storageItem.transform.localPosition = new Vector3(
                localPosition.x,
                localPosition.y,
                1);
        }

        private Vector2 GetSpacePosition(int position)
        {
            return position == 0 ? _firstSpacePosition : _secondSpacePosition;
        }

        private void ClearBag()
        {
            foreach (Transform child in _bagImage.transform)
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
