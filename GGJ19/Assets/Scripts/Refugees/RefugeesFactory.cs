using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Refugees
{
    public class RefugeesFactory : MonoBehaviour
    {
        [SerializeField]
        private GameObject _basicRefugeePrefab;
        [SerializeField]
        private GameObject _mediumRefugeePrefab;
        [SerializeField]
        private GameObject _complexRefugeePrefab;
        [SerializeField]
        private int _spotSpawningProbability = 50;
        [SerializeField]
        private int _basicRefugeeProbability = 50;
        [SerializeField]
        private int _firstDayToSpanRandomRefugees = 2;

        private RefugeeSpawningSpot[] _spawningSpots;
        private Random _random;
        private TimeTracker _timeTracker;

        public void Start()
        {
            _spawningSpots = FindObjectsOfType<RefugeeSpawningSpot>();
            _timeTracker = FindObjectOfType<TimeTracker>();
            _random = new Random();

            _timeTracker.onNewDayBegun += SpawnRandomRefugees;
        }

        public Refugee CreateBasicRefugee(RefugeeSpawningSpot spawningSpot)
        {
            var refugee = Instantiate(_basicRefugeePrefab).GetComponent<BasicRefugee>();
            refugee.transform.position = spawningSpot.transform.position;
            refugee.SetSpawningSpot(spawningSpot);
            spawningSpot.Refugee = refugee;

            return refugee;
        }

        public void SpawnRandomRefugees(int dayNumber)
        {
            if (dayNumber < _firstDayToSpanRandomRefugees)
            {
                return;
            }

            foreach (var refugeeSpawningSpot in _spawningSpots)
            {
                if (refugeeSpawningSpot.Refugee == null)
                {
                    var randomNumber = _random.Next(0, 100);
                    if (randomNumber < _spotSpawningProbability)
                    {
                        SpawnRandomRefugee(refugeeSpawningSpot);
                    }
                }
            }
        }

        public void SpawnRandomRefugee(RefugeeSpawningSpot spawningSpot)
        {
            var randomNumber = _random.Next(0, 100);
            if (randomNumber < _basicRefugeeProbability)
            {
                var refugee = CreateBasicRefugee(spawningSpot);
            }
        }
    }
}
