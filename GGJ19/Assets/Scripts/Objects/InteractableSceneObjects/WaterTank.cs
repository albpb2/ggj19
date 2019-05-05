using Assets.Scripts.Audio;
using Assets.Scripts.Player;

namespace Assets.Scripts.Objects.InteractableSceneObjects
{
    public class WaterTank : InteractableSceneObject
    {
        private const Sound FillWaterSound = Sound.FillWatter;

        private BagHandler _bag;
        private OneShotSoundPlayer _oneShotSoundPlayer;

        public override  void Start()
        {
            base.Start();
            
            _bag = FindObjectOfType<BagHandler>();
            _oneShotSoundPlayer = FindObjectOfType<OneShotSoundPlayer>();
        }

        public override void Interact()
        {
            _bag.FillWater();
            _oneShotSoundPlayer.Play(FillWaterSound);
        }
    }
}
