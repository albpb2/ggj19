using Assets.Scripts.Objects.InteractableSceneObjects;

namespace Assets.Scripts.Objects.PortableObjects
{
    public static class PortableObjectTypeExtensions
    {
        public static bool IsOfGiftType(this PortableObjectType type)
        {
            return Gifts.GetGiftPortableObjectTypes().Contains(type);
        }
    }
}
