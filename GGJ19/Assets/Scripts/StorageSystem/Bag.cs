using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.StorageSystem
{
    public class Bag : MonoBehaviour
    {
        [SerializeField]
        private Image _closeButton;
        [SerializeField]
        private Sprite _fullWaterSprite;
        [SerializeField]
        private Sprite _emptyWaterSprite;
        [SerializeField]
        private Vector2 _firstItemPosition;
        [SerializeField]
        private Vector2 _secondItemPosition;

        public Image CloseButton => _closeButton;

        public Sprite FullWaterSprite => _fullWaterSprite;

        public Sprite EmptyWaterSprite => _emptyWaterSprite;

        public Vector2 FirstItemPosition => _firstItemPosition;

        public Vector2 SecondItemPosition => _secondItemPosition;

        public void SetFullWaterSprite()
        {
            GetComponent<Image>().sprite = _fullWaterSprite;
        }

        public void SetEmptyWaterSprite()
        {
            GetComponent<Image>().sprite = _emptyWaterSprite;
        }
    }
}
