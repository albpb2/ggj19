using Assets.Scripts.Objects;
using Assets.Scripts.Objects.PortableObjects;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.StorageSystem
{
    public class StorageItem : MonoBehaviour
    {
        [SerializeField]
        private PortableObjectType _portableObjectType;

        private Button _button;
        private Storage _storage;

        public static StorageItem Create(
            GameObject prefab,
            Image storeFront,
            Storage storage)
        {
            var storageItem = Instantiate(prefab, storeFront.transform).GetComponent<StorageItem>();

            return storageItem;
        }

        public void Start()
        {
            _button = GetComponent<Button>();
        }

        public void OnClick()
        {

        }

        public void SetStorage(Storage storage)
        {
            _storage = storage;
        }
    }
}
