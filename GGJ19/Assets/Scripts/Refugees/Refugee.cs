using Assets.Scripts.Conversation;
using Assets.Scripts.Objects;
using Assets.Scripts.Player;

namespace Assets.Scripts.Refugees
{
    public abstract class Refugee : InteractableSceneObject
    {
        protected Character _character;
        protected RefugeeSpawningSpot _spawningSpot;
        protected RefugeesSettings _refugeesSettings;
        protected DialogManager _dialogManager;
        protected Karma _karma;

        public override void Start()
        {
            base.Start();
            _character = FindObjectOfType<Character>();
            _dialogManager = FindObjectOfType<DialogManager>();
            _karma = FindObjectOfType<Karma>();
            _refugeesSettings = FindObjectOfType<RefugeesSettings>();
        }

        public string Name { get; set; }

        public abstract void WakeUp();
        public abstract void Talk();

        public override void Interact()
        {
            Talk();
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
