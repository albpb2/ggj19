using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Conversation;
using Assets.Scripts.Events;
using Assets.Scripts.Extensions;
using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.Refugees.Events;
using Random = System.Random;

namespace Assets.Scripts.Refugees
{
    public abstract class RefugeeWithBasicNeeds : Refugee
    {
        private GameEventsManager _gameEventsManager;

        protected Random _random;

        void Awake()
        {
            _gameEventsManager = FindObjectOfType<GameEventsManager>();
        }

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
                _gameEventsManager.AddEvent(new HungerSolvedGameEvent());
                UpdateKarma(_refugeesSettings.HungerResolvedPoints);
                return;
            }
            if (!ThirstResolved && (objectType == PortableObjectType.Water || objectType == PortableObjectType.Bottle || objectType == PortableObjectType.FeedingBottle))
            {
                lineId = BasicDialogLine.ThanksLines.GetRandomElement();
                line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);
                _dialogManager.WriteBasicDialogLine(line, Name);
                _gameEventsManager.AddEvent(new ThirstSolvedEvent());
                UpdateKarma(_refugeesSettings.ThirstResolvedPoints);
                return;
            }
            if (Ill && !IllnessResolved && objectType == PortableObjectType.Pills)
            {
                lineId = BasicDialogLine.ThanksLines.GetRandomElement();
                line = _dialogManager.BasicDialogLines.SingleOrDefault(l => l.LineId == lineId);
                _dialogManager.WriteBasicDialogLine(line, Name);
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
            if (!RefugeeCountsForKarma())
            {
                return;
            }

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

        public override void LeaveCamp()
        {
            _timeTracker.onDayEnded -= CheckStatusAtEndOfDay;

            base.LeaveCamp();
        }
    }
}
