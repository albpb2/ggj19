using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Audio;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.StorageSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Player
{
    public class BagHandler : MonoBehaviour, IUIHideable
    {
        public const int MaxItems = 2;

        [SerializeField]
        private Bag _storageBag;
        [SerializeField]
        private Bag _dialogBag;
        [SerializeField]
        private GameObject _storageSpacePrefab;

        private StorageItemPrefabProvider _storageItemPrefabProvider;
        private GameManager _gameManager;
        private Character _character;
        private GameObject _bottleNotification;
        private SoundPlayer _oneShotSoundPlayer;

        public Bag ActiveBag { get; set; }

        public List<PortableObject> Items { get; set; } = new List<PortableObject>();

        public List<StorageSpace> Spaces { get; set; } = new List<StorageSpace>();

        public Image Image => ActiveBag.GetComponent<Image>();

        public bool WaterFull { get; private set; }

        public bool IsOpen => ActiveBag.gameObject.activeSelf;

        public void Start()
        {
            _storageItemPrefabProvider = FindObjectOfType<StorageItemPrefabProvider>();
            _gameManager = FindObjectOfType<GameManager>();
            _character = GetComponent<Character>();
            _bottleNotification = GameObject.FindGameObjectWithTag("water-fill-notification");
            _oneShotSoundPlayer = FindObjectOfType<SoundPlayer>();
            ActiveBag = _dialogBag;
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
            ActiveBag = _storageBag;
            OpenActiveBag();
        }

        public void OpenDialogBag()
        {
            ActiveBag = _dialogBag;
            OpenActiveBag();
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
            ActiveBag.CloseButton.gameObject.SetActive(false);
        }

        public void ShowCloseButton()
        {
            ActiveBag.CloseButton.gameObject.SetActive(true);
        }

        public void FillWater()
        {
            WaterFull = true;
            _storageBag.SetFullWaterSprite();
            _dialogBag.SetFullWaterSprite();
            _bottleNotification.GetComponent<Animator>().SetTrigger("fill");

        }

        public void GiveWaterToRefugee()
        {
            if (WaterFull)
            {
                WaterFull = false;
                _storageBag.SetEmptyWaterSprite();
                _dialogBag.SetEmptyWaterSprite();
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
            var storageSpace = Instantiate(_storageSpacePrefab, ActiveBag.transform).GetComponent<StorageSpace>();
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

        private void OpenActiveBag()
        {
            ActiveBag.gameObject.SetActive(true);
            _oneShotSoundPlayer.Play(Sound.OpenBag);
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
            return position == 0 ? ActiveBag.FirstItemPosition : ActiveBag.SecondItemPosition;
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
