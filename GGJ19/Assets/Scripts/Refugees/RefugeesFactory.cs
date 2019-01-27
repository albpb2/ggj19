using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Extensions;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Refugees
{
    public class RefugeesFactory : MonoBehaviour
    {
        protected const int MinDaysToStay = 1;

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
        [SerializeField]
        private TextAsset _maleNamesFile;
        [SerializeField]
        private TextAsset _femaleNamesFile;

        private RefugeeSpawningSpot[] _spawningSpots;
        private Random _random;
        private TimeTracker _timeTracker;
        private RefugeesSettings _refugeesSettings;
        private List<string> _maleNames;
        private List<string> _femaleNames;
        private List<string> _surnames;

        public void Start()
        {
            _spawningSpots = FindObjectsOfType<RefugeeSpawningSpot>();
            _timeTracker = FindObjectOfType<TimeTracker>();
            _refugeesSettings = FindObjectOfType<RefugeesSettings>();
            _random = new Random();

            ReadRefugeeNames();

            _timeTracker.onNewDayBegun += SpawnRandomRefugees;
        }

        public Refugee CreateBasicRefugee(RefugeeSpawningSpot spawningSpot)
        {
            var refugee = Instantiate(_basicRefugeePrefab).GetComponent<BasicRefugee>();
            refugee.transform.position = spawningSpot.transform.position;
            refugee.SetSpawningSpot(spawningSpot);
            
            refugee.IsFemale = IsFemale();
            refugee.Name = refugee.IsFemale ? _femaleNames.GetRandomElement() : _maleNames.GetRandomElement();
            refugee.DaysToStay = _random.Next(MinDaysToStay, _refugeesSettings.MaxDaysToStay);

            spawningSpot.Refugee = refugee;

            return refugee;
        }

        private bool IsFemale()
        {
            return _random.Next(0, 2) == 1;
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
                CreateBasicRefugee(spawningSpot);
            }
        }

        private void ReadRefugeeNames()
        {
            _maleNames = new List<string>();
            _femaleNames = new List<string>();
            _surnames = new List<string>();

            var maleNames = _maleNamesFile.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var maleName in maleNames)
            {
                var parts = maleName.Split(' ');
                _maleNames.Add(parts[0]);
                _surnames.Add(parts[1]);
            }

            var femaleNames = _femaleNamesFile.text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var femaleName in femaleNames)
            {
                var parts = femaleName.Split(' ');
                _femaleNames.Add(parts[0]);
                if (!_surnames.Contains(parts[1]))
                {
                    _surnames.Add(parts[1]);
                }
            }
        }
    }
}
