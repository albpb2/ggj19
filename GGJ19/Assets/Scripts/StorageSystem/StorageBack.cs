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
            Rect rect = RectTransformToScreenSpace(_storageFront.GetComponent<RectTransform>());
            if (!rect.Contains(UnityEngine.Input.mousePosition))
            {
                _storage.CloseStorage();
            }
        }

        private static Rect RectTransformToScreenSpace(RectTransform transform)
        {
            Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
            return new Rect((Vector2)transform.position - (size * 0.5f), size);
        }
    }
}
