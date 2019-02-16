using UnityEngine;

namespace Assets.Scripts.UI
{
    public class ActionsBox : MonoBehaviour, IUIHideable
    {
        public void HideUIElement()
        {
            gameObject.SetActive(false);
        }
    }
}
