using System.Collections;
using System.Linq;
using Assets.Scripts.Extensions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class DayTransition : MonoBehaviour, IPointerDownHandler
    {
        private const string DaySummaryTitleName = "DaySummaryTitle";

        private TimeTracker _timeTracker;
        private Image _image;

        private bool _transitionEnded;
        private Text _summaryTitleText;

        void Awake()
        {
            _timeTracker = FindObjectOfType<TimeTracker>();
            _image = GetComponent<Image>();

            FindTextFields();
        }

        void OnEnable()
        {
            _transitionEnded = false;

            _image.color = new Color(
                _image.color.r,
                _image.color.g,
                _image.color.b,
                0);

            HideComponents();
            StartCoroutine(TransitionToBlackScreen());
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_transitionEnded)
            {
                _timeTracker.BeginNewDay();
                gameObject.SetActive(false);
            }
        }

        private void FindTextFields()
        {
            var texts = GetComponentsInChildren<Text>();
            _summaryTitleText = texts.Single(t => t.name == DaySummaryTitleName);
        }

        private void HideComponents()
        {
            _summaryTitleText.gameObject.SetActive(false);
        }
        
        private void ShowComponents()
        {
            _summaryTitleText.gameObject.SetActive(true);
        }

        private IEnumerator TransitionToBlackScreen()
        {
            while (_image.color.a < 1)
            {
                yield return new WaitForSeconds(0.1f);
                _image.color = _image.color.IncreaseAlpha(0.1f);
            }

            _transitionEnded = true;
            ShowComponents();
            SetDaySummaryTitle();
        }

        private void SetDaySummaryTitle()
        {
            _summaryTitleText.text = $"DAY {_timeTracker.CurrentDay} - SUMMARY";
        }
    }
}
