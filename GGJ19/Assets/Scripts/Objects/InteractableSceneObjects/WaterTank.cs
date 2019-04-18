using Assets.Scripts.Player;

namespace Assets.Scripts.Objects.InteractableSceneObjects
{
    public class WaterTank : InteractableSceneObject
    {
        private BagHandler _bag;

        public override  void Start()
        {
            base.Start();
            
            _bag = FindObjectOfType<BagHandler>();
        }

        public override void Interact()
        {
            _bag.FillWater();
        }
    }
}
