using Assets.Scripts.Extensions;
using Assets.Scripts.Objects.InteractableSceneObjects;
using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.Player;
using Assets.Scripts.Refugees;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.StorageSystem
{
    public class ItemsSpawner
    {
        private readonly Storage _storage;
        private readonly Bag _bag;
        private readonly RefugeesSettings _refugeesSettings;
        private readonly Random _random;

        public ItemsSpawner()
        {
            _storage = Object.FindObjectOfType<Storage>();
            _bag = Object.FindObjectOfType<Bag>();
            _refugeesSettings = Object.FindObjectOfType<RefugeesSettings>();

            _random = new Random();
        }

        public void SpawnItems()
        {
            var numberOfObjects = _random.Next(_storage.MinCapacity, _storage.MaxCapacity + 1);

            if (_storage.ItemsCount > numberOfObjects)
            {
                return;
            }

            var refugeesWithBasicNeeds = Object.FindObjectsOfType<RefugeeWithBasicNeeds>();

            foreach (var refugee in refugeesWithBasicNeeds)
            {
                SpawnItem(refugee);

                if (_storage.ItemsCount > numberOfObjects)
                {
                    return;
                }
            }

            while (_storage.ItemsCount < _storage.MinCapacity)
            {
                SpawnItem(GetRandomObjectType());
            }
        }

        public void SpawnGifts()
        {
            if (!_storage.HasFreeGiftsSpace)
            {
                return;
            }

            var mediumRefugees = Object.FindObjectsOfType<MediumRefugee>();

            foreach (var refugee in mediumRefugees)
            {
                SpawnGift(refugee);

                if (!_storage.HasFreeGiftsSpace)
                {
                    return;
                }
            }
        }

        private void SpawnItem(RefugeeWithBasicNeeds referenceRefugee)
        {
            if (!referenceRefugee.HungerResolved)
            {
                SpawnItem(PortableObjectType.Bread);
            }
            else if (!referenceRefugee.ThirstResolved)
            {
                SpawnItem(PortableObjectType.Bottle);
            }
            else if (!referenceRefugee.ColdResolved)
            {
                SpawnItem(PortableObjectType.Coat);
            }
            else
            {
                SpawnItem(PortableObjectType.Pills);
            }
        }

        private void SpawnItem(PortableObjectType objectType)
        {
            if (RandomHelper.IsProbabilityReached100(_refugeesSettings.ProperItemSpawningProbability))
            {
                _storage.AddItem(objectType);
            }
            else
            {
                _storage.AddItem(GetRandomObjectType());
            }
        }

        private PortableObjectType GetRandomObjectType()
        {
            var totalProbability = _refugeesSettings.HungerItemProbability
                                   + _refugeesSettings.ThirstItemProbability
                                   + _refugeesSettings.ColdItemProbability
                                   + _refugeesSettings.IllnessItemProbability;

            var number = _random.Next(0, 100);
            var hungerItemProbability = _refugeesSettings.HungerItemProbability * 100 / totalProbability;
            var thirstItemProbability = _refugeesSettings.ThirstItemProbability * 100 / totalProbability
                                        + hungerItemProbability;
            var coldItemProbability = _refugeesSettings.ColdItemProbability * 100 / totalProbability
                                      + thirstItemProbability;

            if (number < hungerItemProbability)
            {
                return PortableObjectType.Bread;
            }
            else if (number < thirstItemProbability)
            {
                return PortableObjectType.Bottle;
            }
            else if (number < coldItemProbability)
            {
                return PortableObjectType.Coat;
            }

            return PortableObjectType.Pills;
        }

        private void SpawnGift(MediumRefugee refugee)
        {
            if (!refugee.NostalgiaResolved)
            {
                SpawnGift(refugee.ValidObjectTypes);
            }
        }

        private void SpawnGift(List<PortableObjectType> validObjectTypes)
        {
            if (RandomHelper.IsProbabilityReached100(_refugeesSettings.ProperItemSpawningProbability))
            {
                _storage.AddGift(validObjectTypes.GetRandomElement());
            }
            else
            {
                _storage.AddGift(Gifts.GetGiftPortableObjectTypes().GetRandomElement());
            }
        }
    }
}
