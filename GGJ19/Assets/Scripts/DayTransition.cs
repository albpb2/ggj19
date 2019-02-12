using System.Collections;
using System.Linq;
using Assets.Scripts.Events;
using Assets.Scripts.Extensions;
using Assets.Scripts.Refugees;
using Assets.Scripts.Refugees.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class DayTransition : MonoBehaviour, IPointerDownHandler
    {
        private const string DaySummaryTitleName = "DaySummaryTitle";
        private const string HungerSummaryTextName = "HungerSummaryText";
        private const string ThirstSummaryTextName = "ThirstSummaryText";
        private const string ColdSummaryTextName = "ColdSummaryText";
        private const string IllnessSummaryTextName = "IllnessSummaryText";

        private TimeTracker _timeTracker;
        private GameEventsManager _gameEventsManager;
        private Image _image;

        private bool _transitionEnded;
        private Text _summaryTitleText;
        private Text _hungerSummaryText;
        private Text _thirstSummaryText;
        private Text _coldSummaryText;
        private Text _illnessSummaryText;

        void Awake()
        {
            _timeTracker = FindObjectOfType<TimeTracker>();
            _gameEventsManager = FindObjectOfType<GameEventsManager>();
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
            _hungerSummaryText = texts.Single(t => t.name == HungerSummaryTextName);
            _thirstSummaryText = texts.Single(t => t.name == ThirstSummaryTextName);
            _coldSummaryText = texts.Single(t => t.name == ColdSummaryTextName);
            _illnessSummaryText = texts.Single(t => t.name == IllnessSummaryTextName);
        }

        private void HideComponents()
        {
            _summaryTitleText.gameObject.SetActive(false);
            _hungerSummaryText.gameObject.SetActive(false);
            _thirstSummaryText.gameObject.SetActive(false);
            _coldSummaryText.gameObject.SetActive(false);
            _illnessSummaryText.gameObject.SetActive(false);
        }
        
        private void ShowComponents()
        {
            _summaryTitleText.gameObject.SetActive(true);
            _hungerSummaryText.gameObject.SetActive(true);
            _thirstSummaryText.gameObject.SetActive(true);
            _coldSummaryText.gameObject.SetActive(true);
            _illnessSummaryText.gameObject.SetActive(true);
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
            SetBasicNeedsTexts();
        }

        private void SetDaySummaryTitle()
        {
            _summaryTitleText.text = $"DAY {_timeTracker.CurrentDay} - SUMMARY";
        }

        private void SetBasicNeedsTexts()
        {
            var refugees = FindObjectsOfType<RefugeeWithBasicNeeds>();

            var hungerSolved = _gameEventsManager.DayEvents.Count(e => e is HungerSolvedGameEvent);
            var hungryRefugees = refugees.Count(r => !r.HungerResolved);
            _hungerSummaryText.text = $"You helped {hungerSolved}/{hungryRefugees + hungerSolved} hungry people";

            var thirstSolved = _gameEventsManager.DayEvents.Count(e => e is ThirstSolvedEvent);
            var thirstyRefugees = refugees.Count(r => !r.ThirstResolved);
            _thirstSummaryText.text = $"You helped {thirstSolved}/{thirstyRefugees + thirstSolved} thirsty people";

            var coldSolved = _gameEventsManager.DayEvents.Count(e => e is ColdSolvedEvent);
            var coldRefugees = refugees.Count(r => !r.ColdResolved);
            _coldSummaryText.text = $"You helped {coldSolved}/{coldRefugees + coldSolved} cold people";

            var illnessSolved = _gameEventsManager.DayEvents.Count(e => e is IllnessSolvedEvent);
            var illRefugees = refugees.Count(r => !r.IllnessResolved && r.Ill);
            _illnessSummaryText.text = $"You helped {illnessSolved}/{illRefugees + illnessSolved} ill people";
        }
    }
}
