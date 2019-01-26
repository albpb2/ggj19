using Assets.Scripts.Objects;
using Assets.Scripts.Player;

namespace Assets.Scripts.Refugees
{
    public abstract class Refugee : InteractableSceneObject
    {
        private Character _character;

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
    }
}
