﻿using System;
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
        private List<Sprite> _girlSprites;
        [SerializeField]
        private LayerMask _layer;
        [SerializeField]
        private int _initialMaxRefugees = 4;

        [Header("Initial refugees spawning spots")]
        [SerializeField]
        private RefugeeSpawningSpot _initialRefugeeSpawningSpotLayer1;
        [SerializeField]
        private RefugeeSpawningSpot _initialRefugeeSpawningSpotLayer2;
        [SerializeField]
        private RefugeeSpawningSpot _initialRefugeeSpawningSpotLayer3;

        private RefugeeSpawningSpot[] _spawningSpotsLayer1;
        private RefugeeSpawningSpot[] _spawningSpotsLayer2;
        private RefugeeSpawningSpot[] _spawningSpotsLayer3;
        private Random _random;
        private TimeTracker _timeTracker;
        private RefugeesSettings _refugeesSettings;
        private RefugeesResizer _refugeesResizer;
        private List<string> _maleNames;
        private List<string> _femaleNames;
        private List<string> _surnames;

        public int MaxRefugees { get; set; }

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
            _refugeesResizer = new RefugeesResizer();

            MaxRefugees = _initialMaxRefugees;

            ReadRefugeeNames();

            _timeTracker.onNewDayBegun += SpawnRandomRefugees;

            SpawnFirstDaysRefugees();
        }

        public Refugee CreateBasicRefugee(
            RefugeeSpawningSpot spawningSpot,
            int sortingLayerId,
            List<Sprite> familySprites,
            List<Sprite> femaleSprites,
            List<Sprite> maleSprites,
            List<Sprite> girlSprites)
        {
            var refugee = Instantiate(_basicRefugeePrefab).GetComponent<BasicRefugee>();
            return SetUpRefugee(
                refugee,
                spawningSpot,
                sortingLayerId,
                familySprites,
                femaleSprites,
                maleSprites,
                girlSprites);
        }

        public Refugee CreateMediumRefugee(
            RefugeeSpawningSpot spawningSpot,
            int sortingLayerId,
            List<Sprite> familySprites,
            List<Sprite> femaleSprites,
            List<Sprite> maleSprites,
            List<Sprite> girlSprites)
        {
            var refugee = Instantiate(_mediumRefugeePrefab).GetComponent<MediumRefugee>();
            refugee = (MediumRefugee)SetUpRefugee(
                refugee,
                spawningSpot,
                sortingLayerId,
                familySprites,
                femaleSprites,
                maleSprites,
                girlSprites);
            refugee.SetRandomLine();
            return refugee;
        }

        private Refugee SetUpRefugee(
            Refugee refugee,
            RefugeeSpawningSpot spawningSpot,
            int sortingLayerId,
            List<Sprite> familySprites,
            List<Sprite> femaleSprites,
            List<Sprite> maleSprites,
            List<Sprite> girlSprites)
        {
            refugee.transform.position = spawningSpot.transform.position;
            refugee.SetSpawningSpot(spawningSpot);

            refugee.IsFemale = IsFemale();
            refugee.IsChild = refugee.IsFemale ? DecideIfRefugeeIsChild() : false;
            refugee.Name = refugee.IsFemale ? _femaleNames.GetRandomElement() : _maleNames.GetRandomElement();
            refugee.DaysToStay = _random.Next(MinDaysToStay, _refugeesSettings.MaxDaysToStay);

            var spriteRenderer = refugee.GetComponent<SpriteRenderer>();
            Sprite selectedSprite = null;
            selectedSprite = SelectRandomSprite(
                refugee,
                familySprites,
                femaleSprites,
                maleSprites,
                girlSprites);

            if (selectedSprite == null)
            {
                refugee.IsFemale = !refugee.IsFemale;
                refugee.Name = refugee.IsFemale ? _femaleNames.GetRandomElement() : _maleNames.GetRandomElement();
                selectedSprite = SelectRandomSprite(
                    refugee,
                    familySprites,
                    femaleSprites,
                    maleSprites,
                    girlSprites);
            }

            if (selectedSprite == null)
            {
                selectedSprite = SelectRandomDuplicatedSprite(refugee);
            }

            spriteRenderer.sprite = selectedSprite;
            spriteRenderer.sortingLayerID = sortingLayerId;
            spriteRenderer.sortingOrder = VisibleSortingOrder;

            refugee.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);

            _refugeesResizer.ResizeRefugeeBasingOnSortingLayer(refugee);
            AddColliderToRefugee(refugee);

            spawningSpot.Refugee = refugee;

            return refugee;
        }

        private Sprite SelectRandomSprite(
            Refugee refugee,
            List<Sprite> familySprites,
            List<Sprite> femaleSprites,
            List<Sprite> maleSprites,
            List<Sprite> girlSprites)
        {
            if (IsFamily())
            {
                return SelectRandomSprite(familySprites);
            }

            if (!refugee.IsFemale)
            {
                return SelectRandomSprite(maleSprites);
            }

            return refugee.IsChild ? SelectRandomSprite(girlSprites) : SelectRandomSprite(femaleSprites);
        }

        private Sprite SelectRandomSprite(List<Sprite> spritesList)
        {
            if (!spritesList.Any())
            {
                return null;
            }

            var selectedSprite = spritesList.GetRandomElement();
            spritesList.Remove(selectedSprite);
            return selectedSprite;
        }

        private Sprite SelectRandomDuplicatedSprite(
            Refugee refugee)
        {
            if (IsFamily())
            {
                return _familySprites.GetRandomElement();
            }

            if (!refugee.IsFemale)
            {
                return _maleSprites.GetRandomElement();
            }

            return refugee.IsChild ? _girlSprites.GetRandomElement() : _femaleSprites.GetRandomElement();
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

        private bool DecideIfRefugeeIsChild()
        {
            return RandomHelper.IsProbabilityReached100(_refugeesSettings.ChildRefugeeProbability);
        }

        public void SpawnRandomRefugees(int dayNumber)
        {
            if (dayNumber < _firstDayToSpanRandomRefugees)
            {
                return;
            }

            var availableFemaleSprites = new List<Sprite>(_femaleSprites);
            var availableMaleSprites = new List<Sprite>(_maleSprites);
            var availableFamilySprites = new List<Sprite>(_familySprites);
            var availableGirlSprites = new List<Sprite>(_girlSprites);

            RemoveExistingSpritesFromLists(availableFamilySprites, availableFemaleSprites, availableMaleSprites, availableGirlSprites);

            SpawnRefugeesForLayer(_spawningSpotsLayer1, "Floor 1", availableFamilySprites, availableFemaleSprites, availableMaleSprites, availableGirlSprites);
            SpawnRefugeesForLayer(_spawningSpotsLayer2, "Floor 2", availableFamilySprites, availableFemaleSprites, availableMaleSprites, availableGirlSprites);
            SpawnRefugeesForLayer(_spawningSpotsLayer3, "Floor 3", availableFamilySprites, availableFemaleSprites, availableMaleSprites, availableGirlSprites);
        }

        private void RemoveExistingSpritesFromLists(
            List<Sprite> familySprites,
            List<Sprite> femaleSprites,
            List<Sprite> maleSprites,
            List<Sprite> girlSprites)
        {

            var existingSprites = FindObjectsOfType<Refugee>().Select(refugee =>
                refugee.GetComponent<SpriteRenderer>().sprite.name).ToList();

            foreach (var item in existingSprites)
            {
                if (femaleSprites.Any(s => s.name == item))
                {
                    femaleSprites.Remove(femaleSprites.First(s => s.name == item));
                }
                else if (maleSprites.Any(s => s.name == item))
                {
                    maleSprites.Remove(maleSprites.First(s => s.name == item));
                }
                else if (familySprites.Any(s => s.name == item))
                {
                    familySprites.Remove(familySprites.First(s => s.name == item));
                }
                else if (girlSprites.Any(s => s.name == item))
                {
                    girlSprites.Remove(girlSprites.First(s => s.name == item));
                }
            }
        }

        private void SpawnRefugeesForLayer(
            RefugeeSpawningSpot[] spots,
            string sortingLayerName,
            List<Sprite> familySprites,
            List<Sprite> femaleSprites,
            List<Sprite> maleSprites,
            List<Sprite> girlSprites)
        {
            var sortingLayerId = SortingLayer.NameToID(sortingLayerName);

            var existingRefugeesCount = FindObjectsOfType<Refugee>().Count();
            foreach (var refugeeSpawningSpot in spots)
            {
                if (existingRefugeesCount < MaxRefugees && refugeeSpawningSpot.Refugee == null)
                {
                    var randomNumber = _random.Next(0, 100);
                    if (randomNumber < _spotSpawningProbability)
                    {
                        SpawnRandomRefugee(
                            refugeeSpawningSpot,
                            sortingLayerId,
                            familySprites,
                            femaleSprites,
                            maleSprites,
                            girlSprites);
                        existingRefugeesCount++;
                    }
                }
            }
        }

        public void SpawnRandomRefugee(
            RefugeeSpawningSpot spawningSpot,
            int sortingLayerId,
            List<Sprite> familySprites,
            List<Sprite> femaleSprites,
            List<Sprite> maleSprites,
            List<Sprite> girlSprites)
        {
            var randomNumber = _random.Next(0, 100);
            if (randomNumber < _basicRefugeeProbability)
            {
                CreateBasicRefugee(
                    spawningSpot,
                    sortingLayerId,
                    familySprites,
                    femaleSprites,
                    maleSprites,
                    girlSprites);
            }
            else
            {
                CreateMediumRefugee(
                    spawningSpot,
                    sortingLayerId,
                    familySprites,
                    femaleSprites,
                    maleSprites,
                    girlSprites);
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

        private void SpawnFirstDaysRefugees()
        {
            var availableFemaleSprites = new List<Sprite>(_femaleSprites);
            var availableMaleSprites = new List<Sprite>(_maleSprites);
            var availableFamilySprites = new List<Sprite>(_familySprites);
            var availableGirlSprites = new List<Sprite>(_girlSprites);

            var refugee1 = CreateBasicRefugee(
                _initialRefugeeSpawningSpotLayer1,
                SortingLayer.NameToID(SortingLayers.Floor1),
                availableFamilySprites,
                availableFemaleSprites,
                availableMaleSprites,
                availableGirlSprites) as RefugeeWithBasicNeeds;

            refugee1.ClearNeeds();
            refugee1.HungerResolved = false;
            refugee1.ThirstResolved = false;
            refugee1.PrintNeeds();

            var refugee2 = CreateBasicRefugee(
                _initialRefugeeSpawningSpotLayer2,
                SortingLayer.NameToID(SortingLayers.Floor2),
                availableFamilySprites,
                availableFemaleSprites,
                availableMaleSprites,
                availableGirlSprites) as RefugeeWithBasicNeeds;

            refugee2.ClearNeeds();
            refugee2.IllnessResolved = false;
            refugee2.Ill = true;
            refugee2.PrintNeeds();

            var refugee3 = CreateMediumRefugee(
                _initialRefugeeSpawningSpotLayer3,
                SortingLayer.NameToID(SortingLayers.Floor3),
                availableFamilySprites,
                availableFemaleSprites,
                availableMaleSprites,
                availableGirlSprites) as MediumRefugee;

            refugee3.ClearNeeds();
            refugee3.SetLine(10);
            refugee3.PrintNeeds();
        }
    }
}
