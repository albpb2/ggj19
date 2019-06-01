using Assets.Scripts.Extensions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static Assets.Scripts.Maths.Probabilities;

namespace Assets.Scripts.Objects.InteractableSceneObjects
{
    public class CarafeSpawner : InteractableObjectSpawner<Carafe>
    {
        private const float HorizontalInversionProbability = 0.5f;

        [SerializeField]
        private List<Carafe> spawnableCarafes;

        private TimeTracker _timeTracker;
        private List<Carafe> carafes;

        protected void Awake()
        {
            _timeTracker = FindObjectOfType<TimeTracker>();
            carafes = new List<Carafe>();
        }

        protected void Start()
        {
            _timeTracker.onNewDayBegun += SpawnCarafes;
            _timeTracker.onDayEnded += DestroyCarafes;

            SpawnCarafes(1);
        }

        protected void OnDestroy()
        {
            _timeTracker.onNewDayBegun -= SpawnCarafes;
            _timeTracker.onDayEnded -= DestroyCarafes;
        }

        protected override Carafe InstantiateItem()
        {
            return Instantiate(spawnableCarafes.GetRandomElement());
        }

        protected override void PostSpawnItem(Carafe item)
        {
            var shouldInvertHorizontally = CalculateSuccessBase1(HorizontalInversionProbability);

            if (shouldInvertHorizontally)
            {
                var spriteRenderer = item.GetComponent<SpriteRenderer>();
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
        }

        private void SpawnCarafes(int dayNumber)
        {
            foreach (var spawnPoint in _spawnPoints)
            {
                spawnPoint.SetActive(true);
            }

            carafes = SpawnItems().ToList();
        }

        private void DestroyCarafes(int dayNumber)
        {
            foreach(var carafe in carafes)
            {
                Destroy(carafe.gameObject);
            }
        }
    }
}
