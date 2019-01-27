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

        private Dictionary<PortableObjectType, GameObject> _prefabsPerType;

        public void Start()
        {
            _prefabsPerType = new Dictionary<PortableObjectType, GameObject>
            {
                [PortableObjectType.Bread] = _breadPrefab,
                [PortableObjectType.Coat] = _coatPrefab,
                [PortableObjectType.FeedingBottle] = _feedingBottlePrefab,
                [PortableObjectType.Pills] = _pillsPrefab,
            };
        }

        public GameObject GetPrefab(PortableObjectType portableObjectType)
        {
            return _prefabsPerType[portableObjectType];
        }
    }
}
