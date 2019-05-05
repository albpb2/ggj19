using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Conversation;
using Assets.Scripts.Events;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Refugees
{
    public abstract class Refugee : InteractableSceneObject
    {
        protected Character _character;
        protected RefugeeSpawningSpot _spawningSpot;
        protected RefugeesSettings _refugeesSettings;
        protected DialogManager _dialogManager;
        protected Karma _karma;
        protected TimeTracker _timeTracker;
        protected GameEventsManager _gameEventsManager;
        protected List<PortableObjectType> _inventory;

        public int DaysToStay { get; set; }

        public int ArrivalDay { get; set; }

        public void Awake()
        {
            _inventory = new List<PortableObjectType>();

            _character = FindObjectOfType<Character>();
            _dialogManager = FindObjectOfType<DialogManager>();
            _karma = FindObjectOfType<Karma>();
            _refugeesSettings = FindObjectOfType<RefugeesSettings>();
            _timeTracker = FindObjectOfType<TimeTracker>();
            _gameEventsManager = FindObjectOfType<GameEventsManager>();
        }

        public override void Start()
        {
            base.Start();
            
            ArrivalDay = _timeTracker.CurrentDay;

            _timeTracker.onDayEnded += LeaveCampIfDayArrived;
            _timeTracker.onNewDayBegun += WakeUp;
            
            Debug.Log($"Refugee {Name} has arrived to the camp.");

            WakeUp(_timeTracker.CurrentDay);
        }

        public bool IsFemale { get; set; }

        public bool IsChild { get; set; }

        public string Name { get; set; }

        public abstract void WakeUp(int dayNumber);
        public abstract void Talk();
        public abstract void GiveObject(PortableObjectType objectType);

        public override void Interact()
        {
            if (GetComponent<SpriteRenderer>().sortingLayerName != _character.GetComponent<SpriteRenderer>().sortingLayerName)
            {
                return;
            }

            _gameManager.GameFreezed = true;
            _character.BeginInteraction(this);
        }

        public void SetSpawningSpot(RefugeeSpawningSpot spawningSpot)
        {
            _spawningSpot = spawningSpot;
        }

        public virtual void LeaveCamp()
        {
            Debug.Log($"Refugee {Name} is leaving the camp.");

            _spawningSpot.Refugee = null;
            _timeTracker.onNewDayBegun -= WakeUp;
            _timeTracker.onDayEnded -= LeaveCampIfDayArrived;
            Destroy(gameObject);
        }

        public void LeaveCampIfDayArrived(int dayNumber)
        {
            if (_timeTracker.CurrentDay + 1 - ArrivalDay >= DaysToStay)
            {
                LeaveCamp();
            }
        }

        public void PrintNeeds()
        {
            Debug.Log($"Refugee {Name} needs {GetNeedsString()}");
        }

        protected virtual string GetNeedsString()
        {
            return string.Empty;
        }
    }
}
