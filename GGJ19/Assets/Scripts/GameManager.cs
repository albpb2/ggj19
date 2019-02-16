﻿using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private Image _pauseImage;
        [SerializeField]
        private DayTransition _dayTransition;


        private TimeTracker _timeTracker;
        public bool Pause { get; set; }
        private bool _gameFreezed;

        public bool GameFreezed
        {
            get => _gameFreezed;
            set
            {
                _gameFreezed = value;
            }
        }

        public void Start()
        {
            _timeTracker = FindObjectOfType<TimeTracker>();

            _timeTracker.onDayEnded += StartDayTransition;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!IsOnAnyPauseState())
                {
                    PauseGame();
                }
                else if (IsOnPauseScreen())
                {
                    ContinueGame();
                }
            }
        }

        public void PauseGame()
        {
            GameFreezed = true;
            _pauseImage.gameObject.SetActive(true);
            Pause = true;
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void ContinueGame()
        {
            GameFreezed = false;
            _pauseImage.gameObject.SetActive(false);
            Pause = false;
        }

        private bool IsOnAnyPauseState()
        {
            return GameFreezed || Pause;
        }

        private bool IsOnPauseScreen() => Pause;

        private void StartDayTransition(int dayNumber)
        {
            _dayTransition.gameObject.SetActive(true);
        }
    }
}
