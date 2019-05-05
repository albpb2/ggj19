using Assets.Scripts.Conversation;
using Assets.Scripts.Events;
using Assets.Scripts.Extensions;
using Assets.Scripts.Maths;
using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.Refugees.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.Refugees
{
    public abstract class RefugeeWithBasicNeeds : Refugee
    {
        protected Random _random;

        protected Dictionary<BasicNeed, Func<GameEvent>> _createNeedResolvedEvent;
        protected Dictionary<BasicNeed, int> _pointsPerNeed;

        public override void Start()
        {
            base.Start();

            _random = new Random();

            InitializeNeedResolvedEventsDictionary();
            IntializePointsPerNeedDictionary();

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

            _character.TalkToRefugee();
        }

        public override void GiveObject(PortableObjectType objectType)
        {
            int lineId;
            BasicDialogLine line;

            if (!HungerResolved && objectType == PortableObjectType.Bread)
            {
                HungerResolved = true;
                ResolveNeed(BasicNeed.Hunger);
                return;
            }
            if (!ThirstResolved && (objectType == PortableObjectType.Water || objectType == PortableObjectType.Bottle))
            {
                ThirstResolved = true;
                ResolveNeed(BasicNeed.Thirst);
                return;
            }
            if (objectType == PortableObjectType.Coat)
            {
                if (!_inventory.Contains(PortableObjectType.Coat))
                {
                    _inventory.Add(PortableObjectType.Coat);
                }
                if (!ColdResolved)
                {
                    ColdResolved = true;
                    ResolveNeed(BasicNeed.Cold);
                    return;
                }
            }
            if (Ill && !IllnessResolved && objectType == PortableObjectType.Pills)
            {
                IllnessResolved = true;
                ResolveNeed(BasicNeed.Illness);
                return;
            }

            lineId = BasicDialogLine.WrongChoiceLines.GetRandomElement();
            line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);
            _dialogManager.WriteBasicDialogLine(line, Name, false);
            UpdateKarma(_refugeesSettings.RandomObjectPoints);
        }

        private void ResolveNeed(BasicNeed resolvedNeed)
        {
            var lineId = BasicDialogLine.ThanksLines.GetRandomElement();
            var line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);
            _dialogManager.WriteBasicDialogLine(line, Name, false);
            _gameEventsManager.AddEvent(_createNeedResolvedEvent[resolvedNeed]());
            UpdateKarma(_pointsPerNeed[resolvedNeed]);
        }

        public override void Talk()
        {
            var possibleLines = new List<int>();
            var isObjectRequest = true;

            if (!HungerResolved && !ThirstResolved)
            {
                possibleLines.AddRange(BasicDialogLine.HungerAndThirstLines);
            }
            else if (!HungerResolved)
            {
                possibleLines.AddRange(BasicDialogLine.HungerLines);
            }
            else if (!ThirstResolved)
            {
                possibleLines.AddRange(BasicDialogLine.ThirstLines);
            }

            if (!ColdResolved)
            {
                possibleLines.AddRange(BasicDialogLine.ColdLines);
            }

            if (Ill & !IllnessResolved)
            {
                possibleLines.AddRange(BasicDialogLine.IllnessLines);
            }

            if (HungerResolved && ThirstResolved && ColdResolved && (!Ill || IllnessResolved))
            {
                possibleLines.AddRange(BasicDialogLine.GreetingLines);
                isObjectRequest = false;
            }

            var lineId = possibleLines.GetRandomElement();
            var line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);

            _dialogManager.WriteBasicDialogLine(line, Name, isObjectRequest);
        }

        public void ResetNeeds()
        {
            HungerResolved = !Probabilities.CalculateSuccessBase100(_refugeesSettings.HungerProbability);
            ThirstResolved = !Probabilities.CalculateSuccessBase100(_refugeesSettings.ThirstProbability);
            ColdResolved = _inventory.Contains(PortableObjectType.Coat) || !Probabilities.CalculateSuccessBase100(_refugeesSettings.ColdProbability);
            Ill = Probabilities.CalculateSuccessBase100(_refugeesSettings.IllnessProbability);

            IllnessResolved = false;

            PrintNeeds();
        }

        protected override string GetNeedsString()
        {
            var message = "";

            if (!HungerResolved)
            {
                message += "food, ";
            }
            if (!ThirstResolved)
            {
                message += "water, ";
            }
            if (!ColdResolved)
            {
                message += "jacket, ";
            }
            if (!IllnessResolved && Ill)
            {
                message += "pills, ";
            }

            if (message.EndsWith(", "))
            {
                message = message.Substring(0, message.Length - 2);
            }

            return message;
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

        private void InitializeNeedResolvedEventsDictionary()
        {
            _createNeedResolvedEvent = new Dictionary<BasicNeed, Func<GameEvent>>()
            {
                [BasicNeed.Hunger] = () => new HungerSolvedGameEvent(),
                [BasicNeed.Thirst] = () => new ThirstSolvedEvent(),
                [BasicNeed.Illness] = () => new IllnessSolvedEvent(),
                [BasicNeed.Cold] = () => new ColdSolvedEvent(),
            };
        }

        private void IntializePointsPerNeedDictionary()
        {
            _pointsPerNeed = new Dictionary<BasicNeed, int>
            {
                [BasicNeed.Hunger] = _refugeesSettings.HungerResolvedPoints,
                [BasicNeed.Thirst] = _refugeesSettings.ThirstResolvedPoints,
                [BasicNeed.Illness] = _refugeesSettings.IllnessResolvedPoints,
                [BasicNeed.Cold] = _refugeesSettings.ColdItemProbability
            };
        }
    }
}
