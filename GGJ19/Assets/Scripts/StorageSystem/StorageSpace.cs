using Assets.Scripts.Objects;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.StorageSystem
{
    public class StorageSpace : MonoBehaviour
    {
        private Button _button;
        private Storage _storage;
        private PortableObject _portableObject;

        public Bag Bag { get; set; }

        public void Start()
        {
            _button = GetComponent<Button>();
        }

        public void SetStorage(Storage storage)
        {
            _storage = storage;
        }
    }
}
