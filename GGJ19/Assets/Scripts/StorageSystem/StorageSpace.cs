using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.StorageSystem
{
    public class StorageSpace : MonoBehaviour
    {
        private Button _button;
        private Storage _storage;
        private PortableObject _portableObject;

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
