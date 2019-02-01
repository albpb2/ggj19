using UnityEngine;

namespace Assets.Scripts.Objects.InteractableSceneObjects
{
    public class PlaneSwitchTrigger : InteractableSceneObject
    {
        private OldLayerTransitionManager _layerTransitionManager;

        [SerializeField]
        private bool _advance;

        public override void Start()
        {
            base.Start();

            _layerTransitionManager = FindObjectOfType<OldLayerTransitionManager>();
        }

        public override void Interact()
        {
            if (_advance)
            {
                _layerTransitionManager.TransitionToNextLayer();
            }
            else
            {
                _layerTransitionManager.TransitionToPreviousLayer();
            }
        }
    }
}
