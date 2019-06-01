using Assets.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public abstract class InteractableObjectSpawner<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField]
        protected int _itemsToSpawn;
        [SerializeField]
        protected List<GameObject> _spawnPoints;

        public IEnumerable<T> SpawnItems()
        {
            var remainingSpawnPoints = new List<GameObject>(_spawnPoints.Where(p => p.activeSelf));

            var numberOfSpawnableItems = Mathf.Min(_itemsToSpawn, remainingSpawnPoints.Count);

            for (var i = 0; i < numberOfSpawnableItems; i++)
            {
                yield return SpawnItem(remainingSpawnPoints);
            }
        }

        public T SpawnItem(List<GameObject> freeSpawnPoints)
        {
            var spawnPoint = SelectSpawnPoint(freeSpawnPoints);

            PreSpawnItem();

            var item = InstantiateItem();

            ConfigureItem(item, spawnPoint);

            PostSpawnItem(item);

            spawnPoint.SetActive(false);

            return item;
        }

        protected virtual void PreSpawnItem()
        {
        }

        protected abstract T InstantiateItem();

        protected virtual void PostSpawnItem(T item)
        {
        }

        private GameObject SelectSpawnPoint(List<GameObject> freeSpawnPoints)
        {
            var spawnPoint = freeSpawnPoints.GetRandomElement();
            freeSpawnPoints.Remove(spawnPoint);
            return spawnPoint;
        }

        private void ConfigureItem(T item, GameObject spawnPoint)
        {
            item.transform.position = spawnPoint.transform.position;

            var itemSpriteRenderer = item.GetComponent<SpriteRenderer>();
            var spawnPointSpriteRenderer = spawnPoint.GetComponent<SpriteRenderer>();

            itemSpriteRenderer.sortingLayerID = spawnPointSpriteRenderer.sortingLayerID;
            itemSpriteRenderer.sortingOrder = spawnPointSpriteRenderer.sortingOrder;
        }
    }
}
