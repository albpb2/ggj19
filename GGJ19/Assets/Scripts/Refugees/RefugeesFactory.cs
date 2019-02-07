using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Extensions;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Refugees
{
    public class RefugeesFactory : MonoBehaviour
    {
        private const int MinDaysToStay = 1;
        private const int FamilyProbability = 10;
        private const int VisibleSortingOrder = 2;
        private const int MaxAttemptsToSpawn = 10;

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
        [SerializeField]
        private List<Sprite> _maleSprites;
        [SerializeField]
        private List<Sprite> _femaleSprites;
        [SerializeField]
        private List<Sprite> _familySprites;
        [SerializeField]
        private LayerMask _layer;

        private RefugeeSpawningSpot[] _spawningSpotsLayer1;
        private RefugeeSpawningSpot[] _spawningSpotsLayer2;
        private RefugeeSpawningSpot[] _spawningSpotsLayer3;
        private Random _random;
        private TimeTracker _timeTracker;
        private RefugeesSettings _refugeesSettings;
        private List<string> _maleNames;
        private List<string> _femaleNames;
        private List<string> _surnames;
        private List<string> _existingSprites; 

        public void Start()
        {
            _spawningSpotsLayer1 = GameObject.FindGameObjectsWithTag(Tags.SpawningSpotLayer1)
                .Select(gameObject => gameObject.GetComponent<RefugeeSpawningSpot>()).ToArray();
            _spawningSpotsLayer2 = GameObject.FindGameObjectsWithTag(Tags.SpawningSpotLayer2)
                .Select(gameObject => gameObject.GetComponent<RefugeeSpawningSpot>()).ToArray();
            _spawningSpotsLayer3 = GameObject.FindGameObjectsWithTag(Tags.SpawningSpotLayer3)
                .Select(gameObject => gameObject.GetComponent<RefugeeSpawningSpot>()).ToArray();
            _timeTracker = FindObjectOfType<TimeTracker>();
            _refugeesSettings = FindObjectOfType<RefugeesSettings>();
            _random = new Random();

            ReadRefugeeNames();

            _timeTracker.onNewDayBegun += SpawnRandomRefugees;

            SpawnRandomRefugees(2);
        }

        public Refugee CreateBasicRefugee(RefugeeSpawningSpot spawningSpot, int sortingLayerId)
        {
            var refugee = Instantiate(_basicRefugeePrefab).GetComponent<BasicRefugee>();
            return SetUpRefugee(refugee, spawningSpot, sortingLayerId);
        }

        public Refugee CreateMediumRefugee(RefugeeSpawningSpot spawningSpot, int sortingLayerId)
        {
            var refugee = Instantiate(_mediumRefugeePrefab).GetComponent<MediumRefugee>();
            refugee = (MediumRefugee)SetUpRefugee(refugee, spawningSpot, sortingLayerId);
            refugee.ChooseLine();
            return refugee;
        }

        private Refugee SetUpRefugee(Refugee refugee, RefugeeSpawningSpot spawningSpot, int sortingLayerId)
        {
            refugee.Start();
            refugee.transform.position = spawningSpot.transform.position;
            refugee.SetSpawningSpot(spawningSpot);

            refugee.IsFemale = IsFemale();
            refugee.Name = refugee.IsFemale ? _femaleNames.GetRandomElement() : _maleNames.GetRandomElement();
            refugee.DaysToStay = _random.Next(MinDaysToStay, _refugeesSettings.MaxDaysToStay);

            var spriteRenderer = refugee.GetComponent<SpriteRenderer>();
            Sprite selectedSprite = null;
            int attempts = 0;

            selectedSprite = SelectRandomSprite(refugee);

            if (selectedSprite == null)
            {
                selectedSprite = SelectSpriteInOrder(refugee);
            }

            spriteRenderer.sprite = selectedSprite;
            spriteRenderer.sortingLayerID = sortingLayerId;
            spriteRenderer.sortingOrder = VisibleSortingOrder;
            refugee.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
            AddColliderToRefugee(refugee);

            spawningSpot.Refugee = refugee;

            return refugee;
        }

        private Sprite SelectRandomSprite(Refugee refugee)
        {
            Sprite selectedSprite = null;
            int attempts = 0;

            while (selectedSprite == null && attempts < MaxAttemptsToSpawn)
            {
                attempts++;
                if (IsFamily() && _familySprites.Count > 0)
                {
                    selectedSprite = _familySprites.GetRandomElement();
                }
                else
                {
                    selectedSprite = refugee.IsFemale
                        ? _femaleSprites.GetRandomElement()
                        : _maleSprites.GetRandomElement();
                }

                if (_existingSprites.Contains(selectedSprite.name))
                {
                    selectedSprite = null;
                }
            }

            return selectedSprite;
        }

        private Sprite SelectSpriteInOrder(Refugee refugee)
        {
            if (IsFamily() && _familySprites.Count > 0)
            {
                foreach (var familySprite in _familySprites)
                {
                    if (_existingSprites.Contains(familySprite.name))
                    {
                        return familySprite;
                    }
                }
            }

            if (refugee.IsFemale)
            {
                foreach (var femaleSprite in _femaleSprites)
                {
                    if (_existingSprites.Contains(femaleSprite.name))
                    {
                        return femaleSprite;
                    }
                }
            }

            foreach (var maleSprite in _maleSprites)
            {
                if (_existingSprites.Contains(maleSprite.name))
                {
                    return maleSprite;
                }
            }
            
            return null;
        }

        private void AddColliderToRefugee(Refugee refugee)
        {
            refugee.gameObject.AddComponent<PolygonCollider2D>();
            refugee.gameObject.GetComponent<PolygonCollider2D>().isTrigger = true;
        }

        private bool IsFamily()
        {
            return _random.Next(0, 100) < FamilyProbability;
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

            _existingSprites = FindObjectsOfType<Refugee>().Select(refugee =>
                refugee.GetComponent<SpriteRenderer>().sprite.name).ToList();

            SpawnRefugeesForLayer(_spawningSpotsLayer1, "Floor 1");
            SpawnRefugeesForLayer(_spawningSpotsLayer2, "Floor 2");
            SpawnRefugeesForLayer(_spawningSpotsLayer3, "Floor 3");
        }

        private void SpawnRefugeesForLayer(RefugeeSpawningSpot[] spots, string sortingLayerName)
        {
            var sortingLayerId = SortingLayer.NameToID(sortingLayerName);

            foreach (var refugeeSpawningSpot in spots)
            {
                if (refugeeSpawningSpot.Refugee == null)
                {
                    var randomNumber = _random.Next(0, 100);
                    if (randomNumber < _spotSpawningProbability)
                    {
                        SpawnRandomRefugee(refugeeSpawningSpot, sortingLayerId);
                    }
                }
            }
        }

        public void SpawnRandomRefugee(RefugeeSpawningSpot spawningSpot, int sortingLayerId)
        {
            var randomNumber = _random.Next(0, 100);
            if (randomNumber < _basicRefugeeProbability)
            {
                CreateBasicRefugee(spawningSpot, sortingLayerId);
            }
            else
            {
                CreateMediumRefugee(spawningSpot, sortingLayerId);
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
