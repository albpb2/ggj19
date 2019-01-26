using System;
using Assets.Scripts.Objects;
using Assets.Scripts.Refugees;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Character : MonoBehaviour
    {
        private Karma _karma;

        public PortableObject PortableObject { get; private set; }

        public void Start()
        {
            _karma = GetComponent<Karma>();
        }

        public void TakePortableObject(PortableObject portableObject)
        {
            if (PortableObject == null)
            {
                PortableObject = portableObject;
            }
        }

        public void GiveObjectTo(Refugee refugee)
        {
            if (PortableObject != null)
            {
                PortableObject = null;
                _karma.Increment(5);
            }
        }
    }
}
