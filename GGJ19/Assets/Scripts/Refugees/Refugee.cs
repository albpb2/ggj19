using Assets.Scripts.Objects;
using Assets.Scripts.Player;

namespace Assets.Scripts.Refugees
{
    public abstract class Refugee : InteractableSceneObject
    {
        protected Character _character;
        protected RefugeeSpawningSpot _spawningSpot;
        protected RefugeesSettings _refugeesSettings;
        protected Karma _karma;

        public override void Start()
        {
            base.Start();
            _character = FindObjectOfType<Character>();
            _refugeesSettings = FindObjectOfType<RefugeesSettings>();
            _karma = FindObjectOfType<Karma>();
        }

        public abstract void WakeUp();

        public override void Interact()
        {
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
