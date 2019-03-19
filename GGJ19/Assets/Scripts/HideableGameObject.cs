using UnityEngine;

namespace Assets.Scripts
{
    public class HideableGameObject : MonoBehaviour, IUIHideable
    {
        public void HideUIElement()
        {
            gameObject.SetActive(false);
        }
    }
}
