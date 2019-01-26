using System;
using UnityEngine;

namespace Assets.Scripts.Objects.InteractableSceneObjects
{
    public class PlaneSwitchTrigger : InteractableSceneObject
    {
        public override void Interact()
        {
            Debug.Log("Switch plane");
        }
    }
}
