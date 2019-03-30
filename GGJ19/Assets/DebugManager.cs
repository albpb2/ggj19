using UnityEngine;

namespace Assets
{
    public class DebugManager : MonoBehaviour
    {
        [SerializeField]
        private bool _disableKarmaReduction;
        [SerializeField]
        private bool _disableKarmaIncrease;

        public bool DisableKarmaReduction => Debug.isDebugBuild && _disableKarmaReduction;

        public bool DisableKarmaIncrease => Debug.isDebugBuild && _disableKarmaIncrease;
    }
}
