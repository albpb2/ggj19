using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Objects;
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

        private StorageItemPrefabProvider _storageItemPrefabProvider;
        private GameManager _gameManager;

        public List<PortableObject> Items { get; set; } = new List<PortableObject>();
        public List<StorageSpace> Spaces { get; set; } = new List<StorageSpace>();

        public Image Image => _bagImage;

        public void Start()
        {
            _storageItemPrefabProvider = FindObjectOfType<StorageItemPrefabProvider>();
            _gameManager = FindObjectOfType<GameManager>();
        }

        public void Update()
        {
            if (Input.GetKey(KeyCode.Escape))
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
                if (child.gameObject != _closeButton.gameObject)
                {
                    Destroy(child.gameObject);
                }
            }
            _bagImage.gameObject.SetActive(false);

            _gameManager.Pause = false;
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
