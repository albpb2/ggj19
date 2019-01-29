using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.StorageSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class Bag : MonoBehaviour
    {
        private const float storageSpacesY = -10.03f;

        private readonly float[] storageSpacesX =
        {
            -37.1f, 2.83f
        };

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

        private StorageItemPrefabProvider _storageItemPrefabProvider;
        private GameManager _gameManager;
        private Character _character;

        public List<PortableObject> Items { get; set; } = new List<PortableObject>();

        public List<StorageSpace> Spaces { get; set; } = new List<StorageSpace>();

        public Image Image => _bagImage;

        public bool WaterFull { get; private set; }

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

        public void CloseBag()
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
            _bagImage.gameObject.SetActive(false);

            _gameManager.GameFreezed = false;
        }

        public void AddItem(StorageItem storageItem, StorageSpace storageSpace)
        {
            Items.Add(new PortableObject
            {
                Type = storageItem.PortableObjectType
            });

            storageItem.transform.SetParent(_bagImage.transform);
            storageItem.transform.localPosition = storageSpace.transform.localPosition;
            Spaces.Remove(storageSpace);
            Destroy(storageSpace.gameObject);
        }

        public bool DropStorageItem(StorageItem storageItem)
        {
            foreach (var storageSpace in Spaces)
            {
                if (Vector3.Distance(storageItem.transform.position, storageSpace.transform.position) < 20)
                {
                    AddItem(storageItem, storageSpace);
                    return true;
                }
            }

            return false;
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
            }
        }

        private void ShowStorageSpace(int position)
        {
            var storageSpace = Instantiate(_storageSpacePrefab, _bagImage.transform).GetComponent<StorageSpace>();
            storageSpace.transform.localPosition = new Vector3(
                storageSpacesX[position],
                storageSpacesY,
                1);
            storageSpace.Bag = this;
            storageSpace.SetStorage(null);
            Spaces.Add(storageSpace);
        }

        private void ShowStorageItem(PortableObject portableObject, int position)
        {
            var storageItem = StorageItem.Create(_storageItemPrefabProvider.GetPrefab(portableObject.Type), _bagImage);
            storageItem.transform.localPosition = new Vector3(
                storageSpacesX[position],
                storageSpacesY,
                1);
        }
    }
}
