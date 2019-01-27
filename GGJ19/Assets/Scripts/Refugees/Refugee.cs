using Assets.Scripts.Conversation;
using Assets.Scripts.Objects;
using Assets.Scripts.Objects.PortableObjects;
using Assets.Scripts.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Refugees
{
    public abstract class Refugee : InteractableSceneObject
    {
        protected Character _character;
        protected RefugeeSpawningSpot _spawningSpot;
        protected RefugeesSettings _refugeesSettings;
        protected DialogManager _dialogManager;
        protected Karma _karma;

        public int DaysToStay { get; set; }
            
        public override void Start()
        {
            base.Start();
            _character = FindObjectOfType<Character>();
            _dialogManager = FindObjectOfType<DialogManager>();
            _karma = FindObjectOfType<Karma>();
            _refugeesSettings = FindObjectOfType<RefugeesSettings>();
        }

        public bool IsFemale { get; set; }

        public string Name { get; set; }

        public abstract void WakeUp();
        public abstract void Talk();
        public abstract void GiveObject(PortableObjectType objectType);

        public override void Interact()
        {
            _gameManager.Pause = true;
            _character.BeginInteraction(this);
        }

        public void SetSpawningSpot(RefugeeSpawningSpot spawningSpot)
        {
            _spawningSpot = spawningSpot;
        }

        public void LeaveCamp()
        {
            _spawningSpot.Refugee = null;
            Destroy(gameObject);
        }
    }
}
