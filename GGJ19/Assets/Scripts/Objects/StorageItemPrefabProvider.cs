using System.Collections.Generic;
using Assets.Scripts.Objects.PortableObjects;
using UnityEngine;

namespace Assets.Scripts.Objects
{
    public class StorageItemPrefabProvider : MonoBehaviour
    {
        [SerializeField]
        private GameObject _breadPrefab;
        [SerializeField]
        private GameObject _coatPrefab;
        [SerializeField]
        private GameObject _feedingBottlePrefab;
        [SerializeField]
        private GameObject _pillsPrefab;
        [SerializeField]
        private GameObject _ballPrefab;
        [SerializeField]
        private GameObject _rosePrefab;
        [SerializeField]
        private GameObject _toyPrefab;
        [SerializeField]
        private GameObject _bookPrefab;
        [SerializeField]
        private GameObject _guitarPrefab;
        [SerializeField]
        private GameObject _bottlePrefab;

        private Dictionary<PortableObjectType, GameObject> _prefabsPerType;

        public void Start()
        {
            _prefabsPerType = new Dictionary<PortableObjectType, GameObject>
            {
                [PortableObjectType.Bread] = _breadPrefab,
                [PortableObjectType.Coat] = _coatPrefab,
                [PortableObjectType.FeedingBottle] = _feedingBottlePrefab,
                [PortableObjectType.Pills] = _pillsPrefab,
                [PortableObjectType.Ball] = _ballPrefab,
                [PortableObjectType.Rose] = _rosePrefab,
                [PortableObjectType.Toy] = _toyPrefab,
                [PortableObjectType.Book] = _bookPrefab,
                [PortableObjectType.Guitar] = _guitarPrefab,
                [PortableObjectType.Bottle] = _bottlePrefab,
            };
        }

        public GameObject GetPrefab(PortableObjectType portableObjectType)
        {
            return _prefabsPerType[portableObjectType];
        }
    }
}
