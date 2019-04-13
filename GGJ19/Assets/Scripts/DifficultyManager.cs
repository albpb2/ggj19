using Assets.Scripts.Refugees;
using System;
using UnityEngine;

namespace Assets.Scripts
{
    public class DifficultyManager : MonoBehaviour
    {
        private const int MaxTotalRefugees = 8;

        [SerializeField]
        private int _daysToReduceItemsProbability;
        [SerializeField]
        private int _itemsProbabilityReductionStep;
        [SerializeField]
        private int _daysToIncreaseNeedsProbability;
        [SerializeField]
        private int _needsProbabilityIncreaseStep;
        [SerializeField]
        private int _daysToIncreaseMaxRefugees;

        private TimeTracker _timeTracker;
        private RefugeesSettings _refugeesSettings;
        private RefugeesFactory _refugeesFactory;

        private int _lastDayItemsProbabilityWasReduced;
        private int _lastDayNeedsProbabilityWasIncreased;
        private int _lastDayMaxRefugeesWasIncreased;

        public void Start()
        {
            _timeTracker = FindObjectOfType<TimeTracker>();
            _refugeesSettings = FindObjectOfType<RefugeesSettings>();
            _refugeesFactory = FindObjectOfType<RefugeesFactory>();

            _timeTracker.onDayEnded += UpdateDifficulty; 
        }

        void UpdateDifficulty(int dayNumber)
        {
            if (dayNumber - _lastDayItemsProbabilityWasReduced >= _daysToReduceItemsProbability)
            {
                _refugeesSettings.ProperItemSpawningProbability = Math.Max(_refugeesSettings.ProperItemSpawningProbability - _itemsProbabilityReductionStep, 0);
                _lastDayItemsProbabilityWasReduced = dayNumber;
            }

            if (dayNumber - _lastDayNeedsProbabilityWasIncreased >= _daysToIncreaseNeedsProbability)
            {
                _refugeesSettings.HungerProbability = Increase100BasedProbability(_refugeesSettings.HungerProbability, _needsProbabilityIncreaseStep);
                _refugeesSettings.ThirstProbability = Increase100BasedProbability(_refugeesSettings.ThirstProbability, _needsProbabilityIncreaseStep);
                _refugeesSettings.ColdProbability = Increase100BasedProbability(_refugeesSettings.ColdProbability, _needsProbabilityIncreaseStep);
                _refugeesSettings.IllnessProbability = Increase100BasedProbability(_refugeesSettings.IllnessProbability, _needsProbabilityIncreaseStep);
                _lastDayNeedsProbabilityWasIncreased = dayNumber;
            }

            if (dayNumber - _lastDayMaxRefugeesWasIncreased >= _daysToIncreaseMaxRefugees)
            {
                _refugeesFactory.MaxRefugees = Math.Min(_refugeesFactory.MaxRefugees + 1, MaxTotalRefugees);
                _lastDayMaxRefugeesWasIncreased = dayNumber;
            }
        }

        private int Increase100BasedProbability(int currentValue, int step)
        {
            const int MaxProbability = 100;
            return Math.Min(currentValue + step, MaxProbability);
        }
    }
}
