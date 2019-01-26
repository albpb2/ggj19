using Assets.Scripts.Objects;
using Assets.Scripts.Player;

namespace Assets.Scripts.Refugees
{
    public abstract class Refugee : InteractableSceneObject
    {
        protected Character _character;
        protected RefugeeSpawningSpot _spawningSpot;

        public override void Start()
        {
            base.Start();
            _character = FindObjectOfType<Character>();
        }

        public abstract void WakeUp();

        public override void Interact()
        {
            _character.GiveObjectTo(this);
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
