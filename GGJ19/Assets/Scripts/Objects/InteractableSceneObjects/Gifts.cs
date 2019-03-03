using System.Collections.Generic;
using Assets.Scripts.Objects.PortableObjects;

namespace Assets.Scripts.Objects.InteractableSceneObjects
{
    public static class Gifts
    {
        public static List<PortableObjectType> GetGiftPortableObjectTypes()
        {
            return new List<PortableObjectType>
            {
                PortableObjectType.Rose,
                PortableObjectType.Ball,
                PortableObjectType.Book,
                PortableObjectType.Guitar,
                PortableObjectType.Toy,
            };
        }
    }
}
