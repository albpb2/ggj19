using Assets.Scripts.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.StorageSystem
{
    public class StorageBack : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField]
        private GameObject _storageFront;

        private Storage _storage;

        public void Start()
        {
            _storage = FindObjectOfType<Storage>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            HidePanelIfClickOutside();
        }

        private void HidePanelIfClickOutside()
        {
            Rect rect = _storageFront.GetComponent<RectTransform>().GetScreenSpaceRect();
            if (!rect.Contains(Input.mousePosition))
            {
                _storage.CloseStorage();
            }
        }
    }
}
