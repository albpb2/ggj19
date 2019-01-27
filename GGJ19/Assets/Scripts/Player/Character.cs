using UnityEngine;

namespace Assets.Scripts.Player
{
    public class Character : MonoBehaviour
    {
        private Karma _karma;

        public void Start()
        {
            _karma = GetComponent<Karma>();
        }
    }
}
