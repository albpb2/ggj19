using Assets.Scripts.Events;
using UnityEngine;

namespace Assets.Scripts.Player
{
    public class DiaryWriter : MonoBehaviour
    {
        private TimeTracker _timeTracker;
        private GameEventsManager _gameEventsManager;
        private Karma _karma;

        public void WriteDiary(int dayNumber)
        {
            _gameEventsManager.AddPermanentEvent(new DayEndedEvent
            {
                Day = dayNumber,
                Karma = _karma.Amount
            });
        }

        void Awake()
        {
            _timeTracker = FindObjectOfType<TimeTracker>();
            _gameEventsManager = FindObjectOfType<GameEventsManager>();
            _karma = FindObjectOfType<Karma>();
        }

        private void Start()
        {
            _timeTracker.onDayEnded += WriteDiary;
        }
    }
}
