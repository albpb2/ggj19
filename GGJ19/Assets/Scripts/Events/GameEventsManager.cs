﻿using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public class GameEventsManager : MonoBehaviour
    {
        private TimeTracker _timeTracker;

        public List<GameEvent> DayEvents { get; set; }

        public List<GameEvent> PermanentEvents { get; set; }

        public void AddEvent(GameEvent gameEvent)
        {
            DayEvents.Add(gameEvent);
        }

        public void AddPermanentEvent(GameEvent gameEvent)
        {
            PermanentEvents.Add(gameEvent);
        }

        void Awake()
        {
            _timeTracker = FindObjectOfType<TimeTracker>();
        }

        void Start()
        {
            DayEvents = new List<GameEvent>();
            PermanentEvents = new List<GameEvent>();
            _timeTracker.onNewDayBegun += ResetDayEvents;
        }

        private void ResetDayEvents(int dayNumber)
        {
            DayEvents = new List<GameEvent>();
        }
    }
}
