using Assets.Scripts.Objects.PortableObjects;

namespace Assets.Scripts.Objects.InteractableSceneObjects
{
    public class WaterWell : InteractableSceneObject
    {
        public override void Interact()
        {
            _player.TakePortableObject(new Water());
        }
    }
}
