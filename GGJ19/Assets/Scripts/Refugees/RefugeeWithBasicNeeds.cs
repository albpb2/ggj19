using System;
using Assets.Scripts.Player;

namespace Assets.Scripts.Refugees
{
    public abstract class RefugeeWithBasicNeeds : Refugee
    {
        protected Random _random;
        protected TimeTracker _timeTracker;

        public override void Start()
        {
            base.Start();

            _timeTracker = FindObjectOfType<TimeTracker>();
            _random = new Random();

            _timeTracker.onDayEnded += CheckStatusAtEndOfDay;
        }

        public bool HungerResolved { get; set; }

        public bool ThirstResolved { get; set; }

        public bool Ill { get; set; }

        public bool IllnessResolved { get; set; }

        public void ResetNeeds()
        {
            HungerResolved = false;
            ThirstResolved = false;

            var randomNumber = _random.Next(0, 100);
            if (randomNumber < _refugeesSettings.IllnessProbability)
            {
                Ill = true;
            }

            IllnessResolved = false;
        }

        public override void WakeUp()
        {
            ResetNeeds();
        }

        public void Feed()
        {
            HungerResolved = true;
        }

        public void GiveWater()
        {
            ThirstResolved = true;
        }

        public void Heal()
        {
            Ill = false;
            IllnessResolved = true;
        }

        public void CheckStatusAtEndOfDay()
        {
            var karmaModifier = HungerResolved
                ? _refugeesSettings.HungerResolvedPoints
                : -_refugeesSettings.HungerResolvedPoints;
            karmaModifier += ThirstResolved
                ? _refugeesSettings.ThirstResolvedPoints
                : -_refugeesSettings.ThirstResolvedPoints;

            if (IllnessResolved)
            {
                karmaModifier += _refugeesSettings.IllnessResolvedPoints;
            }
            else if (Ill)
            {
                karmaModifier -= _refugeesSettings.IllnessResolvedPoints;
            }

            UpdateKarma(karmaModifier);
        }

        private void UpdateKarma(int karmaModifier)
        {
            if (karmaModifier > 0)
            {
                _karma.Increment(karmaModifier);
            }
            else if (karmaModifier < 0)
            {
                _karma.Reduce(-karmaModifier);
            }
        }
    }
}
