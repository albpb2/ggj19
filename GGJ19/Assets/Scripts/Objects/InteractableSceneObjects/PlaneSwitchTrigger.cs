using System;
using UnityEngine;

namespace Assets.Scripts.Objects.InteractableSceneObjects
{
    public class PlaneSwitchTrigger : InteractableSceneObject
    {
        private LayerTransitionManager _layerTransitionManager;

        public override void Start()
        {
            base.Start();

            _layerTransitionManager = FindObjectOfType<LayerTransitionManager>();
        }

        public override void Interact()
        {
            _layerTransitionManager.TransitionToNextLayer();
        }
    }
}
