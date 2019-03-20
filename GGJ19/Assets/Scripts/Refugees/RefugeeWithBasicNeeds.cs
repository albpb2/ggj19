﻿using Assets.Scripts.Conversation;
using Assets.Scripts.Extensions;
using Assets.Scripts.Maths;
using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.Refugees.Events;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

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

        public bool ColdResolved { get; set; }

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
                HungerResolved = true;
                _gameEventsManager.AddEvent(new HungerSolvedGameEvent());
                UpdateKarma(_refugeesSettings.HungerResolvedPoints);
                return;
            }
            if (!ThirstResolved && (objectType == PortableObjectType.Water || objectType == PortableObjectType.Bottle))
            {
                lineId = BasicDialogLine.ThanksLines.GetRandomElement();
                line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);
                _dialogManager.WriteBasicDialogLine(line, Name);
                ThirstResolved = true;
                _gameEventsManager.AddEvent(new ThirstSolvedEvent());
                UpdateKarma(_refugeesSettings.ThirstResolvedPoints);
                return;
            }
            if (!ColdResolved && (objectType == PortableObjectType.Coat))
            {
                lineId = BasicDialogLine.ThanksLines.GetRandomElement();
                line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);
                _dialogManager.WriteBasicDialogLine(line, Name);
                ColdResolved = true;
                _gameEventsManager.AddEvent(new ColdSolvedEvent());
                UpdateKarma(_refugeesSettings.ColdResolvedPoints);
                return;
            }
            if (Ill && !IllnessResolved && objectType == PortableObjectType.Pills)
            {
                lineId = BasicDialogLine.ThanksLines.GetRandomElement();
                line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);
                _dialogManager.WriteBasicDialogLine(line, Name);
                IllnessResolved = true;
                _gameEventsManager.AddEvent(new IllnessSolvedEvent());
                UpdateKarma(_refugeesSettings.IllnessResolvedPoints);
                return;
            }

            lineId = BasicDialogLine.WrongChoiceLines.GetRandomElement();
            line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);
            _dialogManager.WriteBasicDialogLine(line, Name);
            UpdateKarma(_refugeesSettings.RandomObjectPoints);
        }

        public override void Talk()
        {
            var possibleLines = new List<int>();

            if (!HungerResolved)
            {
                possibleLines.AddRange(BasicDialogLine.HungerLines);
            }

            if (!ThirstResolved)
            {
                possibleLines.AddRange(BasicDialogLine.ThirstLines);
            }

            if (!HungerResolved && !ThirstResolved)
            {
                possibleLines.AddRange(BasicDialogLine.HungerAndThirstLines);
            }

            if (!ColdResolved)
            {
                possibleLines.AddRange(BasicDialogLine.ColdLines);
            }

            if (Ill & !IllnessResolved)
            {
                possibleLines.AddRange(BasicDialogLine.IllnessLines);
            }

            if (HungerResolved && ThirstResolved && (!Ill || IllnessResolved))
            {
                possibleLines.AddRange(BasicDialogLine.GreetingLines);
            }

            var lineId = possibleLines.GetRandomElement();
            var line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);

            _dialogManager.WriteBasicDialogLine(line, Name);
        }

        public void ResetNeeds()
        {
            HungerResolved = !Probabilities.CalculateSuccessBase100(_refugeesSettings.HungerProbability);
            ThirstResolved = !Probabilities.CalculateSuccessBase100(_refugeesSettings.ThirstProbability);
            ColdResolved = !Probabilities.CalculateSuccessBase100(_refugeesSettings.ColdProbability);
            Ill = !Probabilities.CalculateSuccessBase100(_refugeesSettings.IllnessProbability);

            IllnessResolved = false;
        }

        public void ClearNeeds()
        {
            HungerResolved = true;
            ThirstResolved = true;
            ColdResolved = true;
            Ill = false;

            IllnessResolved = true;
        }

        public override void WakeUp(int dayNumber)
        {
            if (dayNumber != 1)
            {
                ResetNeeds();
            }
        }

        public void Feed()
        {
            HungerResolved = true;
        }

        public void GiveWater()
        {
            ThirstResolved = true;
        }

        public void GiveJacket()
        {
            ColdResolved = true;
        }

        public void Heal()
        {
            Ill = false;
            IllnessResolved = true;
        }

        public void CheckStatusAtEndOfDay(int dayNumber)
        {
            var karmaModifier = 0;
            if (!HungerResolved)
            {
                karmaModifier -= _refugeesSettings.HungerResolvedPoints / 2;
            }
            if (!ThirstResolved)
            {
                karmaModifier -= _refugeesSettings.ThirstResolvedPoints / 2;
            }
            if (!ColdResolved)
            {
                karmaModifier -= _refugeesSettings.ColdResolvedPoints / 2;
            }
            if (Ill && !IllnessResolved)
            {
                karmaModifier -= _refugeesSettings.IllnessResolvedPoints / 2;
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

        public override void LeaveCamp()
        {
            _timeTracker.onDayEnded -= CheckStatusAtEndOfDay;

            base.LeaveCamp();
        }
    }
}
