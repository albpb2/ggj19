using System;
using System.Linq;
using Assets.Scripts.Conversation;
using Assets.Scripts.Extensions;
using Assets.Scripts.Objects.PortableObjects;

namespace Assets.Scripts.Refugees
{
    public abstract class RefugeeWithBasicNeeds : Refugee
    {
        protected Random _random;

        public override void Start()
        {
            base.Start();

            _random = new Random();

            _timeTracker.onDayEnded += CheckStatusAtEndOfDay;
        }

        public bool HungerResolved { get; set; }

        public bool ThirstResolved { get; set; }

        public bool Ill { get; set; }

        public bool IllnessResolved { get; set; }

        public override void Interact()
        {
            base.Interact();

            _character.OpenActionsBox();
        }

        public override void GiveObject(PortableObjectType objectType)
        {
            int lineId;
            BasicDialogLine line;

            if (!HungerResolved && objectType == PortableObjectType.Bread)
            {
                lineId = BasicDialogLine.ThanksLines.GetRandomElement();
                line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);
                _dialogManager.WriteBasicDialogLine(line, Name);
                UpdateKarma(_refugeesSettings.HungerResolvedPoints);
                return;
            }
            if (!ThirstResolved && objectType == PortableObjectType.Water)
            {
                lineId = BasicDialogLine.ThanksLines.GetRandomElement();
                line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);
                _dialogManager.WriteBasicDialogLine(line, Name);
                UpdateKarma(_refugeesSettings.ThirstResolvedPoints);
                return;
            }
            if (Ill && !IllnessResolved && objectType == PortableObjectType.Pills)
            {
                lineId = BasicDialogLine.ThanksLines.GetRandomElement();
                line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);
                _dialogManager.WriteBasicDialogLine(line, Name);
                UpdateKarma(_refugeesSettings.IllnessResolvedPoints);
                return;
            }

            lineId = BasicDialogLine.WrongChoiceLines.GetRandomElement();
            line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);
            _dialogManager.WriteBasicDialogLine(line, Name);
        }

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
            var karmaModifier = 0;
            if (!HungerResolved)
            {
                karmaModifier -= _refugeesSettings.HungerResolvedPoints;
            }
            if (!ThirstResolved)
            {
                karmaModifier -= _refugeesSettings.ThirstResolvedPoints;
            }
            if (Ill && !IllnessResolved)
            {
                karmaModifier -= _refugeesSettings.IllnessResolvedPoints;
            }

            UpdateKarma(karmaModifier);
        }

        protected void UpdateKarma(int karmaModifier)
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
