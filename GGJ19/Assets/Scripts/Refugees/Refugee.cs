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

        public int DaysToStay { get; set; }

        public int ArrivalDay { get; set; }
            
        public override void Start()
        {
            base.Start();
            _character = FindObjectOfType<Character>();
            _dialogManager = FindObjectOfType<DialogManager>();
            _karma = FindObjectOfType<Karma>();
            _refugeesSettings = FindObjectOfType<RefugeesSettings>();
            _timeTracker = FindObjectOfType<TimeTracker>();
            _gameEventsManager = FindObjectOfType<GameEventsManager>();

            ArrivalDay = _timeTracker.CurrentDay;
            _timeTracker.onDayEnded += LeaveCampIfDayArrived;
        }

        public bool IsFemale { get; set; }

        public string Name { get; set; }

        public abstract void WakeUp();
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
            _spawningSpot.Refugee = null;
            _timeTracker.onDayEnded -= LeaveCampIfDayArrived;
            Destroy(gameObject);
        }

        public void LeaveCampIfDayArrived()
        {
            if (_timeTracker.CurrentDay + 1 - ArrivalDay >= DaysToStay)
            {
                LeaveCamp();
            }
        }

        protected bool RefugeeCountsForKarma()
        {
            return _refugeesSettings.ValidSortingLayers == null ||
                   !_refugeesSettings.ValidSortingLayers.Any() ||
                   _refugeesSettings.ValidSortingLayers.Contains(GetComponent<SpriteRenderer>().sortingLayerName);
        }
    }
}
