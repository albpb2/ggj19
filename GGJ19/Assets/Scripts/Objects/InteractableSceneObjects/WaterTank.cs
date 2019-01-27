using Assets.Scripts.Player;

namespace Assets.Scripts.Objects.InteractableSceneObjects
{
    public class WaterTank : InteractableSceneObject
    {
        private Bag _bag;

        public override  void Start()
        {
            base.Start();
            
            _bag = FindObjectOfType<Bag>();
        }

        public override void Interact()
        {
            _bag.FillWater();
        }
    }
}
